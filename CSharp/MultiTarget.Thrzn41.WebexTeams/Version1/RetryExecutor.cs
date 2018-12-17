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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Thrzn41.WebexTeams.Version1
{

    /// <summary>
    /// Retry executor to retry requests.
    /// </summary>
    [Obsolete("Please use TeamsRetryHandler or TeamsRetryOnErrorHandler instead. These handlers can be set on creating a TeamsAPIClient instance and handle retry automatically.")]
    public class RetryExecutor : TeamsRetry
    {

        /// <summary>
        /// This executor will retry once at most, if needed.
        /// This is recomended, because it will be something wrong with Teams API service, if twice or more retry-after are returned.
        /// </summary>
        public static readonly RetryExecutor One = new RetryExecutor(1);

        /// <summary>
        /// This executor will retry twice at most, if needed.
        /// This is NOT recomended, because it will be something wrong with Teams API service, if twice or more retry-after are returned.
        /// </summary>
        public static readonly RetryExecutor Two = new RetryExecutor(2);

        /// <summary>
        /// This executor will retry twice at most, if needed.
        /// This is NOT recomended, because it will be something wrong with Teams API service, if twice or more retry-after are returned.
        /// </summary>
        public static readonly RetryExecutor Three = new RetryExecutor(3);

        /// <summary>
        /// This executor will retry twice at most, if needed.
        /// This is NOT recomended, because it will be something wrong with Teams API service, if twice or more retry-after are returned.
        /// </summary>
        public static readonly RetryExecutor Four = new RetryExecutor(4);




        /// <summary>
        /// Creates instance for retry.
        /// If Retry-After is requested,
        /// the retry will be sent after (retry-after + (retry-after * weight) + buffer).
        /// </summary>
        /// <param name="retryMax">Max retry count.</param>
        /// <param name="buffer">Buffer duration for retry.</param>
        /// <param name="weight">Weight value for retry.</param>
        public RetryExecutor(int retryMax, TimeSpan buffer, double weight = 0.0d)
            : base(retryMax, buffer, weight)
        {
        }

        /// <summary>
        /// Creates instance for retry.
        /// If Retry-After is requested,
        /// the retry will be sent after (retry-after + 250ms).
        /// </summary>
        public RetryExecutor()
            : base()
        {
        }

        /// <summary>
        /// Creates instance for retry.
        /// If Retry-After is requested,
        /// the retry will be sent after (retry-after + 250ms).
        /// </summary>
        /// <param name="retryMax">Max retry count.</param>
        public RetryExecutor(int retryMax)
            : base(retryMax)
        {
        }


        /// <summary>
        /// Requests with retry.
        /// If you want to get notification on each retry, you can use notificationFunc function.
        /// <see cref="TeamsResult{TTeamsObject}"/> and retry counter will be notified to the function.
        /// You should retrun true, if you want to retry, otherwize the retry will be cancelled.
        /// </summary>
        /// <typeparam name="TTeamsData">Type of TeamsData to be returned.</typeparam>
        /// <param name="teamsRequestFunc">A function to be requested with retry.</param>
        /// <param name="notificationFunc">A function to be notified when a retry is trying.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> that represents result of API request.</returns>
        public Task< TeamsResult<TTeamsData> > RequestAsync<TTeamsData>(Func< Task< TeamsResult<TTeamsData> > > teamsRequestFunc, Func<TeamsResult<TTeamsData>, int, bool> notificationFunc = null, CancellationToken? cancellationToken = null)
            where TTeamsData : TeamsData, new()
        {
            return (this.requestAsync<TeamsResult<TTeamsData>, TTeamsData>(teamsRequestFunc, notificationFunc, cancellationToken));
        }

        /// <summary>
        /// Creates with retry.
        /// If you want to get notification on each retry, you can use notificationFunc function.
        /// <see cref="TeamsResult{TTeamsObject}"/> and retry counter will be notified to the function.
        /// You should retrun true, if you want to retry, otherwize the retry will be cancelled.
        /// </summary>
        /// <typeparam name="TTeamsData">Type of TeamsData to be returned.</typeparam>
        /// <param name="teamsRequestFunc">A function to be requested with retry.</param>
        /// <param name="notificationFunc">A function to be notified when a retry is trying.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> that represents result of API request.</returns>
        public Task< TeamsResult<TTeamsData> > CreateAsync<TTeamsData>(Func<Task<TeamsResult<TTeamsData>>> teamsRequestFunc, Func<TeamsResult<TTeamsData>, int, bool> notificationFunc = null, CancellationToken? cancellationToken = null)
            where TTeamsData : TeamsData, new()
        {
            return (this.requestAsync<TeamsResult<TTeamsData>, TTeamsData>(teamsRequestFunc, notificationFunc, cancellationToken));
        }

        /// <summary>
        /// Gets with retry.
        /// If you want to get notification on each retry, you can use notificationFunc function.
        /// <see cref="TeamsResult{TTeamsObject}"/> and retry counter will be notified to the function.
        /// You should retrun true, if you want to retry, otherwize the retry will be cancelled.
        /// </summary>
        /// <typeparam name="TTeamsData">Type of TeamsData to be returned.</typeparam>
        /// <param name="teamsRequestFunc">A function to be requested with retry.</param>
        /// <param name="notificationFunc">A function to be notified when a retry is trying.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> that represents result of API request.</returns>
        public Task< TeamsResult<TTeamsData> > GetAsync<TTeamsData>(Func<Task<TeamsResult<TTeamsData>>> teamsRequestFunc, Func<TeamsResult<TTeamsData>, int, bool> notificationFunc = null, CancellationToken? cancellationToken = null)
            where TTeamsData : TeamsData, new()
        {
            return (this.requestAsync<TeamsResult<TTeamsData>, TTeamsData>(teamsRequestFunc, notificationFunc, cancellationToken));
        }

        /// <summary>
        /// Updates with retry.
        /// If you want to get notification on each retry, you can use notificationFunc function.
        /// <see cref="TeamsResult{TTeamsObject}"/> and retry counter will be notified to the function.
        /// You should retrun true, if you want to retry, otherwize the retry will be cancelled.
        /// </summary>
        /// <typeparam name="TTeamsData">Type of TeamsData to be returned.</typeparam>
        /// <param name="teamsRequestFunc">A function to be requested with retry.</param>
        /// <param name="notificationFunc">A function to be notified when a retry is trying.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> that represents result of API request.</returns>
        public Task< TeamsResult<TTeamsData> > UpdateAsync<TTeamsData>(Func<Task<TeamsResult<TTeamsData>>> teamsRequestFunc, Func<TeamsResult<TTeamsData>, int, bool> notificationFunc = null, CancellationToken? cancellationToken = null)
            where TTeamsData : TeamsData, new()
        {
            return (this.requestAsync<TeamsResult<TTeamsData>, TTeamsData>(teamsRequestFunc, notificationFunc, cancellationToken));
        }


        /// <summary>
        /// Lists with retry.
        /// If you want to get notification on each retry, you can use notificationFunc function.
        /// <see cref="TeamsResult{TTeamsObject}"/> and retry counter will be notified to the function.
        /// You should retrun true, if you want to retry, otherwize the retry will be cancelled.
        /// </summary>
        /// <typeparam name="TTeamsListData">Type of TeamsData to be returned.</typeparam>
        /// <param name="teamsRequestFunc">A function to be requested with retry.</param>
        /// <param name="notificationFunc">A function to be notified when a retry is trying.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used cancellation.</param>
        /// <returns><see cref="TeamsListData{TTeamsObject}"/> that represents result of API request.</returns>
        public Task< TeamsListResult<TTeamsListData> > ListAsync<TTeamsListData>(Func< Task< TeamsListResult<TTeamsListData> > > teamsRequestFunc, Func<TeamsListResult<TTeamsListData>, int, bool> notificationFunc = null, CancellationToken? cancellationToken = null)
            where TTeamsListData : TeamsData, new()
        {
            return (this.requestAsync<TeamsListResult<TTeamsListData>, TTeamsListData>(teamsRequestFunc, notificationFunc, cancellationToken));
        }


        /// <summary>
        /// Deletes with retry.
        /// If you want to get notification on each retry, you can use notificationFunc function.
        /// <see cref="TeamsResult{TTeamsObject}"/> and retry counter will be notified to the function.
        /// You should retrun true, if you want to retry, otherwize the retry will be cancelled.
        /// </summary>
        /// <param name="teamsRequestFunc">A function to be requested with retry.</param>
        /// <param name="notificationFunc">A function to be notified when a retry is trying.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> that represents result of API request.</returns>
        public Task< TeamsResult<NoContent> > DeleteAsync(Func< Task< TeamsResult<NoContent> > > teamsRequestFunc, Func<TeamsResult<NoContent>, int, bool> notificationFunc = null, CancellationToken? cancellationToken = null)
        {
            return (this.requestAsync<TeamsResult<NoContent>, NoContent>(teamsRequestFunc, notificationFunc, cancellationToken));
        }


    }

}
