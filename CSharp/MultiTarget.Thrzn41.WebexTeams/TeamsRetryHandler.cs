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

namespace Thrzn41.WebexTeams
{

    /// <summary>
    /// Retry handler when the APIClients receive HTTP 429 response.
    /// You can set the instance in <see cref="TeamsAPI"/> methods and you can reuse the instance.
    /// If you want the client also retry on HTTP 500, 502, 503 and 504 response, use <see cref="TeamsRetryOnErrorHandler"/>.
    /// </summary>
    public class TeamsRetryHandler : TeamsRetry
    {


        /// <summary>
        /// Creates <see cref="TeamsRetryHandler"/>.
        /// You can reuse the instance, if the retry max is the same.
        /// </summary>
        /// <param name="retryMax">Max retry count.</param>
        public TeamsRetryHandler(int retryMax)
            : base(retryMax)
        {
        }

    }

}
