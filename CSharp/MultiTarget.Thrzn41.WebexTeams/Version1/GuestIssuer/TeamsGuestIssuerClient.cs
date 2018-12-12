/* 
 * MIT License
 * 
 * Copyright(c) 2018 thrzn41
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Thrzn41.Util;

namespace Thrzn41.WebexTeams.Version1.GuestIssuer
{

    /// <summary>
    /// Cisco Webex Teams Guest Issuer Client for API version 1.
    /// </summary>
    public class TeamsGuestIssuerClient : IDisposable
    {


        /// <summary>
        /// Teams Guest Issuer API Path.
        /// </summary>
        protected static readonly string TEAMS_GUEST_ISSUER_API_PATH = TeamsAPIClient.GetAPIPath("jwt/login");


        /// <summary>
        /// Teams Guest Issuer API Uri.
        /// </summary>
        protected static readonly Uri TEAMS_GUEST_ISSUER_API_URI = new Uri(TEAMS_GUEST_ISSUER_API_PATH);


        /// <summary>
        /// Internal Encoding of this class.
        /// </summary>
        private static readonly Encoding ENCODING = UTF8Utils.UTF8_WITHOUT_BOM;

        /// <summary>
        /// base64url encoded JWT Header.
        /// </summary>
        private static readonly string TEAMS_GUEST_ISSUER_ENCODED_JWT_HEADER = encodeBase64Url( (new JWTHeader()).ToJsonString() );


        /// <summary>
        /// Unix time base.
        /// </summary>
        private static readonly DateTime UNIX_TIME_BASE = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Default time of expiration of issuer token.
        /// </summary>
        private static readonly TimeSpan DEFAULT_ISSUER_TOKEN_EXPIRES_IN = TimeSpan.FromSeconds(90.0);


        /// <summary>
        /// HttpClient for Guest Issuer API.
        /// </summary>
        protected readonly TeamsHttpClient teamsHttpClient;


        /// <summary>
        /// <see cref="HashAlgorithm"/> to sign JWT Header and Payload.
        /// </summary>
        private readonly HashAlgorithm jwtSigner;

        /// <summary>
        /// Guest Issuer Id.
        /// </summary>
        private readonly string guestIssuerId;




        /// <summary>
        /// Constuctor of TeamsGuestIssuerClient.
        /// </summary>
        /// <param name="secret">Secret of the Guest Issuer.</param>
        /// <param name="guestIssuerId">Guest Issuer ID.</param>
        /// <param name="retryExecutor">Executor for retry.</param>
        /// <param name="retryNotificationFunc">Notification func for retry.</param>
        internal TeamsGuestIssuerClient(string secret, string guestIssuerId, TeamsRetry retryExecutor = null, Func<TeamsResultInfo, int, bool> retryNotificationFunc = null)
        {
            this.teamsHttpClient = new TeamsHttpClient(null, TeamsAPIClient.TEAMS_API_URI_PATTERN, retryExecutor, retryNotificationFunc);

            // For now, Webex Teams Guest Issuer uses HMAC-SHA256 for JWT signature.
            this.jwtSigner = new HMACSHA256( Convert.FromBase64String(secret) );

            this.guestIssuerId = guestIssuerId;
        }


        /// <summary>
        /// Encodes to base64url string.
        /// </summary>
        /// <param name="data">data to be encoded.</param>
        /// <returns>base64url encoded string.</returns>
        private static string encodeBase64Url(byte[] data)
        {
#if (DOTNETSTANDARD1_3 || DOTNETCORE1_0)
            string base64Str = Convert.ToBase64String(data);
#else
            string base64Str = Convert.ToBase64String(data, Base64FormattingOptions.None);
#endif

            return base64Str.TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        /// <summary>
        /// Encodes to base64url string.
        /// </summary>
        /// <param name="str"><see cref="string"/> to be encoded.</param>
        /// <returns>base64url encoded string.</returns>
        private static string encodeBase64Url(string str)
        {
            return encodeBase64Url( ENCODING.GetBytes(str) );
        }



        /// <summary>
        /// Converts Guest Issuer token result to Teams token result.
        /// </summary>
        /// <param name="source">Source result.</param>
        /// <param name="refreshedAt"><see cref="DateTime"/> when the token refreshed.</param>
        /// <param name="guestUserId">Guest user id.</param>
        /// <param name="displayName">Guest user display name.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        private static TeamsResult<GuestUserInfo> convert(TeamsResult<GuestTokenInternalInfo> source, DateTime refreshedAt, string guestUserId, string displayName)
        {
            var result = new TeamsResult<GuestUserInfo>();

            source.CopyInfoTo(result);


            var guestUserInfo = new GuestUserInfo();

            var data = source.Data;

            if (result.IsSuccessStatus)
            {
                guestUserInfo.UserId      = guestUserId;
                guestUserInfo.DisplayName = displayName;

                guestUserInfo.RefreshedAt = refreshedAt;

                guestUserInfo.AccessToken = data.Token;

                if (data.ExpiresIn.HasValue)
                {
                    guestUserInfo.AccessTokenExpiresIn = TimeSpan.FromSeconds(data.ExpiresIn.Value);
                    guestUserInfo.AccessTokenExpiresAt = refreshedAt + guestUserInfo.AccessTokenExpiresIn;
                }
            }

            if (data.HasExtensionData)
            {
                guestUserInfo.JsonExtensionData = data.JsonExtensionData;
            }

            guestUserInfo.HasValues = data.HasValues;

            result.Data = guestUserInfo;

            return result;
        }


        /// <summary>
        /// Creates a Guest user.
        /// </summary>
        /// <param name="subject">The subject of the token. A unique, public identifier for the end-user of the token. This claim may contain only letters, numbers, and hyphens.</param>
        /// <param name="name">The display name of the guest user. This will be the name shown in Webex Teams clients.</param>
        /// <param name="expiresAt">The expiration time of the token. Use the lowest practical value for the use of the token.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        private async Task< TeamsResult<GuestUserInfo> > createGuestUserAsync(string subject, string name, DateTime expiresAt, CancellationToken? cancellationToken = null)
        {
            var expirationTime = expiresAt.ToUniversalTime() - UNIX_TIME_BASE;

            var payload = new JWTPayload
                            {
                                Subject        = subject,
                                Name           = name,
                                Issuer         = this.guestIssuerId,
                                ExpirationTime = Convert.ToInt64(expirationTime.TotalSeconds),
                            };

            var jwtToken = new StringBuilder(TEAMS_GUEST_ISSUER_ENCODED_JWT_HEADER);

            jwtToken.Append('.').Append( encodeBase64Url(payload.ToJsonString()) );

            var data = this.jwtSigner.ComputeHash( ENCODING.GetBytes(jwtToken.ToString()) );

            jwtToken.Append('.').Append( encodeBase64Url(data) );



            DateTime refreshedAt = DateTime.UtcNow;

            var result = await this.teamsHttpClient.RequestJsonWithBearerTokenAsync<TeamsResult<GuestTokenInternalInfo>, GuestTokenInternalInfo>(
                                    HttpMethod.Post,
                                    TEAMS_GUEST_ISSUER_API_URI,
                                    null,
                                    null,
                                    jwtToken.ToString(),
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return convert(result, refreshedAt, subject, name);
        }


        /// <summary>
        /// Creates a Guest user.
        /// </summary>
        /// <param name="guestUserId">You can specify a unique, public identifier for the guest user. This id may contain only letters, numbers, and hyphens. If the same guestUserId is used in the same Guest Issuer app, the guest user will be treated as the same user.</param>
        /// <param name="displayName">The display name of the guest user. This will be the name shown in Webex Teams clients.</param>
        /// <param name="issuerTokenExpiresIn">The expiration time of the issuer token. The issuer token is generated by the API client to create the guest user. The issuer token is NOT the same as the Access token of guest user. The issuer token is used on creating the guest user and should be available while this creation process. Use the lowest practical value for the use of the issuer token. Make sure your local clock is accurate or synced with NTP. Otherwise, the issuer token may be already expired now.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<GuestUserInfo> > CreateGuestUserAsync(string guestUserId, string displayName, TimeSpan issuerTokenExpiresIn, CancellationToken? cancellationToken = null)
        {
            return createGuestUserAsync(guestUserId, displayName, (DateTime.UtcNow + issuerTokenExpiresIn), cancellationToken);
        }

        /// <summary>
        /// Creates a Guest user.
        /// </summary>
        /// <param name="guestUserId">You can specify a unique, public identifier for the guest user. This id may contain only letters, numbers, and hyphens. If the same guestUserId is used in the same Guest Issuer app, the guest user will be treated as the same user.</param>
        /// <param name="displayName">The display name of the guest user. This will be the name shown in Webex Teams clients.</param>
        /// <param name="issuerTokenExpiresAt">The expiration time of the issuer token. The issuer token is generated by the API client to create the guest user. The issuer token is NOT the same as the Access token of guest user. The issuer token is used on creating the guest user and should be available while this creation process. Use the lowest practical value for the use of the issuer token. Make sure your local clock is accurate or synced with NTP. Otherwise, the issuer token may be already expired now.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<GuestUserInfo> > CreateGuestUserAsync(string guestUserId, string displayName, DateTime issuerTokenExpiresAt, CancellationToken? cancellationToken = null)
        {
            return createGuestUserAsync(guestUserId, displayName, issuerTokenExpiresAt, cancellationToken);
        }

        /// <summary>
        /// Creates a Guest user.
        /// Make sure your local clock is accurate or synced with NTP. Otherwise, it may fail to create guest user because of expiration of internal token.
        /// </summary>
        /// <param name="guestUserId">You can specify a unique, public identifier for the guest user. This id may contain only letters, numbers, and hyphens. If the same guestUserId is used in the same Guest Issuer app, the guest user will be treated as the same user.</param>
        /// <param name="displayName">The display name of the guest user. This will be the name shown in Webex Teams clients.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<GuestUserInfo> > CreateGuestUserAsync(string guestUserId, string displayName, CancellationToken? cancellationToken = null)
        {
            return createGuestUserAsync(guestUserId, displayName, (DateTime.UtcNow + DEFAULT_ISSUER_TOKEN_EXPIRES_IN), cancellationToken);
        }

        /// <summary>
        /// Creates a Guest user with a guest user id which is generated dynamically.
        /// Make sure your local clock is accurate or synced with NTP. Otherwise, it may fail to create guest user because of expiration of issuer token.
        /// </summary>
        /// <param name="displayName">The display name of the guest user. This will be the name shown in Webex Teams clients.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<GuestUserInfo> > CreateGuestUserAsync(string displayName, CancellationToken? cancellationToken = null)
        {
            return (CreateGuestUserAsync(Guid.NewGuid().ToString("D"), displayName, cancellationToken));
        }


        /// <summary>
        /// Refreshed the Guest user token.
        /// </summary>
        /// <param name="guestUserInfo"><see cref="GuestUserInfo"/> which the token will be refreshed.</param>
        /// <param name="issuerTokenExpiresIn">The expiration time of the issuer token. The issuer token is generated by the API client to create the guest user. The issuer token is NOT the same as the Access token of guest user. The issuer token is used on creating the guest user and should be available while this creation process. Use the lowest practical value for the use of the issuer token. Make sure your local clock is accurate or synced with NTP. Otherwise, the issuer token may be already expired now.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<GuestUserInfo> > RefreshGuestUserTokenAsync(GuestUserInfo guestUserInfo, TimeSpan issuerTokenExpiresIn, CancellationToken? cancellationToken = null)
        {
            return (CreateGuestUserAsync(guestUserInfo.UserId, guestUserInfo.DisplayName, issuerTokenExpiresIn, cancellationToken));
        }

        /// <summary>
        /// Refreshed the Guest user token.
        /// </summary>
        /// <param name="guestUserInfo"><see cref="GuestUserInfo"/> which the token will be refreshed.</param>
        /// <param name="issuerTokenExpiresAt">The expiration time of the issuer token. The issuer token is generated by the API client to create the guest user. The issuer token is NOT the same as the Access token of guest user. The issuer token is used on creating the guest user and should be available while this creation process. Use the lowest practical value for the use of the issuer token. Make sure your local clock is accurate or synced with NTP. Otherwise, the issuer token may be already expired now.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<GuestUserInfo> > RefreshGuestUserTokenAsync(GuestUserInfo guestUserInfo, DateTime issuerTokenExpiresAt, CancellationToken? cancellationToken = null)
        {
            return (CreateGuestUserAsync(guestUserInfo.UserId, guestUserInfo.DisplayName, issuerTokenExpiresAt, cancellationToken));
        }

        /// <summary>
        /// Refreshed the Guest user token.
        /// Make sure your local clock is accurate or synced with NTP. Otherwise, it may fail to create guest user because of expiration of issuer token.
        /// </summary>
        /// <param name="guestUserInfo"><see cref="GuestUserInfo"/> which the token will be refreshed.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<GuestUserInfo> > RefreshGuestUserTokenAsync(GuestUserInfo guestUserInfo, CancellationToken? cancellationToken = null)
        {
            return (CreateGuestUserAsync(guestUserInfo.UserId, guestUserInfo.DisplayName, cancellationToken));
        }


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls


        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposing">disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    using (this.teamsHttpClient)
                    using (this.jwtSigner)
                    {

                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~TeamsOAuth2Client() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion


    }

}
