using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Thrzn41.WebexTeams
{

    /// <summary>
    /// Teams Extension object.
    /// This object will be used to get data that is not available in API Client currently.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class TeamsExtensionObject : TeamsObject
    {

        /// <summary>
        /// Extension data key list.
        /// </summary>
        [JsonIgnore]
        public ICollection<string> Keys
        {
            get
            {
                if( !this.HasExtensionData )
                {
                    return null;
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
            if( key == null || !this.HasExtensionData )
            {
                return false;
            }

            return this.JsonExtensionData.ContainsKey(key);
        }


        /// <summary>
        /// Gets Json extension data.
        /// </summary>
        /// <typeparam name="T">Type of result.</typeparam>
        /// <param name="key">The key of Json object.</param>
        /// <returns>Json extension data.</returns>
        public T GetExtensionData<T>(string key)
        {
            T result = default(T);

            if (this.HasExtensionData && this.JsonExtensionData.ContainsKey(key))
            {
                result = this.JsonExtensionData[key].ToObject<T>();
            }

            return result;
        }

        /// <summary>
        /// Gets Json extension data.
        /// </summary>
        /// <param name="key">The key of Json object.</param>
        /// <returns>Json extension data.</returns>
        public string GetExtensionJsonString(string key)
        {
            string result = null;

            if (this.HasExtensionData && this.JsonExtensionData.ContainsKey(key))
            {
                result = this.JsonExtensionData[key].ToString(Formatting.None);
            }

            return result;
        }

        /// <summary>
        /// Gets Json extension data dictionary.
        /// </summary>
        /// <returns>Json extension data dictionary.</returns>
        public Dictionary<string, string> GetExtensionJsonStrings()
        {
            var result = new Dictionary<string, string>();

            if (this.HasExtensionData)
            {
                foreach (var item in this.JsonExtensionData.Keys)
                {
                    result.Add(item, this.JsonExtensionData[item].ToString(Formatting.None));
                }
            }

            return result;
        }

    }

}
