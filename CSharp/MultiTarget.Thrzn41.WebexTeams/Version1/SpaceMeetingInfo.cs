/* 
 * MIT License
 * 
 * Copyright(c) 2020 thrzn41
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

namespace Thrzn41.WebexTeams.Version1
{

    /// <summary>
    /// Cisco Webex Teams Space meeting info object.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class SpaceMeetingInfo : TeamsData
    {

        /// <summary>
        /// The unique identifier for the Space.
        /// </summary>
        [JsonProperty(PropertyName = "roomId")]
        public string SpaceId { get; internal set; }

        /// <summary>
        /// The Webex meeting URL string for the Space.
        /// </summary>
        [JsonProperty(PropertyName = "meetingLink")]
        public string MeetingLink { get; internal set; }

        /// <summary>
        /// The Webex meeting Uri string for the Space.
        /// </summary>
        [JsonIgnore]
        public Uri MeetingLinkUri
        {
            get
            {
                if(String.IsNullOrEmpty(this.MeetingLink))
                {
                    return null;
                }

                return (new Uri(this.MeetingLink));
            }
        }

        /// <summary>
        /// The SIP address for the Space.
        /// </summary>
        [JsonProperty(PropertyName = "sipAddress")]
        public string SipAddress { get; internal set; }

        /// <summary>
        /// The Webex meeting number for the Space.
        /// </summary>
        [JsonProperty(PropertyName = "meetingNumber")]
        public string MeetingNumber { get; internal set; }

        /// <summary>
        /// The toll-free PSTN number for the Space.
        /// </summary>
        [JsonProperty(PropertyName = "callInTollFreeNumber")]
        public string CallInTollFreeNumber { get; internal set; }

        /// <summary>
        /// The toll (local) PSTN number for the Space.
        /// </summary>
        [JsonProperty(PropertyName = "callInTollNumber")]
        public string CallInTollNumber { get; internal set; }

    }
}
