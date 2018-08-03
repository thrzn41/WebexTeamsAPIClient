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
    /// Cisco Webex Teams Webhook object.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Webhook : TeamsData
    {

        /// <summary>
        /// Id of the webhook.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; internal set; }

        /// <summary>
        /// Name of the webhook.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; internal set; }

        /// <summary>
        /// Target url of the webhook.
        /// </summary>
        [JsonProperty(PropertyName = "targetUrl")]
        public string TargetUrl { get; internal set; }

        /// <summary>
        /// Target Uri of the webhook.
        /// </summary>
        [JsonIgnore]
        public Uri TargetUri
        {
            get
            {
                if(String.IsNullOrEmpty(this.TargetUrl))
                {
                    return null;
                }

                return (new Uri(this.TargetUrl));
            }
        }

        /// <summary>
        /// Resource name of the webhook.
        /// </summary>
        [JsonProperty(PropertyName = "resource")]
        public string ResourceName { get; internal set; }

        /// <summary>
        /// Resource of the webhook.
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
        /// Event name of the webhook.
        /// </summary>
        [JsonProperty(PropertyName = "event")]
        public string EventTypeName { get; internal set; }

        /// <summary>
        /// Event of the webhook.
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
        /// Filter for the webhook.
        /// </summary>
        [JsonProperty(PropertyName = "filter")]
        public string Filter { get; internal set; }

        /// <summary>
        /// Secret for the webhook.
        /// </summary>
        [JsonProperty(PropertyName = "secret")]
        public string Secret { get; internal set; }

        /// <summary>
        /// Indicates if the webhook is active.
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public string StatusName { get; internal set; }

        /// <summary>
        /// Indicates if the webhook is active.
        /// </summary>
        [JsonIgnore]
        public WebhookStatus Status
        {
            get
            {
                return WebhookStatus.Parse(this.StatusName);
            }
        }

        /// <summary>
        /// <see cref="DateTime"/> when the webhook was created.
        /// </summary>
        [JsonProperty(PropertyName = "created")]
        public DateTime? Created { get; internal set; }


        /// <summary>
        /// Creates <see cref="EventValidator"/>.
        /// </summary>
        /// <returns><see cref="EventValidator"/> instance.</returns>
        public EventValidator CreateEventValidator()
        {
            return EventValidator.Create(this.Secret);
        }

    }

}
