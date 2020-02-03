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
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Thrzn41.WebexTeams
{

    /// <summary>
    /// Base Object for all Cisco Webex Teams objects.
    /// This class provides feature to convert object to/from Json.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class TeamsObject : TeamsJsonObject
    {

        /// <summary>
        /// Default Json converter.
        /// </summary>
        [JsonIgnore]
        internal static readonly TeamsJsonConverter DEFAULT_JSON_CONVERTER = new TeamsJsonObjectConverter();


        /// <summary>
        /// Json converter to serialize or deserialize to/from Json.
        /// </summary>
        [JsonIgnore]
        private TeamsJsonConverter jsonConverter = null;

        /// <summary>
        /// Json converter to serialize or deserialize to/from Json.
        /// </summary>
        [JsonIgnore]
        private TeamsJsonConverter JsonConverter
        {
            get
            {
                return (this.jsonConverter ?? DEFAULT_JSON_CONVERTER);
            }
        }


        /// <summary>
        /// Indicates the Teams Object has values or not.
        /// </summary>
        [JsonIgnore]
        public bool HasValues { get; internal set; } = true;


        /// <summary>
        /// Extension data which contains non-deserialized json.
        /// </summary>
        [JsonExtensionData]
        internal protected IDictionary<string, JToken> JsonExtensionData { get; internal set; }

        /// <summary>
        /// Indicates this Teams Object has extension data.
        /// </summary>
        [JsonIgnore]
        public bool HasExtensionData
        {
            get
            {
                return (this.JsonExtensionData != null && this.JsonExtensionData.Count > 0);
            }
        }


        /// <summary>
        /// Indicates this Teams Object has errors.
        /// </summary>
        [JsonIgnore]
        public virtual bool HasErrors { get; internal set; } = false;


        /// <summary>
        /// Creates <see cref="TeamsObject"/>.
        /// </summary>
        /// <param name="dateFormatStringForSerializer">Date format to serialize to Json.</param>
        /// <param name="dateFormatStringForDeserializer">Date format to deserialize from Json.</param>
        public TeamsObject(string dateFormatStringForSerializer, string dateFormatStringForDeserializer)
        {
            this.jsonConverter = new TeamsJsonObjectConverter(dateFormatStringForSerializer, dateFormatStringForDeserializer);
        }

        /// <summary>
        /// Creates <see cref="TeamsObject"/>.
        /// </summary>
        /// <param name="dateFormatStringForSerializer">Date format to serialize to Json.</param>
        public TeamsObject(string dateFormatStringForSerializer)
        {
            this.jsonConverter = new TeamsJsonObjectConverter(dateFormatStringForSerializer);
        }

        /// <summary>
        /// Creates <see cref="TeamsObject"/>.
        /// </summary>
        public TeamsObject()
        {
        }

        /// <summary>
        /// Gets error message.
        /// </summary>
        /// <returns>Error message.</returns>
        public virtual string GetErrorMessage()
        {
            return null;
        }

        /// <summary>
        /// Converts object to Json style string.
        /// </summary>
        /// <returns>Json style string that represents this object.</returns>
        public virtual string ToJsonString()
        {
            return this.JsonConverter.SerializeObject(this);
        }


        /// <summary>
        /// Converts object to <see cref="TeamsExtensionObject"/>.
        /// <see cref="TeamsExtensionObject"/> will be used to get data that is not available in API Client currently.
        /// </summary>
        /// <returns><see cref="TeamsExtensionObject"/>.</returns>
        public virtual TeamsExtensionObject ToExtensionObject()
        {
            return FromJsonString<TeamsExtensionObject>(this.ToJsonString(), this.JsonConverter);
        }

        /// <summary>
        /// Converts Json string to a Cisco Webex Teams object.
        /// </summary>
        /// <typeparam name="TTeamsObject">A subclass type of TeamsObject.</typeparam>
        /// <param name="jsonString">Json style string to be converted.</param>
        /// <param name="jsonConverter">Json converter to deserialize the object.</param>
        /// <returns>Cisco Webex Teams object converted from Json string.</returns>
        public static TTeamsObject FromJsonString<TTeamsObject>(string jsonString, TeamsJsonConverter jsonConverter)
            where TTeamsObject : TeamsObject, new()
        {
            return (jsonConverter ?? DEFAULT_JSON_CONVERTER).DeserializeObject<TTeamsObject>(jsonString);
        }

        /// <summary>
        /// Converts Json string to a Cisco Webex Teams object.
        /// </summary>
        /// <typeparam name="TTeamsObject">A subclass type of TeamsObject.</typeparam>
        /// <param name="jsonString">Json style string to be converted.</param>
        /// <returns>Cisco Webex Teams object converted from Json string.</returns>
        public static TTeamsObject FromJsonString<TTeamsObject>(string jsonString)
            where TTeamsObject : TeamsObject, new()
        {
            return DEFAULT_JSON_CONVERTER.DeserializeObject<TTeamsObject>(jsonString);
        }


    }

}
