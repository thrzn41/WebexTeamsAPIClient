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
using Thrzn41.WebexTeams.Version1.AttachmentData;

namespace Thrzn41.WebexTeams.Version1
{

    /// <summary>
    /// Adaptive Card.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class AdaptiveCardAttachment : Attachment
    {

        /// <summary>
        /// Default Json Converter.
        /// </summary>
        [JsonIgnore]
        private static readonly TeamsJsonConverter JSON_CONVERTER = TeamsObject.DEFAULT_JSON_CONVERTER;

        /// <summary>
        /// Creates <see cref="AdaptiveCardAttachment"/>.
        /// </summary>
        public AdaptiveCardAttachment()
        {
            this.Type = AttachmentType.AdaptiveCard;
        }


        /// <summary>
        /// Creates <see cref="AdaptiveCardAttachment"/> from Json string.
        /// </summary>
        /// <param name="jsonString">Json string which represents Adaptive Card.</param>
        /// <returns><see cref="AdaptiveCardAttachment"/> created from Json string.</returns>
        /// <exception cref="TeamsJsonSerializationException">Throws on deserialization error.</exception>
        public static AdaptiveCardAttachment FromJsonString(string jsonString)
        {
            var content = JSON_CONVERTER.DeserializeObject<AdaptiveCardExtensionData>(jsonString);

            return new AdaptiveCardAttachmentFromString(content);
        }

        /// <summary>
        /// Creates <see cref="AdaptiveCardAttachment"/> from object.
        /// </summary>
        /// <param name="obj">The object which represents Adaptive Card.</param>
        /// <returns><see cref="AdaptiveCardAttachment"/> created from Json string.</returns>
        /// <exception cref="TeamsJsonSerializationException">Throws on deserialization error.</exception>
        public static AdaptiveCardAttachment FromObject(object obj)
        {
            return new AdaptiveCardAttachmentFromObject(obj);
        }

    }
}
