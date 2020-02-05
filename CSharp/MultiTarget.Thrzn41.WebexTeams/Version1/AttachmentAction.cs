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
    /// Cisco Webex Teams Attachment Action object.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class AttachmentAction : TeamsData
    {

        /// <summary>
        /// A unique identifier for the action.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; internal set; }

        /// <summary>
        /// The Id of the person who performed the action.
        /// </summary>
        [JsonProperty(PropertyName = "personId")]
        public string PersonId { get; internal set; }

        /// <summary>
        /// The Id of the space the action was performed within.
        /// </summary>
        [JsonProperty(PropertyName = "roomId")]
        public string SpaceId { get; internal set; }

        /// <summary>
        /// The type name of action performed.
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string TypeName { get; internal set; }

        /// <summary>
        /// The type of action performed.
        /// </summary>
        [JsonIgnore]
        public AttachmentActionType Type
        {
            get
            {
                return AttachmentActionType.Parse(this.TypeName);
            }
        }

        /// <summary>
        /// The parent message the attachment action was performed on.
        /// </summary>
        [JsonProperty(PropertyName = "messageId")]
        public string MessageId { get; internal set; }

        /// <summary>
        /// The attachment action's inputs.
        /// </summary>
        [JsonProperty(PropertyName = "inputs")]
        public AttachmentActionInputs Inputs { get; internal set; }

        /// <summary>
        /// The date and time the action was created.
        /// </summary>
        [JsonProperty(PropertyName = "created")]
        public DateTime? Created { get; internal set; }

    }
}
