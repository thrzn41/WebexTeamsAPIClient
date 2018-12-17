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

namespace Thrzn41.WebexTeams
{

    /// <summary>
    /// Class to retry requests.
    /// </summary>
    public abstract class TeamsRetry
    {

        /// <summary>
        /// Default buffer delay.
        /// </summary>
        protected static readonly TimeSpan DEFAULT_BUFFER = TimeSpan.FromMilliseconds(250.0d);


        /// <summary>
        /// Max trial count.
        /// </summary>
        private readonly int trials;

        /// <summary>
        /// Buffer for retry.
        /// </summary>
        private readonly double buffer;

        /// <summary>
        /// Weight to adjust retry.
        /// </summary>
        private readonly double weight;

        /// <summary>
        /// Time to retry on error.
        /// </summary>
        private readonly TimeSpan? timeToRetryOnError;


        /// <summary>
        /// Creates instance for retry.
        /// If Retry-After is requested,
        /// the retry will be sent after (retry-after + (retry-after * weight) + buffer).
        /// </summary>
        /// <param name="retryMax">Max retry count.</param>
        /// <param name="buffer">Buffer duration for retry.</param>
        /// <param name="weight">Weight value for retry.</param>
        /// <param name="timeToRetryOnError">Time to retry on error.</param>
        public TeamsRetry(int retryMax, TimeSpan buffer, double weight = 0.0d, TimeSpan? timeToRetryOnError = null)
        {
            this.trials = Math.Max( (retryMax + 1), 1);
            this.buffer = buffer.TotalMilliseconds;
            this.weight = weight;

            this.timeToRetryOnError = timeToRetryOnError;
        }

        /// <summary>
        /// Creates instance for retry.
        /// If Retry-After is requested,
        /// the retry will be sent after (retry-after + 250ms).
        /// </summary>
        public TeamsRetry()
            : this(1, DEFAULT_BUFFER, 0.0f, null)
        {
        }

        /// <summary>
        /// Creates instance for retry.
        /// If Retry-After is requested,
        /// the retry will be sent after (retry-after + 250ms).
        /// </summary>
        /// <param name="retryMax">Max retry count.</param>
        public TeamsRetry(int retryMax)
            : this(retryMax, DEFAULT_BUFFER, 0.0f, null)
        {
        }


        /// <summary>
        /// Checks if retry is needed or not.
        /// </summary>
        /// <param name="result"><see cref="TeamsResultInfo"/> to be checked.</param>
        /// <returns>true if retry is needed, otherwise false.</returns>
        private bool checkRetry(TeamsResultInfo result)
        {
            int statusCode = (int)result.HttpStatusCode;

            TimeSpan? timeToRetry = null;

            if (result.HasRetryAfter && result.RetryAfter.Delta.HasValue)
            {
                var delta = result.RetryAfter.Delta.Value;

                timeToRetry = TimeSpan.FromMilliseconds((delta.TotalMilliseconds + (delta.TotalMilliseconds * this.weight) + this.buffer));
            }

            if(statusCode == 429 && timeToRetry.HasValue)
            {
                result.TimeToRetry = timeToRetry;
            }

            if( this.timeToRetryOnError != null && !result.HasTimeToRetry && (statusCode == 429 || statusCode == 500 || (statusCode >= 502 && statusCode <= 504)))
            {
                if (timeToRetry.HasValue)
                {
                    result.TimeToRetry = timeToRetry;
                }
                else
                {
                    result.TimeToRetry = this.timeToRetryOnError;
                }
            }

            return result.HasTimeToRetry;
        }

        /// <summary>
        /// Checks if retry is needed or not.
        /// </summary>
        /// <param name="result"><see cref="TeamsResultInfo"/> to be checked.</param>
        /// <param name="notificationFunc">A function to be notified when a retry is trying.</param>
        /// <param name="count">Trial count.</param>
        /// <returns>true if retry is needed, otherwise false.</returns>
        private bool checkRetry(TeamsResultInfo result, Func<TeamsResultInfo, int, bool> notificationFunc, int count)
        {
            return (checkRetry(result) && ((notificationFunc == null) || notificationFunc(result, count)));
        }

        /// <summary>
        /// Checks if retry is needed or not.
        /// </summary>
        /// <typeparam name="TTeamsResult">Type of TeamsResult to be returned.</typeparam>
        /// <typeparam name="TTeamsObject">Type of TeamsObject to be returned.</typeparam>
        /// <param name="result">The result to be checked.</param>
        /// <param name="notificationFunc">A function to be notified when a retry is trying.</param>
        /// <param name="count">Trial count.</param>
        /// <returns>true if retry is needed, otherwise false.</returns>
        private bool checkRetry<TTeamsResult, TTeamsObject>(TTeamsResult result, Func<TTeamsResult, int, bool> notificationFunc, int count)
            where TTeamsResult : TeamsResult<TTeamsObject>, new()
            where TTeamsObject : TeamsObject, new()
        {
            return (checkRetry(result) && ((notificationFunc == null) || notificationFunc(result, count)));
        }

        /// <summary>
        /// Requests with retry.
        /// If you want to get notification on each retry, you can use notificationFunc function.
        /// <see cref="TeamsResult{TTeamsObject}"/> and retry counter will be notified to the function.
        /// You should retrun true, if you want to retry, otherwize the retry will be cancelled.
        /// </summary>
        /// <typeparam name="TTeamsResult">Type of TeamsResult to be returned.</typeparam>
        /// <typeparam name="TTeamsObject">Type of TeamsObject to be returned.</typeparam>
        /// <param name="teamsRequestFunc">A function to be requested with retry.</param>
        /// <param name="notificationFunc">A function to be notified when a retry is trying.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> that represents result of API request.</returns>
        internal async Task<TTeamsResult> requestAsync<TTeamsResult, TTeamsObject>(Func< Task<TTeamsResult> > teamsRequestFunc, Func<TeamsResultInfo, int, bool> notificationFunc = null, CancellationToken? cancellationToken = null)
            where TTeamsResult : TeamsResult<TTeamsObject>, new()
            where TTeamsObject : TeamsObject, new()
        {
            TTeamsResult result = null;
            Guid guid = Guid.Empty;

            for (int i = 0; i < this.trials; i++)
            {
                if (result != null)
                {
                    if (guid != Guid.Empty)
                    {
                        result.TransactionId = guid;
                    }
                    else
                    {
                        guid = result.TransactionId;
                    }

                    if ( checkRetry(result, notificationFunc, i) )
                    {
                        await DelayAsync(result.TimeToRetry.Value, cancellationToken);
                    }
                    else
                    {
                        break;
                    }
                }

                result = await teamsRequestFunc();
            }

            return result;
        }

        /// <summary>
        /// Requests with retry.
        /// If you want to get notification on each retry, you can use notificationFunc function.
        /// <see cref="TeamsResult{TTeamsObject}"/> and retry counter will be notified to the function.
        /// You should retrun true, if you want to retry, otherwize the retry will be cancelled.
        /// </summary>
        /// <typeparam name="TTeamsResult">Type of TeamsResult to be returned.</typeparam>
        /// <typeparam name="TTeamsObject">Type of TeamsObject to be returned.</typeparam>
        /// <param name="teamsRequestFunc">A function to be requested with retry.</param>
        /// <param name="notificationFunc">A function to be notified when a retry is trying.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used cancellation.</param>
        /// <returns><see cref="TeamsResult{TTeamsObject}"/> that represents result of API request.</returns>
        internal async Task<TTeamsResult> requestAsync<TTeamsResult, TTeamsObject>(Func< Task<TTeamsResult> > teamsRequestFunc, Func<TTeamsResult, int, bool> notificationFunc = null, CancellationToken? cancellationToken = null)
            where TTeamsResult : TeamsResult<TTeamsObject>, new()
            where TTeamsObject : TeamsObject, new()
        {
            TTeamsResult result = null;
            Guid         guid   = Guid.Empty;

            for(int i = 0; i < this.trials; i++)
            {
                if(result != null)
                {
                    if(guid != Guid.Empty)
                    {
                        result.TransactionId = guid;
                    }
                    else
                    {
                        guid = result.TransactionId;
                    }

                    if ( checkRetry<TTeamsResult, TTeamsObject>(result, notificationFunc, i) )
                    {
                        await DelayAsync(result.TimeToRetry.Value, cancellationToken);
                    }
                    else
                    {
                        break;
                    }
                }

                result = await teamsRequestFunc();
            }

            return result;
        }


        /// <summary>
        /// Delay before retry.
        /// </summary>
        /// <param name="result"><see cref="TeamsRequestInfo"/> to be delayed.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used cancellation.</param>
        /// <returns><see cref="Task"/> for result.</returns>
        public static Task DelayAsync(TeamsResultInfo result, CancellationToken? cancellationToken = null)
        {
            Task task;

            if(result != null && result.HasTimeToRetry)
            {
                task = DelayAsync(result.TimeToRetry.Value, cancellationToken);
            }
            else
            {
#if (DOTNETFRAMEWORK4_5_2)
                task = DelayAsync(TimeSpan.FromTicks(0L), cancellationToken);
#else
                task = Task.CompletedTask;
#endif
            }

            return task;
        }

        /// <summary>
        /// Delay before retry.
        /// </summary>
        /// <param name="delta">Duration for retry.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used cancellation.</param>
        /// <returns><see cref="Task"/> for result.</returns>
        public static async Task DelayAsync(TimeSpan delta, CancellationToken? cancellationToken = null)
        {
            if(delta.Ticks > 0L)
            {
                if(cancellationToken.HasValue)
                {
                    await Task.Delay(delta, cancellationToken.Value);
                }
                else
                {
                    await Task.Delay(delta);
                }
            }
        }


    }

}
