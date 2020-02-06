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
                if( !this.HasExtensionData )
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
        /// <exception cref="TeamsKeyNotFoundException">Throws when the key is not found.</exception>
        /// <exception cref="TeamsJsonSerializationException">Throws on serialization error.</exception>
        public T GetExtensionData<T>(string key)
        {
            if(key == null || !this.HasExtensionData || !this.JsonExtensionData.ContainsKey(key))
            {
                throw new TeamsKeyNotFoundException(key);
            }

            T result;

            try
            {
                result = this.JsonExtensionData[key].ToObject<T>(this.JsonConverter.Deserializer);
            }
            catch(JsonReaderException jre)
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
        /// Gets Json extension data.
        /// </summary>
        /// <param name="key">The key of Json object.</param>
        /// <returns>Json extension data.</returns>
        /// <exception cref="TeamsKeyNotFoundException">Throws when the key is not found.</exception>
        /// <exception cref="TeamsJsonSerializationException">Throws on serialization error.</exception>
        public string GetExtensionJsonString(string key)
        {
            if (key == null || !this.HasExtensionData || !this.JsonExtensionData.ContainsKey(key))
            {
                throw new TeamsKeyNotFoundException(key);
            }

            string result = null;

            try
            {
                result = this.JsonExtensionData[key].ToString(Formatting.None);
            }
            catch (JsonWriterException jwe)
            {
                throw new TeamsJsonSerializationException(TeamsSerializationOperation.Serialize, jwe.Path);
            }
            catch (JsonSerializationException jse)
            {
                throw new TeamsJsonSerializationException(TeamsSerializationOperation.Serialize, jse.LineNumber, jse.LinePosition, jse.Path);
            }

            return result;
        }

        /// <summary>
        /// Gets Json extension data dictionary.
        /// </summary>
        /// <returns>Json extension data dictionary.</returns>
        /// <exception cref="TeamsJsonSerializationException">Throws on serialization error.</exception>
        public Dictionary<string, string> GetExtensionJsonStrings()
        {
            var result = new Dictionary<string, string>();

            if (this.HasExtensionData)
            {
                try
                {
                    foreach (var item in this.JsonExtensionData.Keys)
                    {
                        result.Add(item, this.JsonExtensionData[item].ToString(Formatting.None));
                    }
                }
                catch (JsonWriterException jwe)
                {
                    throw new TeamsJsonSerializationException(TeamsSerializationOperation.Serialize, jwe.Path);
                }
                catch (JsonSerializationException jse)
                {
                    throw new TeamsJsonSerializationException(TeamsSerializationOperation.Serialize, jse.LineNumber, jse.LinePosition, jse.Path);
                }
            }

            return result;
        }

    }

}
