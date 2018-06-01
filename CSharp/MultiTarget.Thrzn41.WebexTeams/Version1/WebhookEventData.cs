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

namespace Thrzn41.WebexTeams.Version1
{

    /// <summary>
    /// Cisco Webex Teams Webhook Event data object.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class WebhookEventData : TeamsData
    {

        /// <summary>
        /// The webhook ID. This is the same ID returned when you created the webhook and is what you would use to view the webhook configuration or delete the webhook.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; internal set; }

        /// <summary>
        /// The name you gave your webhook when you created it.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; internal set; }

        /// <summary>
        /// The resource the webhook data is about.
        /// </summary>
        [JsonProperty(PropertyName = "resource")]
        public string ResourceName { get; internal set; }

        /// <summary>
        /// The resource the webhook data is about.
        /// </summary>
        [JsonIgnore]
        public EventResource Resource
        {
            get
            {
                return EventResource.Parse(this.ResourceName);
            }
        }

        /// <summary>
        /// The type of event this webhook is for.
        /// </summary>
        [JsonProperty(PropertyName = "event")]
        public string EventTypeName { get; internal set; }

        /// <summary>
        /// The type of event this webhook is for.
        /// </summary>
        [JsonIgnore]
        public EventType EventType
        {
            get
            {
                return EventType.Parse(this.EventTypeName);
            }
        }

        /// <summary>
        /// Lists any filters that you applied that triggered this webhook.
        /// </summary>
        [JsonProperty(PropertyName = "filter")]
        public string Filter { get; internal set; }

        /// <summary>
        /// The organization that owns the webhook. This usually is the organization the user that added the webhook belongs to.
        /// </summary>
        [JsonProperty(PropertyName = "orgId")]
        public string OrganizationId { get; internal set; }

        /// <summary>
        /// The personId of the user that added the webhook.
        /// </summary>
        [JsonProperty(PropertyName = "createdBy")]
        public string CreatedBy { get; internal set; }

        /// <summary>
        /// Identifies the application that added the webhook.
        /// </summary>
        [JsonProperty(PropertyName = "appId")]
        public string ApplicationId { get; internal set; }

        /// <summary>
        /// Indicates if the webhook is owned by the org or the creator. Webhooks owned by the creator can only receive events that are accessible to the creator of the webhook. Those owned by the organization will receive events that are visible to anyone in the organization.
        /// </summary>
        [JsonProperty(PropertyName = "ownedBy")]
        public string OwnedByName { get; internal set; }

        /// <summary>
        /// Indicates if the webhook is owned by the org or the creator. Webhooks owned by the creator can only receive events that are accessible to the creator of the webhook. Those owned by the organization will receive events that are visible to anyone in the organization.
        /// </summary>
        [JsonIgnore]
        public EventOwnerType OwnedBy
        {
            get
            {
                return EventOwnerType.Parse(this.OwnedByName);
            }
        }

        /// <summary>
        /// Indicates if the webhook is active.
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string WebhookStatusName { get; internal set; }

        /// <summary>
        /// Indicates if the webhook is active.
        /// </summary>
        [JsonIgnore]
        public WebhookStatus WebhookStatus
        {
            get
            {
                return WebhookStatus.Parse(this.WebhookStatusName);
            }
        }

        /// <summary>
        /// The personId of the user that caused the webhook to be sent. For example, on a messsage created webhook, the author of the message will be the actor. On a membership deleted webhook, the actor is the person who removed a user from a room.
        /// </summary>
        [JsonProperty(PropertyName = "actorId")]
        public string ActorId { get; internal set; }


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
        /// Space resource data.
        /// </summary>
        [JsonIgnore]
        public Space SpaceData
        {
            get
            {
                return this.checkAndGetResouceData<Space>(EventResource.Space);
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
            
            if(this.JsonExtensionData != null)
            {
                JToken jtoken = null;

                if(this.JsonExtensionData.TryGetValue("data", out jtoken))
                {
                    result = jtoken.ToObject<TTeamsData>();
                }
            }

            if(result == null)
            {
                result = new TTeamsData();

                result.HasValues = false;
            }

            return result;
        }

    }

}
