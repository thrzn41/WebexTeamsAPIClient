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
    /// Cisco Webex Teams Person object.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Person : TeamsData
    {

        /// <summary>
        /// Id of the person.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; internal set; }

        /// <summary>
        /// Email addresses of the person.
        /// </summary>
        [JsonProperty(PropertyName = "emails")]
        public string[] Emails { get; internal set; }

        /// <summary>
        /// Default Email address of the person.
        /// </summary>
        [JsonIgnore]
        public string Email {
            get
            {
                return ((this.Emails != null && this.Emails.Length > 0) ? (this.Emails[0]) : null);
            }
        }

        /// <summary>
        /// Phone numbers of the person.
        /// </summary>
        [JsonProperty(PropertyName = "phoneNumbers")]
        public PhoneNumber[] PhoneNumbers { get; internal set; }

        /// <summary>
        /// Full name of the person.
        /// </summary>
        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; internal set; }

        /// <summary>
        /// Nick name of the person.
        /// </summary>
        [JsonProperty(PropertyName = "nickName")]
        public string NickName { get; internal set; }

        /// <summary>
        /// First name of the person.
        /// </summary>
        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; internal set; }

        /// <summary>
        /// Last name of the person.
        /// </summary>
        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; internal set; }

        /// <summary>
        /// URL string to the person's avatar in PNG format.
        /// </summary>
        [JsonProperty(PropertyName = "avatar")]
        public string Avatar { get; internal set; }

        /// <summary>
        /// Uri to the person's avatar in PNG format.
        /// </summary>
        [JsonIgnore]
        public Uri AvatarUri {
            get
            {
                if( String.IsNullOrEmpty(this.Avatar) )
                {
                    return null;
                }

                return (new Uri(this.Avatar));
            }
        }

        /// <summary>
        /// ID of the organization to which this person belongs.
        /// </summary>
        [JsonProperty(PropertyName = "orgId")]
        public string OrganizationId { get; internal set; }

        /// <summary>
        /// Roles of the person.
        /// </summary>
        [JsonProperty(PropertyName = "roles")]
        public string[] Roles { get; internal set; }

        /// <summary>
        /// Licenses allocated to the person.
        /// </summary>
        [JsonProperty(PropertyName = "licenses")]
        public string[] Licenses { get; internal set; }

        /// <summary>
        /// <see cref="DateTime"/> when the person was created.
        /// </summary>
        [JsonProperty(PropertyName = "created")]
        public DateTime? Created { get; internal set; }

        /// <summary>
        /// <see cref="DateTime"/> when the person was modified last.
        /// </summary>
        [JsonProperty(PropertyName = "lastModified")]
        public DateTime? LastModified { get; internal set; }

        /// <summary>
        /// Timezone info for the person.
        /// </summary>
        [JsonProperty(PropertyName = "timezone")]
        public string TimezoneName { get; internal set; }

        /// <summary>
        /// <see cref="DateTime"/> when the person was active last.
        /// </summary>
        [JsonProperty(PropertyName = "LastActivity")]
        public DateTime? LastActivity { get; internal set; }

        /// <summary>
        /// Status string of the person.
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string StatusName { get; internal set; }

        /// <summary>
        /// Status of the person.
        /// </summary>
        [JsonIgnore]
        public PersonStatus Status {
            get
            {
                return PersonStatus.Parse(this.StatusName);
            }
        }

        /// <summary>
        /// Person is pending status.
        /// </summary>
        [JsonProperty(PropertyName = "invitePending")]
        public bool? IsInvitePending { get; internal set; }

        /// <summary>
        /// Person is login enabled.
        /// </summary>
        [JsonProperty(PropertyName = "loginEnabled")]
        public bool? IsLoginEnabled { get; internal set; }

        /// <summary>
        /// Person type name.
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string TypeName { get; internal set; }

        /// <summary>
        /// Person type.
        /// </summary>
        [JsonIgnore]
        public PersonType Type
        {
            get
            {
                return PersonType.Parse(this.TypeName);
            }
        }




        /// <summary>
        /// Checks ownership of message.
        /// </summary>
        /// <param name="message"><see cref="Message"/> to be checked.</param>
        /// <returns><see cref="OwnershipStatus"/> that indicates the ownership status.</returns>
        public OwnershipStatus CheckOwnershipStatus(Message message)
        {
            var status = OwnershipStatus.Unknown;

            if( !String.IsNullOrEmpty(this.Id) && message != null && !String.IsNullOrEmpty(message.PersonId) )
            {
                if(this.Id == message.PersonId)
                {
                    status = OwnershipStatus.Hold;
                }
                else
                {
                    status = OwnershipStatus.NotHold;
                }
            }

            return status;
        }

    }

}
