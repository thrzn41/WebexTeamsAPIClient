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

namespace Thrzn41.WebexTeams
{


    /// <summary>
    /// The request info of Teams API.
    /// </summary>
    public class TeamsRequestInfo
    {

        /// <summary>
        /// Creates TeamsRequestInfo.
        /// </summary>
        /// <param name="request"><see cref="HttpRequestMessage"/> of this request.</param>
        internal TeamsRequestInfo(HttpRequestMessage request)
        {
            this.Uri         = request.RequestUri;
            this.HttpMethod  = request.Method;
            this.HttpVersion = request.Version;
        }


        /// <summary>
        /// <see cref="Uri"/> of this request.
        /// </summary>
        public Uri Uri { get; internal set; }

        /// <summary>
        /// <see cref="HttpMethod"/> of this request.
        /// </summary>
        public HttpMethod HttpMethod { get; internal set; }

        /// <summary>
        /// <see cref="Version"/> of Http of this request.
        /// </summary>
        public Version HttpVersion { get; internal set; }

        /// <summary>
        /// Content-Length in bytes for request.
        /// </summary>
        public long? ContentLength { get; internal set; }

        /// <summary>
        /// Indicates the result has Content length.
        /// </summary>
        public bool HasContentLength
        {
            get
            {
                return this.ContentLength.HasValue;
            }
        }

        /// <summary>
        /// Gets Request line.
        /// </summary>
        /// <returns>Request line of this request.</returns>
        public string GetRequestLine()
        {
            string method  = "UNKNOWN";
            string uri     = "*";
            string version = "1.1";

            if(this.HttpMethod != null)
            {
                method = this.HttpMethod.ToString();
            }

            if(this.Uri != null)
            {
                uri = this.Uri.AbsolutePath;
            }

            if(this.HttpVersion != null)
            {
                version = this.HttpVersion.ToString();
            }

            return String.Format("{0} {1} HTTP/{2}", method, uri, version);
        }

    }

}
