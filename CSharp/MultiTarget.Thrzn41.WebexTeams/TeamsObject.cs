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

namespace Thrzn41.WebexTeams
{

    /// <summary>
    /// Base Object for all Cisco Webex Teams objects.
    /// This class provides feature to convert object to/from Json.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class TeamsObject
    {

        /// <summary>
        /// Settings for Json serializer.
        /// </summary>
        [JsonIgnore]
        private static readonly JsonSerializerSettings SERIALIZER_SETTINGS = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting        = Formatting.None,
        };

        /// <summary>
        /// Settings for Json deserializer.
        /// </summary>
        [JsonIgnore]
        private static readonly JsonSerializerSettings DESERIALIZER_SETTINGS = new JsonSerializerSettings
        {
        };


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
            return JsonConvert.SerializeObject(this, SERIALIZER_SETTINGS);
        }


        /// <summary>
        /// Converts object to <see cref="TeamsExtensionObject"/>.
        /// <see cref="TeamsExtensionObject"/> will be used to get data that is not available in API Client currently.
        /// </summary>
        /// <returns><see cref="TeamsExtensionObject"/>.</returns>
        public virtual TeamsExtensionObject ToExtensionObject()
        {
            return FromJsonString<TeamsExtensionObject>(this.ToJsonString());
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
            return JsonConvert.DeserializeObject<TTeamsObject>(jsonString, DESERIALIZER_SETTINGS);
        }

    }

}
