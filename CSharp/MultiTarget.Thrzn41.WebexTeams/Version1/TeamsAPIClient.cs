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
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Thrzn41.Util;

namespace Thrzn41.WebexTeams.Version1
{

    /// <summary>
    /// Cisco Webex Teams API Client for API version 1.
    /// </summary>
    public class TeamsAPIClient : IDisposable
    {

        /// <summary>
        /// Teams API Path.
        /// </summary>
        protected const string TEAMS_API_PATH = "https://webexapis.com/v1/";


        /// <summary>
        /// Teams person API Path.
        /// </summary>
        protected static readonly string TEAMS_PERSON_API_PATH = GetAPIPath("people");

        /// <summary>
        /// Teams spaces API Path.
        /// </summary>
        protected static readonly string TEAMS_SPACES_API_PATH = GetAPIPath("rooms");

        /// <summary>
        /// Teams memberships API Path.
        /// </summary>
        protected static readonly string TEAMS_SPACE_MEMBERSHIPS_API_PATH = GetAPIPath("memberships");

        /// <summary>
        /// Teams messages API Path.
        /// </summary>
        protected static readonly string TEAMS_MESSAGES_API_PATH = GetAPIPath("messages");

        /// <summary>
        /// Teams direct messages API Path.
        /// </summary>
        protected static readonly string TEAMS_DIRECT_MESSAGES_API_PATH = GetAPIPath("messages/direct");

        /// <summary>
        /// Teams attachment actions API Path.
        /// </summary>
        protected static readonly string TEAMS_ATTACHMENT_ACTIONS_API_PATH = GetAPIPath("attachment/actions");

        /// <summary>
        /// Teams teams API Path.
        /// </summary>
        protected static readonly string TEAMS_TEAMS_API_PATH = GetAPIPath("teams");

        /// <summary>
        /// Teams team memberships API Path.
        /// </summary>
        protected static readonly string TEAMS_TEAM_MEMBERSHIPS_API_PATH = GetAPIPath("team/memberships");

        /// <summary>
        /// Teams webhooks API Path.
        /// </summary>
        protected static readonly string TEAMS_WEBHOOKS_API_PATH = GetAPIPath("webhooks");


        /// <summary>
        /// Teams person API Uri.
        /// </summary>
        protected static readonly Uri TEAMS_PERSON_API_URI = new Uri(TEAMS_PERSON_API_PATH);

        /// <summary>
        /// Teams messages API Uri.
        /// </summary>
        protected static readonly Uri TEAMS_SPACES_API_URI = new Uri(TEAMS_SPACES_API_PATH);

        /// <summary>
        /// Teams memberships API Uri.
        /// </summary>
        protected static readonly Uri TEAMS_SPACE_MEMBERSHIPS_API_URI = new Uri(TEAMS_SPACE_MEMBERSHIPS_API_PATH);

        /// <summary>
        /// Teams messages API Uri.
        /// </summary>
        protected static readonly Uri TEAMS_MESSAGES_API_URI = new Uri(TEAMS_MESSAGES_API_PATH);

        /// <summary>
        /// Teams direct messages API Uri.
        /// </summary>
        protected static readonly Uri TEAMS_DIRECT_MESSAGES_API_URI = new Uri(TEAMS_DIRECT_MESSAGES_API_PATH);

        /// <summary>
        /// Teams attachment actions API Uri.
        /// </summary>
        protected static readonly Uri TEAMS_ATTACHMENT_ACTIONS_API_URI = new Uri(TEAMS_ATTACHMENT_ACTIONS_API_PATH);

        /// <summary>
        /// Teams teams API Uri.
        /// </summary>
        protected static readonly Uri TEAMS_TEAMS_API_URI = new Uri(TEAMS_TEAMS_API_PATH);

        /// <summary>
        /// Teams team memberships API Uri.
        /// </summary>
        protected static readonly Uri TEAMS_TEAM_MEMBERSHIPS_API_URI = new Uri(TEAMS_TEAM_MEMBERSHIPS_API_PATH);

        /// <summary>
        /// Teams webhooks API Uri.
        /// </summary>
        protected static readonly Uri TEAMS_WEBHOOKS_API_URI = new Uri(TEAMS_WEBHOOKS_API_PATH);


        /// <summary>
        /// Uri pattern of Teams API.
        /// </summary>
        internal readonly static Regex TEAMS_API_URI_PATTERN = new Regex(String.Format("(^{0})|(^https://api.ciscospark.com/v1/)", TEAMS_API_PATH), RegexOptions.Compiled, TimeSpan.FromSeconds(60.0f));

        /// <summary>
        /// Person email pattern.
        /// This regex pattern is Not intended to detect exact email pattern.
        /// This is a vague pattern intentionally.
        /// </summary>
        private readonly static Regex TEAMS_PERSON_EMAIL_PATTERN = new Regex("^[^@]+@[^@]+$", RegexOptions.Compiled, TimeSpan.FromSeconds(60.0f));


        /// <summary>
        /// Random generator.
        /// </summary>
        private readonly static CryptoRandom RAND = new CryptoRandom();


        /// <summary>
        /// <see cref="SlimLock"/> for my own person info.
        /// </summary>
        private readonly SlimLock lockForMyOwnPersonInfo;


        /// <summary>
        /// HttpClient for Teams API.
        /// </summary>
        protected readonly TeamsHttpClient teamsHttpClient;


        /// <summary>
        /// My own info cache.
        /// </summary>
        private TeamsResult<CachedPerson> cachedMe = null;



        /// <summary>
        /// Constructor of TeamsAPIClient.
        /// </summary>
        /// <param name="token">token of Teams API.</param>
        /// <param name="retryExecutor">Executor for retry.</param>
        /// <param name="retryNotificationFunc">Notification func for retry.</param>
        internal TeamsAPIClient(string token, TeamsRetry retryExecutor = null, Func<TeamsResultInfo, int, bool> retryNotificationFunc = null)
        {
            this.lockForMyOwnPersonInfo = new SlimLock();

            this.teamsHttpClient = new TeamsHttpClient(token, TEAMS_API_URI_PATTERN, retryExecutor, retryNotificationFunc);
        }


        /// <summary>
        /// Gets API path for each api.
        /// </summary>
        /// <param name="apiPath">Each api path.</param>
        /// <returns>Full path for the api.</returns>
        internal static string GetAPIPath(string apiPath)
        {
            return String.Format("{0}{1}", TEAMS_API_PATH, apiPath);
        }


        /// <summary>
        /// Detects person id type.
        /// </summary>
        /// <param name="personId">Person id to be detected.</param>
        /// <param name="personIdType">Person id type to be detected.</param>
        /// <returns>Detected <see cref="PersonIdType"/>.</returns>
        internal static PersonIdType DetectPersonIdType(string personId, PersonIdType personIdType)
        {
            var result = personIdType;

            if(personIdType == PersonIdType.Detect)
            {
                if( String.IsNullOrEmpty(personId) )
                {
                    result = PersonIdType.Id;
                }
                else if( TEAMS_PERSON_EMAIL_PATTERN.IsMatch(personId) )
                {
                    result = PersonIdType.Email;
                }
                else
                {
                    result = PersonIdType.Id;
                }
            }

            return result;
        }


        /// <summary>
        /// Builds comma separated string.
        /// </summary>
        /// <param name="values">Value list.</param>
        /// <returns>Comma separated string.</returns>
        internal static string BuildCommaSeparatedString(IEnumerable<string> values)
        {
            if(values == null)
            {
                return null;
            }

            var strs = new StringBuilder();

            var separator = String.Empty;

            foreach (var item in values)
            {
                if(item != null)
                {
                    strs.AppendFormat("{0}{1}", separator, item);
                    separator = ",";
                }
            }

            return strs.ToString();
        }

        /// <summary>
        /// Gets value or default.
        /// </summary>
        /// <typeparam name="TResult">Type of the value.</typeparam>
        /// <param name="value">Value.</param>
        /// <param name="defaultValue">Default value that is returned if value parameter is null.</param>
        /// <returns>Value or default.</returns>
        protected static TResult getValueOrDefault<TResult>(TResult value, TResult defaultValue)
        {
            return ((value != null) ? value : defaultValue);
        }




        #region Person APIs

        /// <summary>
        /// Lists people.
        /// </summary>
        /// <param name="email">List people with this email address. For non-admin requests, either this or displayName are required.</param>
        /// <param name="displayName">List people whose name starts with this string. For non-admin requests, either this or email are required.</param>
        /// <param name="ids">List people by ID. Accepts up to 85 person IDs.</param>
        /// <param name="max">Limit the maximum number of people in the response.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsListResult<PersonList> > ListPersonsAsync(string email = null, string displayName = null, IEnumerable<string> ids = null, int? max = null, CancellationToken? cancellationToken = null)
        {
            var queryParameters = new NameValueCollection();

            queryParameters.Add("email",       email);
            queryParameters.Add("displayName", displayName);
            queryParameters.Add("id",          BuildCommaSeparatedString(ids));
            queryParameters.Add("max",         max?.ToString());

            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsListResult<PersonList>, PersonList>(
                                    HttpMethod.Get,
                                    TEAMS_PERSON_API_URI,
                                    queryParameters,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Lists people.
        /// </summary>
        /// <param name="email">List people with this email address. For non-admin requests, either this or displayName are required.</param>
        /// <param name="displayName">List people whose name starts with this string. For non-admin requests, either this or email are required.</param>
        /// <param name="ids">List people by ID. Accepts up to 85 person IDs.</param>
        /// <param name="max">Limit the maximum number of people in the response.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsListResult<PersonList> > ListPeopleAsync(string email = null, string displayName = null, IEnumerable<string> ids = null, int? max = null, CancellationToken? cancellationToken = null)
        {
            return (ListPersonsAsync(email, displayName, ids, max, cancellationToken));
        }


        /// <summary>
        /// Get person detail.
        /// </summary>
        /// <param name="personId">Person id that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<Person> > GetPersonAsync(string personId, CancellationToken? cancellationToken = null)
        {
            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<Person>, Person>(
                                    HttpMethod.Get,
                                    new Uri(String.Format("{0}/{1}", TEAMS_PERSON_API_URI, Uri.EscapeDataString(personId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Get person detail.
        /// </summary>
        /// <param name="person"><see cref="Person"/> that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<Person> > GetPersonAsync(Person person, CancellationToken? cancellationToken = null)
        {
            return (GetPersonAsync(person.Id, cancellationToken));
        }


        /// <summary>
        /// Get my own detail.
        /// </summary>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<Person> > GetMeAsync(CancellationToken? cancellationToken = null)
        {
            return (GetPersonAsync("me", cancellationToken));
        }

        /// <summary>
        /// Get cached person detail.
        /// </summary>
        /// <param name="personId">Person id that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        private async Task< TeamsResult<CachedPerson> > getCachedPersonAsync(string personId, CancellationToken? cancellationToken = null)
        {
            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<CachedPerson>, CachedPerson>(
                                    HttpMethod.Get,
                                    new Uri(String.Format("{0}/{1}", TEAMS_PERSON_API_URI, Uri.EscapeDataString(personId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            if(result.IsSuccessStatus)
            {
                result.Data.CachedAt = DateTime.UtcNow;
            }

            return result;
        }

        /// <summary>
        /// Refreshes my own info in cache.
        /// </summary>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<CachedPerson> > RefreshCachedMeAsync(CancellationToken? cancellationToken = null)
        {
            var result = await getCachedPersonAsync("me", cancellationToken);

            this.lockForMyOwnPersonInfo.ExecuteInWriterLock(
                () =>
                {
                    var me = this.cachedMe;

                    if(me == null || !me.IsSuccessStatus || result.IsSuccessStatus)
                    {
                        this.cachedMe = result;
                    }
                });

            return result; 
        }

        /// <summary>
        /// Get my own detail from cache.
        /// </summary>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<CachedPerson> > GetMeFromCacheAsync(CancellationToken? cancellationToken = null)
        {
            var me = this.lockForMyOwnPersonInfo.ExecuteInReaderLock(
                () =>
                {
                    return this.cachedMe;
                }
                );

            if(me == null || !me.IsSuccessStatus)
            {
                me = await RefreshCachedMeAsync(cancellationToken);
            }

            return me;
        }

        #endregion


        #region Spaces APIs

        /// <summary>
        /// Lists spaces. 
        /// </summary>
        /// <param name="teamId">Limit the rooms to those associated with a team, by ID.</param>
        /// <param name="type"><see cref="SpaceType.Direct"/> returns all 1-to-1 rooms. <see cref="SpaceType.Group"/> returns all group rooms.If not specified or values are not matched, will return all room types.</param>
        /// <param name="sortBy">Sort results by space ID(<see cref="SpaceSortBy.Id"/>), most recent activity(<see cref="SpaceSortBy.LastActivity"/>), or most recently created(<see cref="SpaceSortBy.Created"/>).</param>
        /// <param name="max">Limit the maximum number of messages in the response.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsListResult<SpaceList> > ListSpacesAsync(string teamId = null, SpaceType type = null, SpaceSortBy sortBy = null, int? max = null, CancellationToken? cancellationToken = null)
        {
            var queryParameters = new NameValueCollection();

            queryParameters.Add("teamId", teamId);
            queryParameters.Add("max",    max?.ToString());
            queryParameters.Add("type",   type?.Name);
            queryParameters.Add("sortBy", sortBy?.Name);

            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsListResult<SpaceList>, SpaceList>(
                                    HttpMethod.Get,
                                    TEAMS_SPACES_API_URI,
                                    queryParameters,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }


        /// <summary>
        /// Create a space.
        /// </summary>
        /// <param name="title">A user-friendly name for the room.</param>
        /// <param name="teamId">The ID for the team with which this room is associated.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<Space> > CreateSpaceAsync(string title, string teamId = null, CancellationToken? cancellationToken = null)
        {
            var space = new Space();

            space.Title  = title;
            space.TeamId = teamId;

            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<Space>, Space>(
                                    HttpMethod.Post,
                                    TEAMS_SPACES_API_URI,
                                    null,
                                    space,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }


        /// <summary>
        /// Gets space detail.
        /// </summary>
        /// <param name="spaceId">Space id that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<Space> > GetSpaceAsync(string spaceId, CancellationToken? cancellationToken = null)
        {
            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<Space>, Space>(
                                    HttpMethod.Get,
                                    new Uri(String.Format("{0}/{1}", TEAMS_SPACES_API_PATH, Uri.EscapeDataString(spaceId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Gets space detail.
        /// </summary>
        /// <param name="space"><see cref="Space"/> that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<Space> > GetSpaceAsync(Space space, CancellationToken? cancellationToken = null)
        {
            return (GetSpaceAsync(space.Id, cancellationToken));
        }

        /// <summary>
        /// Gets space meeting info detail.
        /// </summary>
        /// <param name="spaceId">Space id that the meeting info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<SpaceMeetingInfo> > GetSpaceMeetingInfoAsync(string spaceId, CancellationToken? cancellationToken = null)
        {
            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<SpaceMeetingInfo>, SpaceMeetingInfo>(
                                    HttpMethod.Get,
                                    new Uri(String.Format("{0}/{1}/meetingInfo", TEAMS_SPACES_API_PATH, Uri.EscapeDataString(spaceId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Gets space meeting info detail.
        /// </summary>
        /// <param name="space"><see cref="Space"/> that the meeting info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<SpaceMeetingInfo> > GetSpaceMeetingInfoAsync(Space space, CancellationToken? cancellationToken = null)
        {
            return GetSpaceMeetingInfoAsync(space.Id, cancellationToken);
        }


        /// <summary>
        /// Updates space.
        /// </summary>
        /// <param name="spaceId">Space id to be updated.</param>
        /// <param name="title">A user-friendly name for the space.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<Space> > UpdateSpaceAsync(string spaceId, string title, CancellationToken? cancellationToken = null)
        {
            var space = new Space();

            space.Title = title;

            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<Space>, Space>(
                                    HttpMethod.Put,
                                    new Uri(String.Format("{0}/{1}", TEAMS_SPACES_API_PATH, Uri.EscapeDataString(spaceId))),
                                    null,
                                    space,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Updates space.
        /// </summary>
        /// <param name="space"><see cref="Space"/> to be updated.</param>
        /// <param name="title">A user-friendly name for the space.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<Space> > UpdateSpaceAsync(Space space, string title, CancellationToken? cancellationToken = null)
        {
            return (UpdateSpaceAsync(space.Id, title, cancellationToken));
        }


        /// <summary>
        /// Deletes a space.
        /// </summary>
        /// <param name="spaceId">Space id to be deleted.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<NoContent> > DeleteSpaceAsync(string spaceId, CancellationToken? cancellationToken = null)
        {
            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<NoContent>, NoContent>(
                                    HttpMethod.Delete,
                                    new Uri(String.Format("{0}/{1}", TEAMS_SPACES_API_PATH, Uri.EscapeDataString(spaceId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.NoContent));

            return result;
        }

        /// <summary>
        /// Deletes a space.
        /// </summary>
        /// <param name="space"><see cref="Space"/> to be deleted.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<NoContent> > DeleteSpaceAsync(Space space, CancellationToken? cancellationToken = null)
        {
            return (DeleteSpaceAsync(space.Id, cancellationToken));
        }

        #endregion


        #region SpaceMemberships APIs

        /// <summary>
        /// Lists space memberships.
        /// </summary>
        /// <param name="spaceId">Limit results to a specific space, by ID.</param>
        /// <param name="personIdOrEmail">Limit results to a specific person, by ID or Email.</param>
        /// <param name="max">Limit the maximum number of items in the response.</param>
        /// <param name="personIdType"><see cref="PersonIdType"/> for personIdOrEmail parameter.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsListResult<SpaceMembershipList> > ListSpaceMembershipsAsync(string spaceId = null, string personIdOrEmail = null, int? max = null, PersonIdType personIdType = PersonIdType.Detect, CancellationToken? cancellationToken = null)
        {
            personIdType = DetectPersonIdType(personIdOrEmail, personIdType);

            string personIdOrEmailKey;

            switch (personIdType)
            {
                case PersonIdType.Email:
                    personIdOrEmailKey = "personEmail";
                    break;
                default:
                    personIdOrEmailKey = "personId";
                    break;
            }

            var queryParameters = new NameValueCollection();

            queryParameters.Add("roomId",           spaceId);
            queryParameters.Add(personIdOrEmailKey, personIdOrEmail);
            queryParameters.Add("max",              max?.ToString());

            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsListResult<SpaceMembershipList>, SpaceMembershipList>(
                                    HttpMethod.Get,
                                    TEAMS_SPACE_MEMBERSHIPS_API_URI,
                                    queryParameters,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Lists space memberships.
        /// </summary>
        /// <param name="space">Limit results to a specific <see cref="Space"/>.</param>
        /// <param name="person">Limit results to a specific <see cref="Person"/>.</param>
        /// <param name="max">Limit the maximum number of items in the response.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsListResult<SpaceMembershipList> > ListSpaceMembershipsAsync(Space space, Person person, int? max = null, CancellationToken? cancellationToken = null)
        {
            return (ListSpaceMembershipsAsync(space.Id, person?.Id, max, PersonIdType.Id, cancellationToken));
        }

        /// <summary>
        /// Lists space memberships.
        /// </summary>
        /// <param name="space">Limit results to a specific <see cref="Space"/>.</param>
        /// <param name="max">Limit the maximum number of items in the response.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsListResult<SpaceMembershipList> > ListSpaceMembershipsAsync(Space space, int? max = null, CancellationToken? cancellationToken = null)
        {
            return (ListSpaceMembershipsAsync(space.Id, null, max, PersonIdType.Id, cancellationToken));
        }

        /// <summary>
        /// Lists space memberships.
        /// </summary>
        /// <param name="person">Limit results to a specific <see cref="Person"/>.</param>
        /// <param name="max">Limit the maximum number of items in the response.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsListResult<SpaceMembershipList> > ListSpaceMembershipsAsync(Person person, int? max = null, CancellationToken? cancellationToken = null)
        {
            return (ListSpaceMembershipsAsync(null, person.Id, max, PersonIdType.Id, cancellationToken));
        }


        /// <summary>
        /// Create a space membership.
        /// </summary>
        /// <param name="spaceId">The space ID.</param>
        /// <param name="personIdOrEmail">The person ID or Email.</param>
        /// <param name="isModerator">Set to true to make the person a room moderator.</param>
        /// <param name="personIdType"><see cref="PersonIdType"/> for personIdOrEmail parameter.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<SpaceMembership> > CreateSpaceMembershipAsync(string spaceId, string personIdOrEmail, bool? isModerator = null, PersonIdType personIdType = PersonIdType.Detect, CancellationToken? cancellationToken = null)
        {
            var membership = new SpaceMembership();

            membership.SpaceId = spaceId;

            personIdType = DetectPersonIdType(personIdOrEmail, personIdType);

            switch (personIdType)
            {
                case PersonIdType.Email:
                    membership.PersonEmail = personIdOrEmail;
                    break;
                default:
                    membership.PersonId = personIdOrEmail;
                    break;
            }

            membership.IsModerator = isModerator;

            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<SpaceMembership>, SpaceMembership>(
                                    HttpMethod.Post,
                                    TEAMS_SPACE_MEMBERSHIPS_API_URI,
                                    null,
                                    membership,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Create a space membership.
        /// </summary>
        /// <param name="space"><see cref="Space"/>.</param>
        /// <param name="person"><see cref="Person"/>.</param>
        /// <param name="isModerator">Set to true to make the person a room moderator.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<SpaceMembership> > CreateSpaceMembershipAsync(Space space, Person person, bool? isModerator = null, CancellationToken? cancellationToken = null)
        {
            return (CreateSpaceMembershipAsync(space.Id, person.Id, isModerator, PersonIdType.Id, cancellationToken));
        }

        /// <summary>
        /// Create a space membership.
        /// </summary>
        /// <param name="space"><see cref="Space"/>.</param>
        /// <param name="personIdOrEmail">The person ID or Email.</param>
        /// <param name="isModerator">Set to true to make the person a room moderator.</param>
        /// <param name="personIdType"><see cref="PersonIdType"/> for personIdOrEmail parameter.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<SpaceMembership> > CreateSpaceMembershipAsync(Space space, string personIdOrEmail, bool? isModerator = null, PersonIdType personIdType = PersonIdType.Detect, CancellationToken? cancellationToken = null)
        {
            return (CreateSpaceMembershipAsync(space.Id, personIdOrEmail, isModerator, personIdType, cancellationToken));
        }


        /// <summary>
        /// Gets space membership detail.
        /// </summary>
        /// <param name="membershipId">Space Membership id that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<SpaceMembership> > GetSpaceMembershipAsync(string membershipId, CancellationToken? cancellationToken = null)
        {
            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<SpaceMembership>, SpaceMembership>(
                                    HttpMethod.Get,
                                    new Uri(String.Format("{0}/{1}", TEAMS_SPACE_MEMBERSHIPS_API_PATH, Uri.EscapeDataString(membershipId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Gets space membership detail.
        /// </summary>
        /// <param name="membership"><see cref="SpaceMembership"/> that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<SpaceMembership> > GetSpaceMembershipAsync(SpaceMembership membership, CancellationToken? cancellationToken = null)
        {
            return (GetSpaceMembershipAsync(membership.Id, cancellationToken));
        }


        /// <summary>
        /// Updates space membership.
        /// </summary>
        /// <param name="membershipId">Membership id to be updated.</param>
        /// <param name="isModerator">Set to true to make the person a space moderator.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<SpaceMembership> > UpdateSpaceMembershipAsync(string membershipId, bool isModerator, CancellationToken? cancellationToken = null)
        {
            var membership = new SpaceMembership();

            membership.IsModerator = isModerator;

            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<SpaceMembership>, SpaceMembership>(
                                    HttpMethod.Put,
                                    new Uri(String.Format("{0}/{1}", TEAMS_SPACE_MEMBERSHIPS_API_PATH, Uri.EscapeDataString(membershipId))),
                                    null,
                                    membership,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Updates space membership.
        /// </summary>
        /// <param name="membership"><see cref="SpaceMembership"/> to be updated.</param>
        /// <param name="isModerator">Set to true to make the person a space moderator.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<SpaceMembership> > UpdateSpaceMembershipAsync(SpaceMembership membership, bool isModerator, CancellationToken? cancellationToken = null)
        {
            return (UpdateSpaceMembershipAsync(membership.Id, isModerator, cancellationToken));
        }


        /// <summary>
        /// Deletes a space membership.
        /// </summary>
        /// <param name="membershipId">Space Membership id to be deleted.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<NoContent> > DeleteSpaceMembershipAsync(string membershipId, CancellationToken? cancellationToken = null)
        {
            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<NoContent>, NoContent>(
                                    HttpMethod.Delete,
                                    new Uri(String.Format("{0}/{1}", TEAMS_SPACE_MEMBERSHIPS_API_PATH, Uri.EscapeDataString(membershipId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.NoContent));

            return result;
        }

        /// <summary>
        /// Deletes a space membership.
        /// </summary>
        /// <param name="membership"><see cref="SpaceMembership"/> to be deleted.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<NoContent> > DeleteSpaceMembershipAsync(SpaceMembership membership, CancellationToken? cancellationToken = null)
        {
            return (DeleteSpaceMembershipAsync(membership.Id, cancellationToken));
        }

        #endregion


        #region Messages APIs

        /// <summary>
        /// Lists messages.
        /// </summary>
        /// <param name="spaceId">List messages for a space, by ID.</param>
        /// <param name="mentionedPeople">List messages where the caller is mentioned by specifying "me" or the caller personId.</param>
        /// <param name="before">List messages sent before a date and time.</param>
        /// <param name="beforeMessage">List messages sent before a message, by ID.</param>
        /// <param name="max">Limit the maximum number of messages in the response.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsListResult<MessageList> > ListMessagesAsync(string spaceId, string mentionedPeople = null, DateTime? before = null, string beforeMessage = null, int? max = null, CancellationToken? cancellationToken = null)
        {
            var queryParameters = new NameValueCollection();

            queryParameters.Add("roomId",          spaceId);
            queryParameters.Add("max",             max?.ToString());
            queryParameters.Add("mentionedPeople", mentionedPeople);
            queryParameters.Add("before",          before?.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffK"));
            queryParameters.Add("beforeMessage",   beforeMessage);

            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsListResult<MessageList>, MessageList>(
                                    HttpMethod.Get,
                                    TEAMS_MESSAGES_API_URI,
                                    queryParameters,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Lists messages.
        /// </summary>
        /// <param name="space">List messages for <see cref="Space"/>.</param>
        /// <param name="mentionedPeople">List messages where the caller is mentioned by specifying "me" or the caller personId.</param>
        /// <param name="before">List messages sent before a date and time.</param>
        /// <param name="beforeMessage">List messages sent before a message, by ID.</param>
        /// <param name="max">Limit the maximum number of messages in the response.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsListResult<MessageList> > ListMessagesAsync(Space space, string mentionedPeople = null, DateTime? before = null, string beforeMessage = null, int? max = null, CancellationToken? cancellationToken = null)
        {
            return (ListMessagesAsync(space.Id, mentionedPeople, before, beforeMessage, max, cancellationToken));
        }

        /// <summary>
        /// Lists direct messages.
        /// </summary>
        /// <param name="personIdOrEmail">Person id or email that the message is posted.</param>
        /// <param name="personIdType"><see cref="PersonIdType"/> of personIdOrEmail parameter.</param>
        /// <param name="max">Limit the maximum number of messages in the response.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsListResult<MessageList> > ListDirectMessagesAsync(string personIdOrEmail, PersonIdType personIdType = PersonIdType.Detect, int? max = null, CancellationToken? cancellationToken = null)
        {
            personIdType = DetectPersonIdType(personIdOrEmail, personIdType);

            string parameterName = "personId";

            if(personIdType == PersonIdType.Email)
            {
                parameterName = "personEmail";
            }

            var queryParameters = new NameValueCollection();

            queryParameters.Add(parameterName, personIdOrEmail);
            queryParameters.Add("max", max?.ToString());

            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsListResult<MessageList>, MessageList>(
                                    HttpMethod.Get,
                                    TEAMS_DIRECT_MESSAGES_API_URI,
                                    queryParameters,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Lists direct messages.
        /// </summary>
        /// <param name="person"><see cref="Person"/> that the message is posted.</param>
        /// <param name="max">Limit the maximum number of messages in the response.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsListResult<MessageList> > ListDirectMessagesAsync(Person person, int? max = null, CancellationToken? cancellationToken = null)
        {
            return ListDirectMessagesAsync(person.Id, PersonIdType.Id, max, cancellationToken);
        }

        /// <summary>
        /// Creates a message.
        /// </summary>
        /// <param name="targetId">Id that the message is posted.</param>
        /// <param name="markdownOrText">markdown or text to be posted.</param>
        /// <param name="files">File uris to be attached with the message.</param>
        /// <param name="target"><see cref="MessageTarget"/> that the targetId parameter represents.</param>
        /// <param name="textType"><see cref="MessageTextType"/> of markdownOrText parameter.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<Message> > CreateMessageAsync(string targetId, string markdownOrText, IEnumerable<Uri> files = null, MessageTarget target = MessageTarget.SpaceId, MessageTextType textType = MessageTextType.Markdown, CancellationToken? cancellationToken = null)
        {
            var message = new Message();

            switch (target)
            {
                case MessageTarget.PersonId:
                    message.ToPersonId = targetId;
                    break;
                case MessageTarget.PersonEmail:
                    message.ToPersonEmail = targetId;
                    break;
                default:
                    message.SpaceId = targetId;
                    break;
            }

            switch (textType)
            {
                case MessageTextType.Text:
                    message.Text = markdownOrText;
                    break;
                default:
                    message.Markdown = markdownOrText;
                    break;
            }

            if(files != null)
            {
                List<string> fileList = new List<string>();

                foreach (var item in files)
                {
                    fileList.Add(item.AbsoluteUri);
                }

                message.Files = fileList.ToArray();
            }

            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<Message>, Message>(
                                    HttpMethod.Post,
                                    TEAMS_MESSAGES_API_URI,
                                    null,
                                    message,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }


        /// <summary>
        /// Creates a message.
        /// </summary>
        /// <param name="space"><see cref="Space"/> that the message is posted.</param>
        /// <param name="markdownOrText">markdown or text to be posted.</param>
        /// <param name="files">File uris to be attached with the message.</param>
        /// <param name="textType"><see cref="MessageTextType"/> of markdownOrText parameter.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<Message> > CreateMessageAsync(Space space, string markdownOrText, IEnumerable<Uri> files = null, MessageTextType textType = MessageTextType.Markdown, CancellationToken? cancellationToken = null)
        {
            return (CreateMessageAsync(space.Id, markdownOrText, files, MessageTarget.SpaceId, textType, cancellationToken));
        }

        /// <summary>
        /// Creates a message with an attachment.
        /// </summary>
        /// <param name="targetId">Id that the message is posted.</param>
        /// <param name="markdownOrText">markdown or text to be posted.</param>
        /// <param name="attachment">Message attachment to be attached with the message.</param>
        /// <param name="target"><see cref="MessageTarget"/> that the targetId parameter represents.</param>
        /// <param name="textType"><see cref="MessageTextType"/> of markdownOrText parameter.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<Message> > CreateMessageAsync(string targetId, string markdownOrText, Attachment attachment, MessageTarget target = MessageTarget.SpaceId, MessageTextType textType = MessageTextType.Markdown, CancellationToken? cancellationToken = null)
        {
            var message = new Message();

            switch (target)
            {
                case MessageTarget.PersonId:
                    message.ToPersonId = targetId;
                    break;
                case MessageTarget.PersonEmail:
                    message.ToPersonEmail = targetId;
                    break;
                default:
                    message.SpaceId = targetId;
                    break;
            }

            switch (textType)
            {
                case MessageTextType.Text:
                    message.Text = markdownOrText;
                    break;
                default:
                    message.Markdown = markdownOrText;
                    break;
            }

            if (attachment != null)
            {
                message.Attachments = new Attachment[] { attachment };
            }

            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<Message>, Message>(
                                    HttpMethod.Post,
                                    TEAMS_MESSAGES_API_URI,
                                    null,
                                    message,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Creates a message with an attachment.
        /// </summary>
        /// <param name="space"><see cref="Space"/> that the message is posted.</param>
        /// <param name="markdownOrText">markdown or text to be posted.</param>
        /// <param name="attachment">Message attachment to be attached with the message.</param>
        /// <param name="target"><see cref="MessageTarget"/> that the targetId parameter represents.</param>
        /// <param name="textType"><see cref="MessageTextType"/> of markdownOrText parameter.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<Message> > CreateMessageAsync(Space space, string markdownOrText, Attachment attachment, MessageTarget target = MessageTarget.SpaceId, MessageTextType textType = MessageTextType.Markdown, CancellationToken? cancellationToken = null)
        {
            return (CreateMessageAsync(space.Id, markdownOrText, attachment, MessageTarget.SpaceId, textType, cancellationToken));
        }


        /// <summary>
        /// Creates a message.
        /// </summary>
        /// <param name="targetId">Id that the message is posted.</param>
        /// <param name="markdownOrText">markdown or text to be posted.</param>
        /// <param name="fileData">File data to be attached with the message.</param>
        /// <param name="target"><see cref="MessageTarget"/> that the targetId parameter represents.</param>
        /// <param name="textType"><see cref="MessageTextType"/> of markdownOrText parameter.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<Message> > CreateMessageAsync(string targetId, string markdownOrText, TeamsFileData fileData, MessageTarget target = MessageTarget.SpaceId, MessageTextType textType = MessageTextType.Markdown, CancellationToken? cancellationToken = null)
        {
            string targetKey;
            string textKey;

            switch (target)
            {
                case MessageTarget.PersonId:
                    targetKey = "toPersonId";
                    break;
                case MessageTarget.PersonEmail:
                    targetKey = "toPersonEmail";
                    break;
                default:
                    targetKey = "roomId";
                    break;
            }

            switch (textType)
            {
                case MessageTextType.Text:
                    textKey = "text";
                    break;
                default:
                    textKey = "markdown";
                    break;
            }

            var stringData = new NameValueCollection();

            stringData.Add(targetKey, targetId);
            stringData.Add(textKey,   markdownOrText);

            var result = await this.teamsHttpClient.RequestMultipartFormDataAsync<TeamsResult<Message>, Message>(
                                    HttpMethod.Post,
                                    TEAMS_MESSAGES_API_URI,
                                    null,
                                    stringData,
                                    fileData,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Creates a message.
        /// </summary>
        /// <param name="space"><see cref="Space"/> that the message is posted.</param>
        /// <param name="markdownOrText">markdown or text to be posted.</param>
        /// <param name="fileData">File data to be attached with the message.</param>
        /// <param name="textType"><see cref="MessageTextType"/> of markdownOrText parameter.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<Message> > CreateMessageAsync(Space space, string markdownOrText, TeamsFileData fileData, MessageTextType textType = MessageTextType.Markdown, CancellationToken? cancellationToken = null)
        {
            return (CreateMessageAsync(space.Id, markdownOrText, fileData, MessageTarget.SpaceId, textType, cancellationToken));
        }



        /// <summary>
        /// Create a message to direct space.
        /// </summary>
        /// <param name="personIdOrEmail">Person id or email that the message is posted.</param>
        /// <param name="markdownOrText">markdown or text to be posted.</param>
        /// <param name="files">File uris to be attached with the message.</param>
        /// <param name="personIdType"><see cref="PersonIdType"/> of personIdOrEmail parameter.</param>
        /// <param name="textType"><see cref="MessageTextType"/> of markdownOrText parameter.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<Message> > CreateDirectMessageAsync(string personIdOrEmail, string markdownOrText, IEnumerable<Uri> files = null, PersonIdType personIdType = PersonIdType.Detect, MessageTextType textType = MessageTextType.Markdown, CancellationToken? cancellationToken = null)
        {
            personIdType = DetectPersonIdType(personIdOrEmail, personIdType);

            MessageTarget targetType;

            switch (personIdType)
            {
                case PersonIdType.Email:
                    targetType = MessageTarget.PersonEmail;
                    break;
                default:
                    targetType = MessageTarget.PersonId;
                    break;
            }

            return (CreateMessageAsync(personIdOrEmail, markdownOrText, files, targetType, textType, cancellationToken));
        }

        /// <summary>
        /// Create a message to direct space.
        /// </summary>
        /// <param name="personIdOrEmail">Person id or email that the message is posted.</param>
        /// <param name="markdownOrText">markdown or text to be posted.</param>
        /// <param name="fileData">File data to be attached with the message.</param>
        /// <param name="personIdType"><see cref="PersonIdType"/> of personIdOrEmail parameter.</param>
        /// <param name="textType"><see cref="MessageTextType"/> of markdownOrText parameter.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<Message> > CreateDirectMessageAsync(string personIdOrEmail, string markdownOrText, TeamsFileData fileData, PersonIdType personIdType = PersonIdType.Detect, MessageTextType textType = MessageTextType.Markdown, CancellationToken? cancellationToken = null)
        {
            personIdType = DetectPersonIdType(personIdOrEmail, personIdType);

            MessageTarget targetType;

            switch (personIdType)
            {
                case PersonIdType.Email:
                    targetType = MessageTarget.PersonEmail;
                    break;
                default:
                    targetType = MessageTarget.PersonId;
                    break;
            }

            return (CreateMessageAsync(personIdOrEmail, markdownOrText, fileData, targetType, textType, cancellationToken));
        }

        /// <summary>
        /// Create a message to direct space.
        /// </summary>
        /// <param name="person"><see cref="Person"/> that the message is posted.</param>
        /// <param name="markdownOrText">markdown or text to be posted.</param>
        /// <param name="files">File uris to be attached with the message.</param>
        /// <param name="textType"><see cref="MessageTextType"/> of markdownOrText parameter.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<Message> > CreateDirectMessageAsync(Person person, string markdownOrText, IEnumerable<Uri> files = null, MessageTextType textType = MessageTextType.Markdown, CancellationToken? cancellationToken = null)
        {
            return (CreateDirectMessageAsync(person.Id, markdownOrText, files, PersonIdType.Id, textType, cancellationToken));
        }

        /// <summary>
        /// Create a message to direct space.
        /// </summary>
        /// <param name="person"><see cref="Person"/> that the message is posted.</param>
        /// <param name="markdownOrText">markdown or text to be posted.</param>
        /// <param name="fileData">File data to be attached with the message.</param>
        /// <param name="textType"><see cref="MessageTextType"/> of markdownOrText parameter.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<Message> > CreateDirectMessageAsync(Person person, string markdownOrText, TeamsFileData fileData, MessageTextType textType = MessageTextType.Markdown, CancellationToken? cancellationToken = null)
        {
            return (CreateDirectMessageAsync(person.Id, markdownOrText, fileData, PersonIdType.Id, textType, cancellationToken));
        }


        /// <summary>
        /// Create a message to direct space with an attachment.
        /// </summary>
        /// <param name="personIdOrEmail">Person id or email that the message is posted.</param>
        /// <param name="markdownOrText">markdown or text to be posted.</param>
        /// <param name="attachment">Message attachment to be attached with the message.</param>
        /// <param name="personIdType"><see cref="PersonIdType"/> of personIdOrEmail parameter.</param>
        /// <param name="textType"><see cref="MessageTextType"/> of markdownOrText parameter.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task<TeamsResult<Message>> CreateDirectMessageAsync(string personIdOrEmail, string markdownOrText, Attachment attachment, PersonIdType personIdType = PersonIdType.Detect, MessageTextType textType = MessageTextType.Markdown, CancellationToken? cancellationToken = null)
        {
            personIdType = DetectPersonIdType(personIdOrEmail, personIdType);

            MessageTarget targetType;

            switch (personIdType)
            {
                case PersonIdType.Email:
                    targetType = MessageTarget.PersonEmail;
                    break;
                default:
                    targetType = MessageTarget.PersonId;
                    break;
            }

            return (CreateMessageAsync(personIdOrEmail, markdownOrText, attachment, targetType, textType, cancellationToken));
        }

        /// <summary>
        /// Create a message to direct space with an attachment.
        /// </summary>
        /// <param name="person"><see cref="Person"/> that the message is posted.</param>
        /// <param name="markdownOrText">markdown or text to be posted.</param>
        /// <param name="attachment">Message attachment to be attached with the message.</param>
        /// <param name="personIdType"><see cref="PersonIdType"/> of personIdOrEmail parameter.</param>
        /// <param name="textType"><see cref="MessageTextType"/> of markdownOrText parameter.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task<TeamsResult<Message>> CreateDirectMessageAsync(Person person, string markdownOrText, Attachment attachment, PersonIdType personIdType = PersonIdType.Detect, MessageTextType textType = MessageTextType.Markdown, CancellationToken? cancellationToken = null)
        {
            return (CreateDirectMessageAsync(person.Id, markdownOrText, attachment, PersonIdType.Id, textType, cancellationToken));
        }


        /// <summary>
        /// Gets message detail.
        /// </summary>
        /// <param name="messageId">Message id that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<Message> > GetMessageAsync(string messageId, CancellationToken? cancellationToken = null)
        {
            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<Message>, Message>(
                                    HttpMethod.Get,
                                    new Uri(String.Format("{0}/{1}", TEAMS_MESSAGES_API_PATH, Uri.EscapeDataString(messageId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Gets message detail from Cisco Webex Teams.
        /// </summary>
        /// <param name="message"><see cref="Message"/> that the detail info is be gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<Message> > GetMessageAsync(Message message, CancellationToken? cancellationToken = null)
        {
            return (GetMessageAsync(message.Id, cancellationToken));
        }


        /// <summary>
        /// Deletes message from Cisco Webex Teams.
        /// </summary>
        /// <param name="messageId">Message id to be deleted.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<NoContent> > DeleteMessageAsync(string messageId, CancellationToken? cancellationToken = null)
        {
            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<NoContent>, NoContent>(
                                    HttpMethod.Delete,
                                    new Uri(String.Format("{0}/{1}", TEAMS_MESSAGES_API_PATH, Uri.EscapeDataString(messageId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.NoContent));

            return result;
        }

        /// <summary>
        /// Deletes message from Cisco Webex Teams.
        /// </summary>
        /// <param name="message">Message to be deleted.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task<TeamsResult<NoContent>> DeleteMessageAsync(Message message, CancellationToken? cancellationToken = null)
        {
            return (DeleteMessageAsync(message.Id, cancellationToken));
        }

        #endregion

        #region AttachmentActions APIs

        /// <summary>
        /// Creates an attachment action.
        /// </summary>
        /// <param name="messageId">The Id of the message which contains the attachment.</param>
        /// <param name="inputs">The attachment action's inputs.</param>
        /// <param name="type">The type of action to perform.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<AttachmentAction> > CreateAttachmentActionAsync(string messageId, AttachmentActionInputs inputs, AttachmentActionType type, CancellationToken? cancellationToken = null)
        {
            var attachmentAction = new AttachmentAction();

            attachmentAction.MessageId = messageId;
            attachmentAction.Inputs    = inputs;
            attachmentAction.TypeName  = type.Name;

            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<AttachmentAction>, AttachmentAction>(
                                    HttpMethod.Post,
                                    TEAMS_ATTACHMENT_ACTIONS_API_URI,
                                    null,
                                    attachmentAction,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Creates an attachment action.
        /// </summary>
        /// <param name="message"><see cref="Message"/> which contains the attachment.</param>
        /// <param name="inputs">The attachment action's inputs.</param>
        /// <param name="type">The type of action to perform.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<AttachmentAction> > CreateAttachmentActionAsync(Message message, AttachmentActionInputs inputs, AttachmentActionType type, CancellationToken? cancellationToken = null)
        {
            return CreateAttachmentActionAsync(message.Id, inputs, type, cancellationToken);
        }

        /// <summary>
        /// Gets Attachment action detail.
        /// </summary>
        /// <param name="attachmentActionId">A unique identifier for the attachment action.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task<TeamsResult<AttachmentAction>> GetAttachmentActionAsync(string attachmentActionId, CancellationToken? cancellationToken = null)
        {
            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<AttachmentAction>, AttachmentAction>(
                                    HttpMethod.Get,
                                    new Uri(String.Format("{0}/{1}", TEAMS_ATTACHMENT_ACTIONS_API_PATH, Uri.EscapeDataString(attachmentActionId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Gets Attachment action detail.
        /// </summary>
        /// <param name="attachmentAction"><see cref="AttachmentAction"/> that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<AttachmentAction> > GetAttachmentActionAsync(AttachmentAction attachmentAction, CancellationToken? cancellationToken = null)
        {
            return GetAttachmentActionAsync(attachmentAction.Id, cancellationToken);
        }


        #endregion

        #region Teams APIs

        /// <summary>
        /// Lists teams.
        /// </summary>
        /// <param name="max">Limit the maximum number of teams in the response.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsListResult<TeamList> > ListTeamsAsync(int? max = null, CancellationToken? cancellationToken = null)
        {
            var queryParameters = new NameValueCollection();

            queryParameters.Add("max", max?.ToString());

            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsListResult<TeamList>, TeamList>(
                                    HttpMethod.Get,
                                    TEAMS_TEAMS_API_URI,
                                    queryParameters,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }


        /// <summary>
        /// Creates a team.
        /// </summary>
        /// <param name="name">A user-friendly name for the team.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<Team> > CreateTeamAsync(string name, CancellationToken? cancellationToken = null)
        {
            var team = new Team();

            team.Name = name;

            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<Team>, Team>(
                                    HttpMethod.Post,
                                    TEAMS_TEAMS_API_URI,
                                    null,
                                    team,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }


        /// <summary>
        /// Gets team detail.
        /// </summary>
        /// <param name="teamId">Team id that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<Team> > GetTeamAsync(string teamId, CancellationToken? cancellationToken = null)
        {
            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<Team>, Team>(
                                    HttpMethod.Get,
                                    new Uri(String.Format("{0}/{1}", TEAMS_TEAMS_API_PATH, Uri.EscapeDataString(teamId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Gets team detail.
        /// </summary>
        /// <param name="team"><see cref="Team"/> that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<Team> > GetTeamAsync(Team team, CancellationToken? cancellationToken = null)
        {
            return (GetTeamAsync(team.Id, cancellationToken));
        }


        /// <summary>
        /// Updates team info.
        /// </summary>
        /// <param name="teamId">Team id to be updated.</param>
        /// <param name="name">A user-friendly name for the team.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<Team> > UpdateTeamAsync(string teamId, string name, CancellationToken? cancellationToken = null)
        {
            var team = new Team();

            team.Name = name;

            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<Team>, Team>(
                                    HttpMethod.Put,
                                    new Uri(String.Format("{0}/{1}", TEAMS_TEAMS_API_PATH, Uri.EscapeDataString(teamId))),
                                    null,
                                    team,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Updates team info.
        /// </summary>
        /// <param name="team"><see cref="Team"/> to be updated.</param>
        /// <param name="name">A user-friendly name for the team.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task<TeamsResult<Team>> UpdateTeamAsync(Team team, string name, CancellationToken? cancellationToken = null)
        {
            return (UpdateTeamAsync(team.Id, name, cancellationToken));
        }

        /// <summary>
        /// Deletes a team.
        /// </summary>
        /// <param name="teamId">Team id to be deleted.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<NoContent> > DeleteTeamAsync(string teamId, CancellationToken? cancellationToken = null)
        {
            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<NoContent>, NoContent>(
                                    HttpMethod.Delete,
                                    new Uri(String.Format("{0}/{1}", TEAMS_TEAMS_API_PATH, Uri.EscapeDataString(teamId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.NoContent));

            return result;
        }

        /// <summary>
        /// Deletes a team.
        /// </summary>
        /// <param name="team"><see cref="Team"/> to be deleted.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<NoContent> > DeleteTeamAsync(Team team, CancellationToken? cancellationToken = null)
        {
            return (DeleteTeamAsync(team.Id, cancellationToken));
        }


        #endregion


        #region TeamMemberships APIs

        /// <summary>
        /// Lists team memberships.
        /// </summary>
        /// <param name="teamId">List team memberships for a team, by ID.</param>
        /// <param name="max">Limit the maximum number of items in the response.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsListResult<TeamMembershipList> > ListTeamMembershipsAsync(string teamId, int? max = null, CancellationToken? cancellationToken = null)
        {
            var queryParameters = new NameValueCollection();

            queryParameters.Add("teamId", teamId);
            queryParameters.Add("max",    max?.ToString());

            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsListResult<TeamMembershipList>, TeamMembershipList>(
                                    HttpMethod.Get,
                                    TEAMS_TEAM_MEMBERSHIPS_API_URI,
                                    queryParameters,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Lists team memberships.
        /// </summary>
        /// <param name="team">List team memberships for <see cref="Team"/>.</param>
        /// <param name="max">Limit the maximum number of items in the response.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task<TeamsListResult<TeamMembershipList>> ListTeamMembershipsAsync(Team team, int? max = null, CancellationToken? cancellationToken = null)
        {
            return (ListTeamMembershipsAsync(team.Id, max, cancellationToken));
        }


        /// <summary>
        /// Create a team membership.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="personIdOrEmail">The person ID or Email.</param>
        /// <param name="isModerator">Set to true to make the person a room moderator.</param>
        /// <param name="personIdType"><see cref="PersonIdType"/> for personIdOrEmail parameter.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<TeamMembership> > CreateTeamMembershipAsync(string teamId, string personIdOrEmail, bool? isModerator = null, PersonIdType personIdType = PersonIdType.Detect, CancellationToken? cancellationToken = null)
        {
            var teamMembership = new TeamMembership();

            teamMembership.TeamId = teamId;

            personIdType = DetectPersonIdType(personIdOrEmail, personIdType);

            switch (personIdType)
            {
                case PersonIdType.Email:
                    teamMembership.PersonEmail = personIdOrEmail;
                    break;
                default:
                    teamMembership.PersonId = personIdOrEmail;
                    break;
            }

            teamMembership.IsModerator = isModerator;

            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<TeamMembership>, TeamMembership>(
                                    HttpMethod.Post,
                                    TEAMS_TEAM_MEMBERSHIPS_API_URI,
                                    null,
                                    teamMembership,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Create a team membership.
        /// </summary>
        /// <param name="team">The <see cref="Team"/>.</param>
        /// <param name="person">The <see cref="Person"/>.</param>
        /// <param name="isModerator">Set to true to make the person a room moderator.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<TeamMembership> > CreateTeamMembershipAsync(Team team, Person person, bool? isModerator = null, CancellationToken? cancellationToken = null)
        {
            return (CreateTeamMembershipAsync(team.Id, person.Id, isModerator, PersonIdType.Id, cancellationToken));
        }

        /// <summary>
        /// Create a team membership.
        /// </summary>
        /// <param name="team"><see cref="Team"/>.</param>
        /// <param name="personIdOrEmail">The person ID or Email.</param>
        /// <param name="isModerator">Set to true to make the person a room moderator.</param>
        /// <param name="personIdType"><see cref="PersonIdType"/> for personIdOrEmail parameter.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<TeamMembership> > CreateTeamMembershipAsync(Team team, string personIdOrEmail, bool? isModerator = null, PersonIdType personIdType = PersonIdType.Detect, CancellationToken? cancellationToken = null)
        {
            return (CreateTeamMembershipAsync(team.Id, personIdOrEmail, isModerator, personIdType, cancellationToken));
        }


        /// <summary>
        /// Gets team membership detail.
        /// </summary>
        /// <param name="membershipId">Team Membership id that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<TeamMembership> > GetTeamMembershipAsync(string membershipId, CancellationToken? cancellationToken = null)
        {
            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<TeamMembership>, TeamMembership>(
                                    HttpMethod.Get,
                                    new Uri(String.Format("{0}/{1}", TEAMS_TEAM_MEMBERSHIPS_API_PATH, Uri.EscapeDataString(membershipId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Gets team membership detail.
        /// </summary>
        /// <param name="membeship"><see cref="TeamMembership"/> that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<TeamMembership> > GetTeamMembershipAsync(TeamMembership membeship, CancellationToken? cancellationToken = null)
        {
            return (GetTeamMembershipAsync(membeship.Id, cancellationToken));
        }


        /// <summary>
        /// Updates team membership.
        /// </summary>
        /// <param name="membershipId">Membership id to be updated.</param>
        /// <param name="isModerator">Set to true to make the person a team moderator.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<TeamMembership> > UpdateTeamMembershipAsync(string membershipId, bool isModerator, CancellationToken? cancellationToken = null)
        {
            var membership = new TeamMembership();

            membership.IsModerator = isModerator;

            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<TeamMembership>, TeamMembership>(
                                    HttpMethod.Put,
                                    new Uri(String.Format("{0}/{1}", TEAMS_TEAM_MEMBERSHIPS_API_PATH, Uri.EscapeDataString(membershipId))),
                                    null,
                                    membership,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Updates team membership.
        /// </summary>
        /// <param name="membership"><see cref="TeamMembership"/> to be updated.</param>
        /// <param name="isModerator">Set to true to make the person a team moderator.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<TeamMembership> > UpdateTeamMembershipAsync(TeamMembership membership, bool isModerator, CancellationToken? cancellationToken = null)
        {
            return (UpdateTeamMembershipAsync(membership.Id, isModerator, cancellationToken));
        }


        /// <summary>
        /// Deletes a team membership.
        /// </summary>
        /// <param name="membershipId">Team Membership id to be deleted.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<NoContent> > DeleteTeamMembershipAsync(string membershipId, CancellationToken? cancellationToken = null)
        {
            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<NoContent>, NoContent>(
                                    HttpMethod.Delete,
                                    new Uri(String.Format("{0}/{1}", TEAMS_TEAM_MEMBERSHIPS_API_PATH, Uri.EscapeDataString(membershipId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.NoContent));

            return result;
        }

        /// <summary>
        /// Deletes a team membership.
        /// </summary>
        /// <param name="membership"><see cref="TeamMembership"/> to be deleted.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<NoContent> > DeleteTeamMembershipAsync(TeamMembership membership, CancellationToken? cancellationToken = null)
        {
            return (DeleteTeamMembershipAsync(membership.Id, cancellationToken));
        }

        #endregion


        #region Webhooks APIs

        /// <summary>
        /// Lists webhooks.
        /// </summary>
        /// <param name="max">Limit the maximum number of webhooks in the response.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsListResult<WebhookList> > ListWebhooksAsync(int? max = null, CancellationToken? cancellationToken = null)
        {
            var queryParameters = new NameValueCollection();

            queryParameters.Add("max", max?.ToString());

            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsListResult<WebhookList>, WebhookList>(
                                    HttpMethod.Get,
                                    TEAMS_WEBHOOKS_API_URI,
                                    queryParameters,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }


        /// <summary>
        /// Create a webhook.
        /// </summary>
        /// <param name="name">A user-friendly name for this webhook.</param>
        /// <param name="targetUri">The URL that receives POST requests for each event.</param>
        /// <param name="resource">The resource type for the webhook. Creating a webhook requires 'read' scope on the resource the webhook is for.</param>
        /// <param name="eventType">The event type for the webhook.</param>
        /// <param name="filters">The filter that defines the webhook scope.</param>
        /// <param name="secret">Secret used to generate payload signature.</param>
        /// <param name="secretLength">Secret length that is generated, if the secret parameter is null.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<Webhook> > CreateWebhookAsync(string name, Uri targetUri, EventResource resource, EventType eventType, IEnumerable<EventFilter> filters = null, string secret = null, int secretLength = 64, CancellationToken? cancellationToken = null)
        {
            var webhook = new Webhook();

            webhook.Name          = name;
            webhook.TargetUrl     = targetUri.AbsoluteUri;
            webhook.ResourceName  = resource.Name;
            webhook.EventTypeName = eventType.Name;

            if(filters != null)
            {
                NameValueCollection nvc = new NameValueCollection();

                foreach (var item in filters)
                {
                    if((item is EventFilter.SpaceTypeFilter) && (resource == EventResource.Space))
                    {
                        nvc.Add("type", item.Value);
                    }
                    else
                    {
                        nvc.Add(item.Key, item.Value);
                    }
                }

                webhook.Filter = HttpUtils.BuildQueryParameters(nvc);
            }

            if(secret != null)
            {
                webhook.Secret = secret;
            }
            else if(secretLength > 0)
            {
                webhook.Secret = new String(RAND.GetASCIIChars(secretLength, (CryptoRandom.ASCIICategory.UpperAlphabet | CryptoRandom.ASCIICategory.LowerAlphabet | CryptoRandom.ASCIICategory.Number)));
            }

            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<Webhook>, Webhook>(
                                    HttpMethod.Post,
                                    TEAMS_WEBHOOKS_API_URI,
                                    null,
                                    webhook,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }


        /// <summary>
        /// Gets webhook detail.
        /// </summary>
        /// <param name="webhookId">Webhook id that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<Webhook> > GetWebhookAsync(string webhookId, CancellationToken? cancellationToken = null)
        {
            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<Webhook>, Webhook>(
                                    HttpMethod.Get,
                                    new Uri(String.Format("{0}/{1}", TEAMS_WEBHOOKS_API_PATH, Uri.EscapeDataString(webhookId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Gets webhook detail.
        /// </summary>
        /// <param name="webhook"><see cref="Webhook"/> that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<Webhook> > GetWebhookAsync(Webhook webhook, CancellationToken? cancellationToken = null)
        {
            return (GetWebhookAsync(webhook.Id, cancellationToken));
        }


        /// <summary>
        /// Updates webhook.
        /// </summary>
        /// <param name="webhookId">Webhook id to be updated.</param>
        /// <param name="name">A user-friendly name for this webhook.</param>
        /// <param name="targetUri">The URL that receives POST requests for each event.</param>
        /// <param name="secret">Secret used to generate payload signature.</param>
        /// <param name="secretLength">Secret length that is generated, if the secret parameter is null.</param>
        /// <param name="status"><see cref="WebhookStatus"/> to be updated.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<Webhook> > UpdateWebhookAsync(string webhookId, string name, Uri targetUri, string secret = null, int secretLength = 0, WebhookStatus status = null, CancellationToken? cancellationToken = null)
        {
            var webhook = new Webhook();

            webhook.Name      = name;
            webhook.TargetUrl = targetUri.AbsoluteUri;

            if (secret != null)
            {
                webhook.Secret = secret;
            }
            else if (secretLength > 0)
            {
                webhook.Secret = new String(RAND.GetASCIIChars(secretLength, (CryptoRandom.ASCIICategory.UpperAlphabet | CryptoRandom.ASCIICategory.LowerAlphabet | CryptoRandom.ASCIICategory.Number)));
            }

            webhook.StatusName = status?.Name;

            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<Webhook>, Webhook>(
                                    HttpMethod.Put,
                                    new Uri(String.Format("{0}/{1}", TEAMS_WEBHOOKS_API_PATH, Uri.EscapeDataString(webhookId))),
                                    null,
                                    webhook,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Updates webhook.
        /// </summary>
        /// <param name="webhook">Webhook id to be updated.</param>
        /// <param name="name">A user-friendly name for this webhook.</param>
        /// <param name="targetUri">The URL that receives POST requests for each event.</param>
        /// <param name="secret">Secret used to generate payload signature.</param>
        /// <param name="secretLength">Secret length that is generated, if the secret parameter is null.</param>
        /// <param name="status"><see cref="WebhookStatus"/> to be updated.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<Webhook> > UpdateWebhookAsync(Webhook webhook, string name, Uri targetUri, string secret = null, int secretLength = 0, WebhookStatus status = null, CancellationToken? cancellationToken = null)
        {
            return (UpdateWebhookAsync(webhook.Id, name, targetUri, secret, secretLength, status, cancellationToken));
        }


        /// <summary>
        /// Updates webhook.
        /// </summary>
        /// <param name="webhookId">Webhook id to be updated.</param>
        /// <param name="name">A user-friendly name for this webhook.</param>
        /// <param name="targetUri">The URL that receives POST requests for each event.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<Webhook> > UpdateWebhookAsync(string webhookId, string name, Uri targetUri, CancellationToken? cancellationToken = null)
        {
            var webhook = new Webhook();

            webhook.Name      = name;
            webhook.TargetUrl = targetUri.AbsoluteUri;

            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<Webhook>, Webhook>(
                                    HttpMethod.Put,
                                    new Uri(String.Format("{0}/{1}", TEAMS_WEBHOOKS_API_PATH, Uri.EscapeDataString(webhookId))),
                                    null,
                                    webhook,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Updates webhook.
        /// </summary>
        /// <param name="webhook"><see cref="Webhook"/> to be updated.</param>
        /// <param name="name">A user-friendly name for this webhook.</param>
        /// <param name="targetUri">The URL that receives POST requests for each event.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<Webhook> > UpdateWebhookAsync(Webhook webhook, string name, Uri targetUri, CancellationToken? cancellationToken = null)
        {
            return (UpdateWebhookAsync(webhook.Id, name, targetUri, cancellationToken));
        }


        /// <summary>
        /// Deletes a webhook.
        /// </summary>
        /// <param name="webhookId">Webhook id to be deleted.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<NoContent> > DeleteWebhookAsync(string webhookId, CancellationToken? cancellationToken = null)
        {
            var result = await this.teamsHttpClient.RequestJsonAsync<TeamsResult<NoContent>, NoContent>(
                                    HttpMethod.Delete,
                                    new Uri(String.Format("{0}/{1}", TEAMS_WEBHOOKS_API_PATH, Uri.EscapeDataString(webhookId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.NoContent));

            return result;
        }

        /// <summary>
        /// Deletes a webhook.
        /// </summary>
        /// <param name="webhook"><see cref="Webhook"/> to be deleted.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<NoContent> > DeleteWebhookAsync(Webhook webhook, CancellationToken? cancellationToken = null)
        {
            return (DeleteWebhookAsync(webhook.Id, cancellationToken));
        }


        /// <summary>
        /// Activate a webhook.
        /// </summary>
        /// <param name="webhook"><see cref="Webhook"/> to be activated.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public Task< TeamsResult<Webhook> > ActivateWebhookAsync(Webhook webhook, CancellationToken? cancellationToken = null)
        {
            return (UpdateWebhookAsync(webhook.Id, webhook.Name, webhook.TargetUri, webhook.Secret, 0, WebhookStatus.Active, cancellationToken));
        }

        #endregion


        #region Files APIs

        /// <summary>
        /// Gets file info.
        /// </summary>
        /// <param name="fileUri">Uri of the file.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<TeamsFileInfo> > GetFileInfoAsync(Uri fileUri, CancellationToken? cancellationToken = null)
        {
            var result = await this.teamsHttpClient.RequestFileInfoAsync<TeamsResult<TeamsFileInfo>, TeamsFileInfo>(
                                    HttpMethod.Head,
                                    fileUri,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Copies file data to the stream.
        /// </summary>
        /// <param name="fileUri">Uri of the file.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <param name="stream">The stream to where the file data will be copied.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<TeamsFileInfo> > CopyFileDataToStreamAsync(Uri fileUri, Stream stream, CancellationToken? cancellationToken = null)
        {
            var result = await this.teamsHttpClient.RequestCopyFileDataAsync<TeamsResult<TeamsFileInfo>, TeamsFileInfo>(
                                    HttpMethod.Get,
                                    fileUri,
                                    null,
                                    stream,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Gets file data.
        /// The file data is copied to <see cref="MemoryStream"/> internally.
        /// This will cause a high memory consumption depending on file size.
        /// If the file size is too large, please consider to use <see cref="CopyFileDataToStreamAsync(Uri, Stream, CancellationToken?)"/>.
        /// </summary>
        /// <param name="fileUri">Uri of the file.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsResult<TeamsFileData> > GetFileDataAsync(Uri fileUri, CancellationToken? cancellationToken = null)
        {
            var result = await this.teamsHttpClient.RequestFileInfoAsync<TeamsResult<TeamsFileData>, TeamsFileData>(
                                    HttpMethod.Get,
                                    fileUri,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        #endregion




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
                    using (this.lockForMyOwnPersonInfo)
                    using (this.teamsHttpClient)
                    {
                        // disposed.
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~TeamsAPIClient() {
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
