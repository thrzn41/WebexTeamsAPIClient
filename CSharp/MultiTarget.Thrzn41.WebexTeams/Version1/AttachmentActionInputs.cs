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
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Thrzn41.WebexTeams.Version1
{

    /// <summary>
    /// Cisco Webex Teams Attachment Action Inputs object.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class AttachmentActionInputs
    {
        /// <summary>
        /// Default Json Converter.
        /// </summary>
        [JsonIgnore]
        private static readonly TeamsJsonObjectConverter JSON_CONVERTER = TeamsObject.DEFAULT_JSON_CONVERTER;

        /// <summary>
        /// Empty keys.
        /// </summary>
        [JsonIgnore]
        private static readonly string[] EMPTY_KEYS = new string[0];

        /// <summary>
        /// Extension data key list.
        /// </summary>
        [JsonIgnore]
        public ICollection<string> Keys
        {
            get
            {
                if (!this.HasExtensionData)
                {
                    return EMPTY_KEYS;
                }

                return this.JsonExtensionData.Keys;
            }
        }

        /// <summary>
        /// Determines whether this Json Extension data contains a Json object with the specified key.
        /// </summary>
        /// <param name="key">The key to be determined.</param>
        /// <returns>true if the Json Extension data contains a Json object with the key; otherwise, false.</returns>
        public bool ContainsKey(string key)
        {
            if (key == null || !this.HasExtensionData)
            {
                return false;
            }

            return this.JsonExtensionData.ContainsKey(key);
        }

        /// <summary>
        /// Json extension data.
        /// </summary>
        [JsonExtensionData]
        internal Dictionary<string, JToken> JsonExtensionData { get; set; }

        /// <summary>
        /// Indicates this Teams Object has extension data.
        /// </summary>
        [JsonIgnore]
        private bool HasExtensionData
        {
            get
            {
                return (this.JsonExtensionData != null && this.JsonExtensionData.Count > 0);
            }
        }


        /// <summary>
        /// Gets input value.
        /// </summary>
        /// <typeparam name="T">Type of result.</typeparam>
        /// <param name="key">The key of Input object.</param>
        /// <returns>The input value.</returns>
        /// <exception cref="TeamsKeyNotFoundException">Throws when the key is not found.</exception>
        /// <exception cref="TeamsJsonSerializationException">Throws on serialization error.</exception>
        public T GetInputValue<T>(string key)
        {
            if (key == null || !this.HasExtensionData || !this.JsonExtensionData.ContainsKey(key))
            {
                throw new TeamsKeyNotFoundException(key);
            }

            T result;

            try
            {
                result = this.JsonExtensionData[key].ToObject<T>(JSON_CONVERTER.Deserializer);
            }
            catch (JsonReaderException jre)
            {
                throw new TeamsJsonSerializationException(TeamsSerializationOperation.Deserialize, jre.LineNumber, jre.LinePosition, jre.Path);
            }
            catch (JsonSerializationException jse)
            {
                throw new TeamsJsonSerializationException(TeamsSerializationOperation.Deserialize, jse.LineNumber, jse.LinePosition, jse.Path);
            }

            return result;
        }


        /// <summary>
        /// Creates <see cref="AttachmentActionInputs"/> from Json string.
        /// </summary>
        /// <param name="jsonString">Json string which represents Attachment Action inputs.</param>
        /// <returns><see cref="AttachmentActionInputs"/> created from Json string.</returns>
        /// <exception cref="TeamsJsonSerializationException">Throws on deserialization error.</exception>
        public static AttachmentActionInputs FromJsonString(string jsonString)
        {
            return JSON_CONVERTER.DeserializeObject<AttachmentActionInputs>(jsonString);
        }

        /// <summary>
        /// Creates <see cref="AttachmentActionInputs"/> from Json string.
        /// </summary>
        /// <param name="obj">The object which represents Attachment Action inputs.</param>
        /// <returns><see cref="AttachmentActionInputs"/> created from Json string.</returns>
        /// <exception cref="TeamsJsonSerializationException">Throws on deserialization error.</exception>
        public static AttachmentActionInputs FromObject(object obj)
        {
            string jsonString = JSON_CONVERTER.SerializeObject(obj);

            return FromJsonString(jsonString);
        }

    }

}
