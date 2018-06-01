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

namespace Thrzn41.WebexTeams.Version1
{

    /// <summary>
    /// Cisco Webex Teams Space object.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Space : TeamsData
    {

        /// <summary>
        /// Id of the space.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; internal set; }

        /// <summary>
        /// Title of the space.
        /// </summary>
        [JsonProperty(PropertyName = "title")]
        public string Title { get; internal set; }

        /// <summary>
        /// Type name of the space.
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string TypeName { get; internal set; }

        /// <summary>
        /// Type of the space.
        /// </summary>
        [JsonIgnore]
        public SpaceType Type
        {
            get
            {
                return SpaceType.Parse(this.TypeName);
            }
        }

        /// <summary>
        /// Indicates the space is moderated space or not.
        /// </summary>
        [JsonProperty(PropertyName = "isLocked")]
        public bool? IsLocked { get; internal set; }

        /// <summary>
        /// SIP address for the space.
        /// </summary>
        [JsonProperty(PropertyName = "sipAddress")]
        public string SipAddress { get; internal set; }

        /// <summary>
        /// Team id that the space is associated with.
        /// </summary>
        [JsonProperty(PropertyName = "teamId")]
        public string TeamId { get; internal set; }

        /// <summary>
        /// Last activity datetime of the space.
        /// </summary>
        [JsonProperty(PropertyName = "lastActivity")]
        public DateTime? LastActivity { get; internal set; }

        /// <summary>
        /// <see cref="DateTime"/> when the space was created.
        /// </summary>
        [JsonProperty(PropertyName = "created")]
        public DateTime? Created { get; internal set; }

    }

}
