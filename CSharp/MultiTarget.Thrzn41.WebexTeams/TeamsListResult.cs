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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Thrzn41.WebexTeams
{

    /// <summary>
    /// The result of Cisco API list request.
    /// </summary>
    /// <typeparam name="TTeamsObject">Teams Object that is returned on API request.</typeparam>
    public class TeamsListResult<TTeamsObject> : TeamsResult<TTeamsObject>
        where TTeamsObject : TeamsObject, new()
    {


        /// <summary>
        /// Creates TeamsListResult.
        /// </summary>
        public TeamsListResult()
        {
            this.ListTransactionId = Guid.NewGuid();
        }


        /// <summary>
        /// Transaction id for this list.
        /// </summary>
        public Guid ListTransactionId { get; internal set; }


        /// <summary>
        /// <see cref="TeamsHttpClient"/> to get next result.
        /// </summary>
        internal TeamsHttpClient TeamsHttpClient { get; set; }

        /// <summary>
        /// Uri to get next result;
        /// </summary>
        internal Uri NextUri { get; set; }

        /// <summary>
        /// Indicates the next result exists or not.
        /// </summary>
        public bool HasNext
        {
            get
            {
                return (this.TeamsHttpClient != null && this.NextUri != null);
            }
        }




        /// <summary>
        /// Lists result.
        /// </summary>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="TeamsListResult{TTeamsObject}"/> to get result.</returns>
        public async Task< TeamsListResult<TTeamsObject> > ListNextAsync(CancellationToken? cancellationToken = null)
        {
            TeamsListResult<TTeamsObject> result = null;

            if (this.HasNext)
            {
                result = await this.TeamsHttpClient.RequestJsonAsync<TeamsListResult<TTeamsObject>, TTeamsObject>(
                                    HttpMethod.Get,
                                    this.NextUri,
                                    null,
                                    null,
                                    cancellationToken);

                result.ListTransactionId = this.ListTransactionId;
            }
            else
            {
                result = new TeamsListResult<TTeamsObject>();

                result.Data = new TTeamsObject();
                result.Data.HasValues = false;
            }

            return result;
        }


        /// <summary>
        /// Gets enumerator to iterate <see cref="TeamsListResult{TTeamsObject}"/>.
        /// </summary>
        /// <returns><see cref="TeamsListResult{TTeamsObject}"/> to iterate <see cref="TeamsListResult{TTeamsObject}"/>.</returns>
        public TeamsListResultEnumerator<TTeamsObject> GetListResultEnumerator()
        {
            return new TeamsListResultEnumerator<TTeamsObject>(this);
        }

        /// <summary>
        /// Gets enumerator to iterate <see cref="TeamsListResult{TTeamsObject}"/>.
        /// </summary>
        /// <param name="retry">Retry will be tried by this instance if needed.</param>
        /// <param name="retryNotificationFunc">Function to be notified on retry.</param>
        /// <returns><see cref="TeamsListResult{TTeamsObject}"/> to iterate <see cref="TeamsListResult{TTeamsObject}"/>.</returns>
        [Obsolete("Please use TeamsRetryHandler or TeamsRetryOnErrorHandler instead. These handlers can be set on creating a TeamsAPIClient instance and handle retry automatically.")]
        public TeamsListResultEnumerator<TTeamsObject> GetListResultEnumerator(TeamsRetry retry, Func<TeamsListResult<TTeamsObject>, int, bool> retryNotificationFunc = null)
        {
            return new TeamsListResultEnumerator<TTeamsObject>(this, retry, retryNotificationFunc);
        }

    }

}
