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
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Thrzn41.WebexTeams.Version1.Admin
{

    /// <summary>
    /// Cisco Webex Teams Event data object.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class EventData : TeamsData
    {

        /// <summary>
        /// The event ID. This is the same ID returned when you created the event and is what you would use to view the event configuration or delete the event.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; internal set; }

        /// <summary>
        /// The resource the event data is about.
        /// </summary>
        [JsonProperty(PropertyName = "resource")]
        public string ResourceName { get; internal set; }

        /// <summary>
        /// The resource the event data is about.
        /// </summary>
        [JsonIgnore]
        public EventResource Resource {
            get
            {
                return EventResource.Parse(this.ResourceName);
            }
        }

        /// <summary>
        /// The type of event this event is for.
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string TypeName { get; internal set; }

        /// <summary>
        /// The type of event this event is for.
        /// </summary>
        [JsonIgnore]
        public EventType Type
        {
            get
            {
                return EventType.Parse(this.TypeName);
            }
        }

        /// <summary>
        /// The personId of the user that caused the event to be sent. For example, on a messsage created event, the author of the message will be the actor. On a membership deleted event, the actor is the person who removed a user from a room.
        /// </summary>
        [JsonProperty(PropertyName = "actorId")]
        public string ActorId { get; internal set; }

        /// <summary>
        /// The organization that owns the event. This usually is the organization the user that added the event belongs to.
        /// </summary>
        [JsonProperty(PropertyName = "orgId")]
        public string OrganizationId { get; internal set; }

        /// <summary>
        /// Identifies the application that added the event.
        /// </summary>
        [JsonProperty(PropertyName = "appId")]
        public string ApplicationId { get; internal set; }

        /// <summary>
        /// <see cref="DateTime"/> when the event was created.
        /// </summary>
        [JsonProperty(PropertyName = "created")]
        public DateTime? Created { get; internal set; }


        /// <summary>
        /// Space membership resource data.
        /// </summary>
        [JsonIgnore]
        public SpaceMembership SpaceMembershipData
        {
            get
            {
                return this.checkAndGetResouceData<SpaceMembership>(EventResource.SpaceMembership);
            }
        }

        /// <summary>
        /// Message resource data.
        /// </summary>
        [JsonIgnore]
        public Message MessageData
        {
            get
            {
                return this.checkAndGetResouceData<Message>(EventResource.Message);
            }
        }




        /// <summary>
        /// Checks and gets resource data.
        /// </summary>
        /// <typeparam name="TTeamsData">Type of resource data.</typeparam>
        /// <param name="eventResource"><see cref="EventResource"/> to be checked.</param>
        /// <returns>Resource data.</returns>
        private TTeamsData checkAndGetResouceData<TTeamsData>(EventResource eventResource)
            where TTeamsData : TeamsData, new()
        {
            TTeamsData result;

            if (this.Resource == eventResource)
            {
                result = this.GetResourceData<TTeamsData>();
            }
            else
            {
                result = new TTeamsData();
                result.HasValues = false;
            }

            return result;
        }

        /// <summary>
        /// Gets resource data.
        /// </summary>
        /// <typeparam name="TTeamsData">Type of resource data.</typeparam>
        /// <returns>Resource data.</returns>
        public TTeamsData GetResourceData<TTeamsData>()
            where TTeamsData : TeamsData, new()
        {
            TTeamsData result = null;

            if (this.JsonExtensionData != null)
            {
                JToken jtoken = null;

                if (this.JsonExtensionData.TryGetValue("data", out jtoken))
                {
                    result = jtoken.ToObject<TTeamsData>();
                }
            }

            if (result == null)
            {
                result = new TTeamsData();

                result.HasValues = false;
            }

            return result;
        }

    }

}
