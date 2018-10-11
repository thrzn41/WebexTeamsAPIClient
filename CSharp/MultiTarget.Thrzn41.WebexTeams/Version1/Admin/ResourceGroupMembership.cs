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

namespace Thrzn41.WebexTeams.Version1.Admin
{


    /// <summary>
    /// Cisco Webex Teams Resource Group Membership object.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ResourceGroupMembership : TeamsData
    {

        /// <summary>
        /// Id of the Resource Group Membership.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; internal set; }

        /// <summary>
        /// The ID of the resource group to which this resource group membership belongs.
        /// </summary>
        [JsonProperty(PropertyName = "resourceGroupId")]
        public string ResourceGroupId { get; internal set; }

        /// <summary>
        /// The license ID for the Hybrid Services licensed feature.
        /// </summary>
        [JsonProperty(PropertyName = "licenseId")]
        public string LicenseId { get; internal set; }

        /// <summary>
        /// The ID of the person to which this resource group membership is associated.
        /// </summary>
        [JsonProperty(PropertyName = "personId")]
        public string PersonId { get; internal set; }

        /// <summary>
        /// The Organization Id for the person.
        /// </summary>
        [JsonProperty(PropertyName = "personOrgId")]
        public string PersonOrganizationId { get; internal set; }

        /// <summary>
        /// Status string of the Resource Group Membership.
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string StatusName { get; internal set; }

        /// <summary>
        /// Status of the Resource Group Membership.
        /// </summary>
        [JsonIgnore]
        public ResourceGroupMembershipStatus Status
        {
            get
            {
                return ResourceGroupMembershipStatus.Parse(this.StatusName);
            }
        }


    }

}
