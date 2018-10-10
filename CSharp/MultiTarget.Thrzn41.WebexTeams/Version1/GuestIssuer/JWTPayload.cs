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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Thrzn41.WebexTeams.Version1.GuestIssuer
{


    /// <summary>
    /// JWT Payload.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    internal class JWTPayload : TeamsData
    {

        /// <summary>
        /// The subject of the token.
        /// A unique, public identifier for the end-user of the token.
        /// This claim may contain only letters, numbers, and hyphens.
        /// This claim is required.
        /// </summary>
        [JsonProperty(PropertyName = "sub")]
        public string Subject { get; internal set; }


        /// <summary>
        /// The display name of the guest user.
        /// This will be the name shown in Webex Teams clients.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; internal set; }


        /// <summary>
        /// The issuer of the token.
        /// Use the Guest Issuer ID provided in My Webex Teams Apps.
        /// This claim is required.
        /// </summary>
        [JsonProperty(PropertyName = "iss")]
        public string Issuer { get; internal set; }


        /// <summary>
        /// The expiration time of the token, as a UNIX timestamp in seconds.
        /// Use the lowest practical value for the use of the token.
        /// This claim is required.
        /// </summary>
        [JsonProperty(PropertyName = "exp")]
        public Int64? ExpirationTime { get; internal set; }


    }
}
