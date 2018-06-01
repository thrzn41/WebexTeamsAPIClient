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
        private static readonly TimeSpan DEFAULT_BUFFER = TimeSpan.FromMilliseconds(250.0d);


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
        /// Creates instance for retry.
        /// If Retry-After is requested,
        /// the retry will be sent after (retry-after + (retry-after * weight) + buffer).
        /// </summary>
        /// <param name="retryMax">Max retry count.</param>
        /// <param name="buffer">Buffer duration for retry.</param>
        /// <param name="weight">Weight value for retry.</param>
        public TeamsRetry(int retryMax, TimeSpan buffer, double weight = 0.0d)
        {
            this.trials = Math.Max( (retryMax + 1), 1);
            this.buffer = buffer.TotalMilliseconds;
            this.weight = weight;
        }

        /// <summary>
        /// Creates instance for retry.
        /// If Retry-After is requested,
        /// the retry will be sent after (retry-after + 250ms).
        /// </summary>
        public TeamsRetry()
            : this(1, DEFAULT_BUFFER, 0.0f)
        {
        }

        /// <summary>
        /// Creates instance for retry.
        /// If Retry-After is requested,
        /// the retry will be sent after (retry-after + 250ms).
        /// </summary>
        /// <param name="retryMax">Max retry count.</param>
        public TeamsRetry(int retryMax)
            : this(retryMax, DEFAULT_BUFFER, 0.0f)
        {
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
        protected async Task<TTeamsResult> requestAsync<TTeamsResult, TTeamsObject>(Func< Task<TTeamsResult> > teamsRequestFunc, Func<TTeamsResult, int, bool> notificationFunc = null, CancellationToken? cancellationToken = null)
            where TTeamsResult : TeamsResult<TTeamsObject>, new()
            where TTeamsObject : TeamsObject, new()
        {
            TTeamsResult result = null;

            for(int i = 0; i < this.trials; i++)
            {
                if(result != null)
                {
                    if ( (result.HasRetryAfter && result.RetryAfter.Delta.HasValue) &&
                         ((notificationFunc == null) || notificationFunc(result, i)) )
                    {
                        await this.DelayAsync(result.RetryAfter.Delta.Value, cancellationToken);
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
        /// <param name="delta">Duration for retry.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used cancellation.</param>
        /// <returns><see cref="Task"/> for result.</returns>
        /// <exception cref="OverflowException">The (delta + (delta * weight) + buffer) is greater than <see cref="Int32.MaxValue"/> or less than <see cref="Int32.MinValue"/>.</exception>
        public async Task DelayAsync(TimeSpan delta, CancellationToken? cancellationToken = null)
        {
            int delay = Convert.ToInt32( (delta.TotalMilliseconds + (delta.TotalMilliseconds * this.weight) + this.buffer) );

            if(delay > 0)
            {
                if(cancellationToken.HasValue)
                {
                    await Task.Delay(delay, cancellationToken.Value);
                }
                else
                {
                    await Task.Delay(delay);
                }
            }
        }


    }

}
