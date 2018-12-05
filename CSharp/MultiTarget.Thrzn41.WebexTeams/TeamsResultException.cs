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
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Thrzn41.WebexTeams
{

    /// <summary>
    /// Represents errors that occur during a Cisco Webex Teams REST API request.
    /// </summary>
    public class TeamsResultException : TeamsException
    {


        /// <summary>
        /// Teams Result info related with this exception.
        /// </summary>
        public TeamsResultInfo ResultInfo { get; internal set; }

        /// <summary>
        /// Http status code returned on the API request.
        /// </summary>
        public HttpStatusCode HttpStatusCode
        {
            get
            {
                return this.ResultInfo.HttpStatusCode;
            }
        }

        /// <summary>
        /// Tracking id of the request.
        /// This id can be used for technical support.
        /// </summary>
        public string TrackingId
        {
            get
            {
                return this.ResultInfo.TrackingId;
            }
        }

        /// <summary>
        /// Indicates whether the result has tracking id or not.
        /// </summary>
        public bool HasTrackingId
        {
            get
            {
                return this.ResultInfo.HasTrackingId;
            }
        }

        /// <summary>
        /// Retry-After header value.
        /// </summary>
        public RetryConditionHeaderValue RetryAfter
        {
            get
            {
                return this.ResultInfo.RetryAfter;
            }
        }

        /// <summary>
        /// Indicates the request has Retry-After header value.
        /// </summary>
        public bool HasRetryAfter
        {
            get
            {
                return this.ResultInfo.HasRetryAfter;
            }
        }

        /// <summary>
        /// Creates <see cref="TeamsResultException"/>.
        /// </summary>
        /// <param name="message">A message that indicates this error.</param>
        /// <param name="resultInfo">Teams Result info related with this exception.</param>
        public TeamsResultException(string message, TeamsResultInfo resultInfo)
            : base(message)
        {
            this.ResultInfo = resultInfo;
        }

    }

}
