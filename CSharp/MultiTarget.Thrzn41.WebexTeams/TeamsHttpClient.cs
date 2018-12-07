﻿/* 
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
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Thrzn41.Util;

namespace Thrzn41.WebexTeams
{

    /// <summary>
    /// HttpClient for Cisco Webex Teams API.
    /// </summary>
    public class TeamsHttpClient : IDisposable
    {


        /// <summary>
        /// Reusable Http request.
        /// </summary>
        private abstract class ReusableHttpRequest
        {

            /// <summary>
            /// Http method.
            /// </summary>
            private HttpMethod method;

            /// <summary>
            /// Request uri.
            /// </summary>
            private Uri uri;

            /// <summary>
            /// Accept charset list.
            /// </summary>
            private List<StringWithQualityHeaderValue> acceptCharsets;

            /// <summary>
            /// Accept list.
            /// </summary>
            private List<MediaTypeWithQualityHeaderValue> accepts;

            /// <summary>
            /// Authentication info.
            /// </summary>
            public AuthenticationHeaderValue Authentication { get; set; }




            /// <summary>
            /// Is ready to reuse.
            /// </summary>
            public bool IsReadyToReuse { get; protected set; }




            /// <summary>
            /// Creates Reusable http request.
            /// </summary>
            /// <param name="method">Http method.</param>
            /// <param name="uri">Request uri.</param>
            public ReusableHttpRequest(HttpMethod method, Uri uri)
            {
                this.method  = method;
                this.uri     = uri;

                this.acceptCharsets = new List<StringWithQualityHeaderValue>();
                this.accepts        = new List<MediaTypeWithQualityHeaderValue>();

                this.Authentication = null;

                this.IsReadyToReuse = false;
            }


            /// <summary>
            /// Adds Accept charset item.
            /// </summary>
            /// <param name="item">Item to be added.</param>
            public void AddAcceptCharset(StringWithQualityHeaderValue item)
            {
                this.acceptCharsets.Add(item);
            }

            /// <summary>
            /// Adds Accept item.
            /// </summary>
            /// <param name="item">Item to be added.</param>
            public void AddAccept(MediaTypeWithQualityHeaderValue item)
            {
                this.accepts.Add(item);
            }


            /// <summary>
            /// Creates <see cref="HttpRequestMessage"/>.
            /// </summary>
            /// <returns><see cref="HttpRequestMessage"/>.</returns>
            protected HttpRequestMessage CreateHttpRequestMessage()
            {
                var request = new HttpRequestMessage(this.method, this.uri);

                var headers = request.Headers;

                foreach (var item in this.acceptCharsets)
                {
                    headers.AcceptCharset.Add(item);
                }

                foreach (var item in this.accepts)
                {
                    headers.Accept.Add(item);
                }

                if(this.Authentication != null)
                {
                    headers.Authorization = this.Authentication;
                }

                return request;
            }


            /// <summary>
            /// Create <see cref="HttpRequestMessage"/>.
            /// </summary>
            /// <returns><see cref="HttpRequestMessage"/> to be used to request.</returns>
            public abstract HttpRequestMessage CreateRequest();

            /// <summary>
            /// Create <see cref="HttpRequestMessage"/>.
            /// </summary>
            /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used cancellation.</param>
            /// <returns><see cref="HttpRequestMessage"/> to be used to request.</returns>
            public abstract Task<HttpRequestMessage> CreateRequestAsync(CancellationToken? cancellationToken = null);
        }

        /// <summary>
        /// Reusable string http request.
        /// </summary>
        private class ReusableStringHttpRequest : ReusableHttpRequest
        {

            /// <summary>
            /// Request body text.
            /// </summary>
            private string text;

            /// <summary>
            /// Encoding of request body.
            /// </summary>
            private Encoding encoding;

            /// <summary>
            /// MediaType of request body.
            /// </summary>
            private string mediaType;


            /// <summary>
            /// Creates reusable string http request.
            /// </summary>
            /// <param name="method">Http method.</param>
            /// <param name="uri">Request uri.</param>
            /// <param name="text">Request body text.</param>
            /// <param name="encoding">Encoding of request body.</param>
            /// <param name="mediaType">MediaType of request body.</param>
            internal ReusableStringHttpRequest(HttpMethod method, Uri uri, string text = null, Encoding encoding = null, string mediaType = null)
                : base(method, uri)
            {
                this.text      = text;
                this.encoding  = encoding;
                this.mediaType = mediaType;

                this.IsReadyToReuse = true;
            }

            /// <summary>
            /// Create <see cref="HttpRequestMessage"/>.
            /// </summary>
            /// <returns><see cref="HttpRequestMessage"/> to be used to request.</returns>
            public override HttpRequestMessage CreateRequest()
            {
                var request = CreateHttpRequestMessage();

                if(this.text != null && this.encoding != null && this.mediaType != null)
                {
                    request.Content = new StringContent(this.text, this.encoding, this.mediaType);
                }
                else if (this.text != null && this.encoding != null)
                {
                    request.Content = new StringContent(this.text, this.encoding);
                }
                else if (this.text != null)
                {
                    request.Content = new StringContent(this.text);
                }

                return request;
            }

            /// <summary>
            /// Create <see cref="HttpRequestMessage"/>.
            /// </summary>
            /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used cancellation.</param>
            /// <returns><see cref="HttpRequestMessage"/> to be used to request.</returns>
            public override Task<HttpRequestMessage> CreateRequestAsync(CancellationToken? cancellationToken = null)
            {
                return Task.FromResult(CreateRequest());
            }
        }

        /// <summary>
        /// Reusable FormData http request.
        /// </summary>
        private class ReusableFormDataHttpRequest : ReusableHttpRequest
        {


            /// <summary>
            /// String data for FormData.
            /// </summary>
            private List< KeyValuePair<string, string> > stringData;


            /// <summary>
            /// Creates reusable string http request.
            /// </summary>
            /// <param name="method">Http method.</param>
            /// <param name="uri">Request uri.</param>
            /// <param name="stringData">String data for FormData.</param>
            internal ReusableFormDataHttpRequest(HttpMethod method, Uri uri, List< KeyValuePair<string, string> > stringData = null)
                : base(method, uri)
            {
                this.stringData = stringData;

                this.IsReadyToReuse = true;
            }

            /// <summary>
            /// Create <see cref="HttpRequestMessage"/>.
            /// </summary>
            /// <returns><see cref="HttpRequestMessage"/> to be used to request.</returns>
            public override HttpRequestMessage CreateRequest()
            {
                var request = CreateHttpRequestMessage();

                if(this.stringData != null)
                {
                    request.Content = new FormUrlEncodedContent(this.stringData);
                }

                return request;
            }

            /// <summary>
            /// Create <see cref="HttpRequestMessage"/>.
            /// </summary>
            /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used cancellation.</param>
            /// <returns><see cref="HttpRequestMessage"/> to be used to request.</returns>
            public override Task<HttpRequestMessage> CreateRequestAsync(CancellationToken? cancellationToken = null)
            {
                return Task.FromResult(CreateRequest());
            }
        }

        /// <summary>
        /// Reusable MultipartFormData http request.
        /// </summary>
        private class ReusableMultipartFormDataHttpRequest : ReusableHttpRequest
        {


            /// <summary>
            /// String data for FormData.
            /// </summary>
            private NameValueCollection stringData;

            /// <summary>
            /// Teams file data.
            /// </summary>
            private TeamsFileData fileData;

            /// <summary>
            /// Indicates reuse is required or not.
            /// </summary>
            private bool isReuseRequired;

            /// <summary>
            /// Byte data of file.
            /// </summary>
            private byte[] byteData;


            /// <summary>
            /// Creates reusable string http request.
            /// </summary>
            /// <param name="method">Http method.</param>
            /// <param name="uri">Request uri.</param>
            /// <param name="stringData">String data for FormData.</param>
            /// <param name="fileData">Teams file data.</param>
            /// <param name="isReuseRequired">Indicates reuse is required or not.</param>
            internal ReusableMultipartFormDataHttpRequest(HttpMethod method, Uri uri, NameValueCollection stringData = null, TeamsFileData fileData = null, bool isReuseRequired = false)
                : base(method, uri)
            {
                this.stringData = stringData;
                this.fileData   = fileData;

                this.isReuseRequired = isReuseRequired;

                this.byteData     = null;

                this.IsReadyToReuse = false;
            }

            /// <summary>
            /// Create <see cref="HttpRequestMessage"/>.
            /// </summary>
            /// <returns><see cref="HttpRequestMessage"/> to be used to request.</returns>
            private HttpRequestMessage createRequest()
            {
                var request = CreateHttpRequestMessage();

                var content = new MultipartFormDataContent();


                if (this.stringData != null)
                {
                    foreach (var key in this.stringData.AllKeys)
                    {
                        var values = stringData.GetValues(key);

                        if (values != null)
                        {
                            foreach (var item in values)
                            {
                                if (item != null)
                                {
                                    content.Add(new StringContent(item, ENCODING), key);
                                }
                            }
                        }
                    }
                }

                if (this.fileData != null)
                {
                    HttpContent fileContent;

                    if (this.byteData != null)
                    {
                        fileContent = new ByteArrayContent(byteData);
                    }
                    else
                    {
                        fileContent = new StreamContent(fileData.Stream);
                    }

                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(fileData.MediaTypeName);

                    content.Add(fileContent, "files", fileData.FileName);
                }

                request.Content = content;

                return request;
            }

            /// <summary>
            /// Create <see cref="HttpRequestMessage"/>.
            /// </summary>
            /// <returns><see cref="HttpRequestMessage"/> to be used to request.</returns>
            public override HttpRequestMessage CreateRequest()
            {
                if( !this.IsReadyToReuse && this.isReuseRequired && this.byteData == null )
                {
                    using (var memory = new MemoryStream())
                    {
                        fileData.Stream.CopyTo(memory);

                        byteData = memory.ToArray();
                    }

                    this.IsReadyToReuse = true;
                }

                return createRequest();
            }

            /// <summary>
            /// Create <see cref="HttpRequestMessage"/>.
            /// </summary>
            /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used cancellation.</param>
            /// <returns><see cref="HttpRequestMessage"/> to be used to request.</returns>
            public override async Task<HttpRequestMessage> CreateRequestAsync(CancellationToken? cancellationToken = null)
            {
                if ( !this.IsReadyToReuse && this.isReuseRequired && this.byteData == null )
                {
                    using (var memory = new MemoryStream())
                    {
                        if (cancellationToken.HasValue)
                        {
                            await fileData.Stream.CopyToAsync(memory, 81920, cancellationToken.Value);
                        }
                        else
                        {
                            await fileData.Stream.CopyToAsync(memory);
                        }

                        byteData = memory.ToArray();
                    }

                    this.IsReadyToReuse = true;
                }


                return createRequest();
            }

        }



        /// <summary>
        /// Media type value for application/json.
        /// </summary>
        private const string MEDIA_TYPE_APPLICATION_JSON = "application/json";

        /// <summary>
        /// Media type value for any type.
        /// </summary>
        private const string MEDIA_TYPE_ANY = "*/*";


        /// <summary>
        /// Default encoding in this class.
        /// </summary>
        protected static readonly Encoding ENCODING = UTF8Utils.UTF8_WITHOUT_BOM;


        /// <summary>
        /// Header name of TrackingID.
        /// </summary>
        private const string HEADER_NAME_TRACKING_ID = "TrackingID";

        /// <summary>
        /// Header name of Link.
        /// </summary>
        private const string HEADER_NAME_LINK = "Link";


        /// <summary>
        /// Next link pattern.
        /// </summary>
        private readonly static Regex LINK_NEXT_PATTERN = new Regex(".*<(?<NEXTURI>(https://.*?))>.*?rel=\"next\"", RegexOptions.Compiled, TimeSpan.FromSeconds(60.0f));


        /// <summary>
        /// HttpClient to access Cisco Webex Teams API uris.
        /// </summary>
        private readonly HttpClient httpClientForTeamsAPI;

        /// <summary>
        /// HttpClient to access non-Cisco Webex Teams API uris.
        /// </summary>
        private readonly HttpClient httpClientForNonTeamsAPI;


        /// <summary>
        /// Regex pattern to check if the Uri is Cisco Webex Teams API uris.
        /// </summary>
        private readonly Regex teamsAPIUriPattern;


        /// <summary>
        /// Executor for retry.
        /// </summary>
        private readonly TeamsRetry retryExecutor;

        /// <summary>
        /// Notification func for retry.
        /// </summary>
        private readonly Func<TeamsResultInfo, int, bool> retryNotificationFunc;



        /// <summary>
        /// TeamsHttpClient constructor.
        /// </summary>
        /// <param name="teamsToken">Cisco Webex Teams Token.</param>
        /// <param name="teamsAPIUriPattern">Regex pattern to check if the Uri is Cisco Webex Teams API uris.</param>
        /// <param name="preAuthenticate">true if preAuthenticate is needed.</param>
        /// <param name="retryExecutor">Executor for retry.</param>
        /// <param name="retryNotificationFunc">Notification func for retry.</param>
        internal TeamsHttpClient(string teamsToken, Regex teamsAPIUriPattern, bool preAuthenticate, TeamsRetry retryExecutor, Func<TeamsResultInfo, int, bool> retryNotificationFunc)
        {
            // HttpClient for Cisco Webex Teams API.
            // Teams Token MUST be sent to only Teams API https URL.
            // NEVER SEND Token to other URLs or non-secure http URL.
            this.httpClientForTeamsAPI = new HttpClient(
                    new HttpClientHandler
                    {
                        AllowAutoRedirect = false,
                        PreAuthenticate   = preAuthenticate,
                    },
                    true
                );

            // HttpClient for non-Cisco Webex Teams API URL.
            // An avator image path is well-known case.
            this.httpClientForNonTeamsAPI = new HttpClient(
                    new HttpClientHandler
                    {
                        AllowAutoRedirect = false,
                        PreAuthenticate   = false,
                    },
                    true
                );


            if (teamsToken != null)
            {
                // For Cisco Webex Teams API https path, the token is added.
                this.httpClientForTeamsAPI.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", teamsToken);
            }

            this.teamsAPIUriPattern = teamsAPIUriPattern;

            this.retryExecutor         = retryExecutor;
            this.retryNotificationFunc = retryNotificationFunc;

        }



        /// <summary>
        /// TeamsHttpClient constructor.
        /// </summary>
        /// <param name="teamsToken">Cisco Webex Teams Token.</param>
        /// <param name="teamsAPIUriPattern">Regex pattern to check if the Uri is Cisco Webex Teams API uris.</param>
        /// <param name="preAuthenticate">true if preAuthenticate is needed.</param>
        internal TeamsHttpClient(string teamsToken, Regex teamsAPIUriPattern, bool preAuthenticate)
            : this(teamsToken, teamsAPIUriPattern, preAuthenticate, null, null)
        {
        }

        /// <summary>
        /// TeamsHttpClient constructor.
        /// </summary>
        /// <param name="teamsToken">Cisco Webex Teams Token.</param>
        /// <param name="teamsAPIUriPattern">Regex pattern to check if the Uri is Cisco Webex Teams API uris.</param>
        internal TeamsHttpClient(string teamsToken, Regex teamsAPIUriPattern)
            : this(teamsToken, teamsAPIUriPattern, (teamsToken != null))
        {
        }

        /// <summary>
        /// TeamsHttpClient constructor.
        /// </summary>
        /// <param name="teamsToken">Cisco Webex Teams Token.</param>
        /// <param name="teamsAPIUriPattern">Regex pattern to check if the Uri is Cisco Webex Teams API uris.</param>
        /// <param name="retryExecutor">Executor for retry.</param>
        /// <param name="retryNotificationFunc">Notification func for retry.</param>
        internal TeamsHttpClient(string teamsToken, Regex teamsAPIUriPattern, TeamsRetry retryExecutor, Func<TeamsResultInfo, int, bool> retryNotificationFunc)
            : this(teamsToken, teamsAPIUriPattern, (teamsToken != null), retryExecutor, retryNotificationFunc)
        {
        }




        /// <summary>
        /// Selects HttpClient for uri.
        /// </summary>
        /// <param name="uri">Uri to be requested.</param>
        /// <returns>HttpClient for uri.</returns>
        private HttpClient selectHttpClient(Uri uri)
        {
            var result = this.httpClientForNonTeamsAPI;

            if(this.teamsAPIUriPattern.IsMatch(uri.AbsoluteUri))
            {
                result = this.httpClientForTeamsAPI;
            }

            return result;
        }

        /// <summary>
        /// Requests to Cisco Webex Teams API.
        /// </summary>
        /// <typeparam name="TTeamsResult">Type of TeamsResult to be returned.</typeparam>
        /// <typeparam name="TTeamsObject">Type of TeamsObject to be returned.</typeparam>
        /// <param name="reusableRequest"><see cref="ReusableHttpRequest"/> to be requested.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> that represents result of API request.</returns>
        private async Task<TTeamsResult> requestAsync<TTeamsResult, TTeamsObject>(ReusableHttpRequest reusableRequest, CancellationToken? cancellationToken = null)
            where TTeamsResult : TeamsResult<TTeamsObject>, new()
            where TTeamsObject : TeamsObject, new()
        {
            var result = new TTeamsResult();

            HttpRequestMessage request;

            if(reusableRequest.IsReadyToReuse)
            {
                request = reusableRequest.CreateRequest();
            }
            else
            {
                request = await reusableRequest.CreateRequestAsync(cancellationToken);
            }

            using (request)
            using (request.Content)
            {

                var httpClient = selectHttpClient(request.RequestUri);

                result.RequestInfo = new TeamsRequestInfo(request);

                HttpResponseMessage response;

                if (cancellationToken.HasValue)
                {
                    response = await httpClient.SendAsync(request, cancellationToken.Value);
                }
                else
                {
                    response = await httpClient.SendAsync(request);
                }

                using (response)
                {
                    if (response.StatusCode != System.Net.HttpStatusCode.NoContent && response.Content != null)
                    {
                        using (var content = response.Content)
                        {
                            var contentHeaders = content.Headers;

                            if (contentHeaders.ContentType?.MediaType == MEDIA_TYPE_APPLICATION_JSON)
                            {
                                var body = await content.ReadAsStringAsync();

                                if (!String.IsNullOrEmpty(body))
                                {
                                    result.Data = TeamsObject.FromJsonString<TTeamsObject>(body);
                                }
                            }
                            else
                            {
                                var info = new TTeamsObject() as TeamsFileInfo;

                                if (info != null)
                                {
                                    string fileName = contentHeaders.ContentDisposition?.FileName;

                                    if (fileName != null && fileName.StartsWith("\"") && fileName.EndsWith("\""))
                                    {
                                        fileName = fileName.Substring(1, (fileName.Length - 2));
                                    }

                                    info.FileName      = fileName;
                                    info.MediaTypeName = contentHeaders.ContentType?.MediaType;
                                    info.Size          = contentHeaders.ContentLength;
                                }

                                var data = info as TeamsFileData;

                                if (data != null)
                                {
                                    data.Stream = new MemoryStream();

                                    await content.CopyToAsync(data.Stream);

                                    data.Stream.Position = 0;
                                }

                                result.Data = ((data != null) ? data : info) as TTeamsObject;
                            }
                        }
                    }


                    if (result.Data == null)
                    {
                        result.Data = new TTeamsObject();

                        result.Data.HasValues = false;
                    }


                    result.HttpStatusCode = response.StatusCode;

                    var headers = response.Headers;

                    if (headers.Contains(HEADER_NAME_TRACKING_ID))
                    {
                        foreach (var item in headers.GetValues(HEADER_NAME_TRACKING_ID))
                        {
                            result.TrackingId = item;
                            break;
                        }
                    }

                    result.RetryAfter = headers.RetryAfter;


                    if (result is TeamsListResult<TTeamsObject>)
                    {
                        if (headers.Contains(HEADER_NAME_LINK))
                        {
                            var listResult = result as TeamsListResult<TTeamsObject>;

                            if (listResult != null)
                            {
                                foreach (var item in headers.GetValues(HEADER_NAME_LINK))
                                {
                                    if (!String.IsNullOrEmpty(item))
                                    {
                                        var m = LINK_NEXT_PATTERN.Match(item);

                                        if (m.Success)
                                        {
                                            var g = m.Groups["NEXTURI"];

                                            if (g != null && !String.IsNullOrEmpty(g.Value))
                                            {
                                                listResult.NextUri         = new Uri(g.Value);
                                                listResult.TeamsHttpClient = this;

                                                break;
                                            }
                                        }
                                    }
                                }
                            }

                        }
                    }

                    // Result status is once set based on Http Status code.
                    // The exact criteria differs in each API.
                    // This value will be adjusted in each TeamsAPIClient class.
                    result.IsSuccessStatus = response.IsSuccessStatusCode;
                }
            }

            return result;
        }

        /// <summary>
        /// Requests to Cisco Webex Teams API.
        /// </summary>
        /// <typeparam name="TTeamsResult">Type of TeamsResult to be returned.</typeparam>
        /// <typeparam name="TTeamsObject">Type of TeamsObject to be returned.</typeparam>
        /// <param name="request"><see cref="ReusableHttpRequest"/> to be requested.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> that represents result of API request.</returns>
        private Task<TTeamsResult> RequestAsync<TTeamsResult, TTeamsObject>(ReusableHttpRequest request, CancellationToken? cancellationToken = null)
            where TTeamsResult : TeamsResult<TTeamsObject>, new()
            where TTeamsObject : TeamsObject, new()
        {
            Task<TTeamsResult> result;

            if(this.retryExecutor == null)
            {
                result = requestAsync<TTeamsResult, TTeamsObject>(request, cancellationToken);
            }
            else
            {
                result = retryExecutor.requestAsync<TTeamsResult, TTeamsObject>(
                    () =>
                    {
                        return requestAsync<TTeamsResult, TTeamsObject>(request, cancellationToken);
                    },
                    retryNotificationFunc,
                    cancellationToken);
            }

            return result;
        }

        /// <summary>
        /// Requests to Cisco Webex Teams API.
        /// </summary>
        /// <typeparam name="TTeamsResult">Type of TeamsResult to be returned.</typeparam>
        /// <typeparam name="TTeamsObject">Type of TeamsObject to be returned.</typeparam>
        /// <param name="method"><see cref="HttpMethod"/> to be used on requesting.</param>
        /// <param name="uri">Uri to be requested.</param>
        /// <param name="queryParameters">Query parameter collection.</param>
        /// <param name="objectToBePosted">Object inherited from <see cref="TeamsObject"/> to be sent to Teams API.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> that represents result of API request.</returns>
        public Task<TTeamsResult> RequestJsonAsync<TTeamsResult, TTeamsObject>(HttpMethod method, Uri uri, NameValueCollection queryParameters = null, TeamsObject objectToBePosted = null, CancellationToken? cancellationToken = null)
            where TTeamsResult : TeamsResult<TTeamsObject>, new()
            where TTeamsObject : TeamsObject, new()
        {
            return (RequestAsync<TTeamsResult, TTeamsObject>(CreateJsonRequest(method, uri, queryParameters, objectToBePosted), cancellationToken));
        }

        /// <summary>
        /// Requests to Cisco Webex Teams API with bearer token.
        /// </summary>
        /// <typeparam name="TTeamsResult">Type of TeamsResult to be returned.</typeparam>
        /// <typeparam name="TTeamsObject">Type of TeamsObject to be returned.</typeparam>
        /// <param name="method"><see cref="HttpMethod"/> to be used on requesting.</param>
        /// <param name="uri">Uri to be requested.</param>
        /// <param name="queryParameters">Query parameter collection.</param>
        /// <param name="objectToBePosted">Object inherited from <see cref="TeamsObject"/> to be sent to Teams API.</param>
        /// <param name="token">Bearer token of this request.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> that represents result of API request.</returns>
        public Task<TTeamsResult> RequestJsonWithBearerTokenAsync<TTeamsResult, TTeamsObject>(HttpMethod method, Uri uri, NameValueCollection queryParameters = null, TeamsObject objectToBePosted = null, string token = null, CancellationToken? cancellationToken = null)
            where TTeamsResult : TeamsResult<TTeamsObject>, new()
            where TTeamsObject : TeamsObject, new()
        {
            return (RequestAsync<TTeamsResult, TTeamsObject>(CreateJsonRequestWithBearerToken(method, uri, queryParameters, objectToBePosted, token), cancellationToken));
        }


        /// <summary>
        /// Requests file info/data to Cisco Webex Teams API.
        /// </summary>
        /// <typeparam name="TTeamsResult">Type of TeamsResult to be returned.</typeparam>
        /// <typeparam name="TTeamsFileInfo">Type of TeamsFileInfo to be returned.</typeparam>
        /// <param name="method"><see cref="HttpMethod"/> to be used on requesting.</param>
        /// <param name="uri">Uri to be requested.</param>
        /// <param name="queryParameters">Query parameter collection.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> that represents result of API request.</returns>
        public Task<TTeamsResult> RequestFileInfoAsync<TTeamsResult, TTeamsFileInfo>(HttpMethod method, Uri uri, NameValueCollection queryParameters = null, CancellationToken? cancellationToken = null)
            where TTeamsResult   : TeamsResult<TTeamsFileInfo>, new()
            where TTeamsFileInfo : TeamsFileInfo, new()
        {
            return (RequestAsync<TTeamsResult, TTeamsFileInfo>(CreateFileInfoRequest(method, uri, queryParameters), cancellationToken));
        }

        /// <summary>
        /// Requests MultipartFormData to Cisco Webex Teams API.
        /// </summary>
        /// <typeparam name="TTeamsResult">Type of TeamsResult to be returned.</typeparam>
        /// <typeparam name="TTeamsObject">Type of TeamsObject to be returned.</typeparam>
        /// <param name="method"><see cref="HttpMethod"/> to be used on requesting.</param>
        /// <param name="uri">Uri to be requested.</param>
        /// <param name="queryParameters">Query parameter collection.</param>
        /// <param name="stringData">String data collection.</param>
        /// <param name="fileData">File data list.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> that represents result of API request.</returns>
        public Task<TTeamsResult> RequestMultipartFormDataAsync<TTeamsResult, TTeamsObject>(HttpMethod method, Uri uri, NameValueCollection queryParameters = null, NameValueCollection stringData = null, TeamsFileData fileData = null, CancellationToken? cancellationToken = null)
            where TTeamsResult : TeamsResult<TTeamsObject>, new()
            where TTeamsObject : TeamsObject, new()
        {
            return (RequestAsync<TTeamsResult, TTeamsObject>(CreateMultipartFormDataRequest(method, uri, queryParameters, stringData, fileData), cancellationToken));
        }

        /// <summary>
        /// Requests FormData to Cisco Webex Teams API.
        /// </summary>
        /// <typeparam name="TTeamsResult">Type of TeamsResult to be returned.</typeparam>
        /// <typeparam name="TTeamsObject">Type of TeamsObject to be returned.</typeparam>
        /// <param name="method"><see cref="HttpMethod"/> to be used on requesting.</param>
        /// <param name="uri">Uri to be requested.</param>
        /// <param name="queryParameters">Query parameter collection.</param>
        /// <param name="stringData">String data collection.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> that represents result of API request.</returns>
        public Task<TTeamsResult> RequestFormDataAsync<TTeamsResult, TTeamsObject>(HttpMethod method, Uri uri, NameValueCollection queryParameters = null, NameValueCollection stringData = null, CancellationToken? cancellationToken = null)
            where TTeamsResult : TeamsResult<TTeamsObject>, new()
            where TTeamsObject : TeamsObject, new()
        {
            return (RequestAsync<TTeamsResult, TTeamsObject>(CreateFormDataRequest(method, uri, queryParameters, stringData), cancellationToken));
        }



        /// <summary>
        /// Creates <see cref="ReusableHttpRequest"/> to use for Json request.
        /// </summary>
        /// <param name="method"><see cref="HttpMethod"/> to be used on requesting.</param>
        /// <param name="uri">Uri to be requested.</param>
        /// <param name="queryParameters">Query parameter collection.</param>
        /// <param name="objectToBePosted">Object inherited from <see cref="TeamsObject"/> to be sent to Teams API.</param>
        /// <returns><see cref="ReusableHttpRequest"/> that is created.</returns>
        private ReusableHttpRequest CreateJsonRequest(HttpMethod method, Uri uri, NameValueCollection queryParameters = null, TeamsObject objectToBePosted = null)
        {
            ReusableHttpRequest request;

            if(objectToBePosted != null)
            {
                request = new ReusableStringHttpRequest(
                    method, HttpUtils.BuildUri(uri, queryParameters), 
                    objectToBePosted.ToJsonString(), ENCODING, MEDIA_TYPE_APPLICATION_JSON);
            }
            else
            {
                request = new ReusableStringHttpRequest(method, HttpUtils.BuildUri(uri, queryParameters));
            }

            request.AddAcceptCharset(new StringWithQualityHeaderValue(ENCODING.WebName));
            request.AddAccept(new MediaTypeWithQualityHeaderValue(MEDIA_TYPE_APPLICATION_JSON));

            return request;
        }

        /// <summary>
        /// Creates <see cref="ReusableHttpRequest"/> to use for Json request.
        /// </summary>
        /// <param name="method"><see cref="HttpMethod"/> to be used on requesting.</param>
        /// <param name="uri">Uri to be requested.</param>
        /// <param name="queryParameters">Query parameter collection.</param>
        /// <param name="objectToBePosted">Object inherited from <see cref="TeamsObject"/> to be sent to Teams API.</param>
        /// <param name="token">Bearer token of this request.</param>
        /// <returns><see cref="ReusableHttpRequest"/> that is created.</returns>
        private ReusableHttpRequest CreateJsonRequestWithBearerToken(HttpMethod method, Uri uri, NameValueCollection queryParameters = null, TeamsObject objectToBePosted = null, string token = null)
        {
            var request = CreateJsonRequest(method, uri, queryParameters, objectToBePosted);

            if(token != null)
            {
                request.Authentication = new AuthenticationHeaderValue("Bearer", token);
            }

            return request;
        }


        /// <summary>
        /// Creates <see cref="ReusableHttpRequest"/> to use for File info request.
        /// </summary>
        /// <param name="method"><see cref="HttpMethod"/> to be used on requesting.</param>
        /// <param name="uri">Uri to be requested.</param>
        /// <param name="queryParameters">Query parameter collection.</param>
        /// <returns><see cref="ReusableHttpRequest"/> that is created.</returns>
        private ReusableHttpRequest CreateFileInfoRequest(HttpMethod method, Uri uri, NameValueCollection queryParameters = null)
        {
            var request = new ReusableStringHttpRequest(method, HttpUtils.BuildUri(uri, queryParameters));

            request.AddAcceptCharset(new StringWithQualityHeaderValue(ENCODING.WebName));
            request.AddAccept(new MediaTypeWithQualityHeaderValue(MEDIA_TYPE_APPLICATION_JSON));
            request.AddAccept(new MediaTypeWithQualityHeaderValue(MEDIA_TYPE_ANY));

            return request;
        }


        /// <summary>
        /// Creates <see cref="ReusableHttpRequest"/> to use for MultipartFormData request.
        /// </summary>
        /// <param name="method"><see cref="HttpMethod"/> to be used on requesting.</param>
        /// <param name="uri">Uri to be requested.</param>
        /// <param name="queryParameters">Query parameter collection.</param>
        /// <param name="stringData">String data collection.</param>
        /// <param name="fileData">File data list.</param>
        /// <returns><see cref="ReusableHttpRequest"/> that is created.</returns>
        private ReusableHttpRequest CreateMultipartFormDataRequest(HttpMethod method, Uri uri, NameValueCollection queryParameters = null, NameValueCollection stringData = null, TeamsFileData fileData = null)
        {
            var request = new ReusableMultipartFormDataHttpRequest(method, HttpUtils.BuildUri(uri, queryParameters), stringData, fileData, (this.retryExecutor != null));

            request.AddAcceptCharset(new StringWithQualityHeaderValue(ENCODING.WebName));
            request.AddAccept(new MediaTypeWithQualityHeaderValue(MEDIA_TYPE_APPLICATION_JSON));

            return request;
        }

        /// <summary>
        /// Creates <see cref="ReusableHttpRequest"/> to use for FormData request.
        /// </summary>
        /// <param name="method"><see cref="HttpMethod"/> to be used on requesting.</param>
        /// <param name="uri">Uri to be requested.</param>
        /// <param name="queryParameters">Query parameter collection.</param>
        /// <param name="stringData">String data collection.</param>
        /// <returns><see cref="ReusableHttpRequest"/> that is created.</returns>
        private ReusableHttpRequest CreateFormDataRequest(HttpMethod method, Uri uri, NameValueCollection queryParameters = null, NameValueCollection stringData = null)
        {
            var list = new List<KeyValuePair<string, string>>();

            if (stringData != null)
            {
                foreach (var key in stringData.AllKeys)
                {
                    var values = stringData.GetValues(key);

                    if (values != null)
                    {
                        foreach (var item in values)
                        {
                            if (item != null)
                            {
                                list.Add(new KeyValuePair<string, string>(key, item));
                            }
                        }
                    }
                }
            }

            var request = new ReusableFormDataHttpRequest(method, HttpUtils.BuildUri(uri, queryParameters), list);

            request.AddAcceptCharset(new StringWithQualityHeaderValue(ENCODING.WebName));
            request.AddAccept(new MediaTypeWithQualityHeaderValue(MEDIA_TYPE_APPLICATION_JSON));

            return request;
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
                    using (this.httpClientForTeamsAPI)
                    using (this.httpClientForNonTeamsAPI)
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
        // ~TeamsHttpClient() {
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
