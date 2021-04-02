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
    /// Enumerator to iterate <see cref="TeamsListResult{TTeamsObject}"/>.
    /// </summary>
    /// <typeparam name="TTeamsObject">Teams Object that is returned on API request.</typeparam>
    /// <remarks>You should await result of <see cref="MoveNextAsync(CancellationToken?)"/> before getting <see cref="CurrentResult"/> and subsequent MoveNextAsync().</remarks>
    public class TeamsListResultEnumerator<TTeamsObject>
        where TTeamsObject : TeamsObject, new()
    {


        /// <summary>
        /// Previous result.
        /// </summary>
        private TeamsListResult<TTeamsObject> prev;

        /// <summary>
        /// Current result.
        /// </summary>
        public TeamsListResult<TTeamsObject> CurrentResult { get; private set; } = null;

        /// <summary>
        /// Current PageIndex.
        /// </summary>
        public int CurrentPageIndex { get; private set; } = -1;

        /// <summary>
        /// Retry will be tried by this instance if needed.
        /// </summary>
        private TeamsRetry retry;

        /// <summary>
        /// Function to be notified on retry.
        /// </summary>
        private Func<TeamsListResult<TTeamsObject>, int, bool> retryNotificationFunc;



        /// <summary>
        /// Constructor of <see cref="TeamsListResultEnumerator{TTeamsObject}"/>.
        /// </summary>
        /// <param name="listResult">First result.</param>
        /// <param name="retry">Retry will be tried by this instance if needed.</param>
        /// <param name="retryNotificationFunc">Function to be notified on retry.</param>
        internal TeamsListResultEnumerator(TeamsListResult<TTeamsObject> listResult, TeamsRetry retry, Func<TeamsListResult<TTeamsObject>, int, bool> retryNotificationFunc = null)
        {
            this.prev = listResult;

            this.retry                 = retry;
            this.retryNotificationFunc = retryNotificationFunc;
        }

        /// <summary>
        /// Constructor of <see cref="TeamsListResultEnumerator{TTeamsObject}"/>.
        /// </summary>
        /// <param name="listResult">First result.</param>
        internal TeamsListResultEnumerator(TeamsListResult<TTeamsObject> listResult)
            : this(listResult, null, null)
        {
        }


        /// <summary>
        /// Advances the enumerator to the next <see cref="TeamsListResult{TTeamsObject}"/>.
        /// </summary>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns>true if the enumerator was successfully advanced to the next <see cref="TeamsListResult{TTeamsObject}"/>; false if the enumerator has passed the end of the result.</returns>
        public async Task<bool> MoveNextAsync(CancellationToken? cancellationToken = null)
        {
            bool isMoved = false;

            if(this.CurrentResult == null)
            {
                this.CurrentResult = this.prev;

                isMoved = true;
            }
            else if(this.CurrentResult.HasNext)
            {
                this.prev = this.CurrentResult;

                if (this.retry == null)
                {
                    this.CurrentResult = await this.prev.ListNextAsync(cancellationToken);
                }
                else
                {
                    this.CurrentResult = await this.retry.requestAsync<TeamsListResult<TTeamsObject>, TTeamsObject>(
                        () => (this.prev.ListNextAsync(cancellationToken)),
                        this.retryNotificationFunc,
                        cancellationToken);
                }

                isMoved = true;
            }

            if(isMoved)
            {
                this.CurrentPageIndex++;
            }

            return isMoved;
        }


    }

}
