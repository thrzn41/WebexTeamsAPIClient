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
using System.Globalization;
using System.IO;
using System.Text;

namespace Thrzn41.WebexTeams
{

    /// <summary>
    /// Converter to serialize or deserialize to/from Json.
    /// </summary>
    public class TeamsJsonObjectConverter : TeamsJsonConverter
    {

        /// <summary>
        /// Default capacity for <see cref="StringBuilder"/>.
        /// </summary>
        private const int STRING_BUILDER_INITIAL_CAPACITY = 256;

        /// <summary>
        /// Default Settings for Json serializer.
        /// </summary>
        private static readonly JsonSerializerSettings DEFAULT_SERIALIZER_SETTINGS = new JsonSerializerSettings
        {
            DateFormatString  = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffK",
            Formatting        = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore,
        };

        /// <summary>
        /// Default Settings for Json deserializer.
        /// </summary>
        private static readonly JsonSerializerSettings DEFAULT_DESERIALIZER_SETTINGS = new JsonSerializerSettings
        {
            DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK",
        };

        /// <summary>
        /// Default serializer.
        /// </summary>
        internal static readonly JsonSerializer DEFAULT_SERIALIZER;

        /// <summary>
        /// Default deserializer.
        /// </summary>
        internal static readonly JsonSerializer DEFAULT_DESERIALIZER;


        /// <summary>
        /// Static constructor.
        /// </summary>
        static TeamsJsonObjectConverter()
        {
            DEFAULT_SERIALIZER   = JsonSerializer.CreateDefault(DEFAULT_SERIALIZER_SETTINGS);
            DEFAULT_DESERIALIZER = JsonSerializer.CreateDefault(DEFAULT_DESERIALIZER_SETTINGS);
        }

        /// <summary>
        /// Json serializer.
        /// </summary>
        private JsonSerializer serializer = null;

        /// <summary>
        /// Json deserializer.
        /// </summary>
        private JsonSerializer deserializer = null;

        /// <summary>
        /// Json serializer.
        /// </summary>
        internal JsonSerializer Serializer
        {
            get
            {
                return (this.serializer ?? DEFAULT_SERIALIZER);
            }
        }

        /// <summary>
        /// Json deserializer.
        /// </summary>
        internal JsonSerializer Deserializer
        {
            get
            {
                return (this.deserializer ?? DEFAULT_DESERIALIZER);
            }
        }


        /// <summary>
        /// Creates <see cref="TeamsJsonObjectConverter"/>.
        /// </summary>
        /// <param name="dateFormatStringForSerializer">Date format to serialize to Json.</param>
        /// <param name="dateFormatStringForDeserializer">Date format to deserialize from Json.</param>
        public TeamsJsonObjectConverter(string dateFormatStringForSerializer, string dateFormatStringForDeserializer)
        {
            if( !String.IsNullOrEmpty(dateFormatStringForSerializer) )
            {
                this.serializer = JsonSerializer.CreateDefault(new JsonSerializerSettings
                    {
                        DateFormatString  = dateFormatStringForSerializer,
                        Formatting        = Formatting.None,
                        NullValueHandling = NullValueHandling.Ignore,
                    });
            }

            if ( !String.IsNullOrEmpty(dateFormatStringForDeserializer) )
            {
                this.deserializer = JsonSerializer.CreateDefault(new JsonSerializerSettings
                    {
                        DateFormatString = dateFormatStringForDeserializer,
                    });
            }
        }

        /// <summary>
        /// Creates <see cref="TeamsJsonObjectConverter"/>.
        /// </summary>
        /// <param name="dateFormatStringForSerializer">Date format to serialize to Json.</param>
        public TeamsJsonObjectConverter(string dateFormatStringForSerializer)
            : this(dateFormatStringForSerializer, null)
        {
        }

        /// <summary>
        /// Creates <see cref="TeamsJsonObjectConverter"/>.
        /// </summary>
        public TeamsJsonObjectConverter()
        {
        }


        /// <summary>
        /// Serialize the object to Json string.
        /// </summary>
        /// <param name="obj">The object to be serialized.</param>
        /// <returns>The serialized Json string.</returns>
        /// <exception cref="TeamsJsonSerializationException">Throws on serialization error.</exception>
        public override string SerializeObject(object obj)
        {
            try
            {
                var sb = new StringBuilder(STRING_BUILDER_INITIAL_CAPACITY);

                using (var writer = new StringWriter(sb, CultureInfo.InvariantCulture))
                {
                    using(var jsonWriter = new JsonTextWriter(writer))
                    {
                        var serializer = this.Serializer;

                        jsonWriter.Formatting       = serializer.Formatting;
                        jsonWriter.DateFormatString = serializer.DateFormatString;

                        serializer.Serialize(jsonWriter, obj);
                    }

                    return writer.ToString();
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


        /// <summary>
        /// Deserialize the Json string to the object.
        /// </summary>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <param name="jsonString">The Json string to be deserialized.</param>
        /// <returns>The deserialized object.</returns>
        /// <exception cref="TeamsJsonSerializationException">Throws on deserialization error.</exception>
        public override T DeserializeObject<T>(string jsonString)
        {
            try
            {
                using (var reader     = new StringReader(jsonString))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    var deserializer = this.Deserializer;

                    jsonReader.DateFormatString = deserializer.DateFormatString;

                    return deserializer.Deserialize<T>(jsonReader);
                }
            }
            catch (JsonReaderException jre)
            {
                throw new TeamsJsonSerializationException(TeamsSerializationOperation.Deserialize, jre.LineNumber, jre.LinePosition, jre.Path);
            }
            catch (JsonSerializationException jse)
            {
                throw new TeamsJsonSerializationException(TeamsSerializationOperation.Deserialize, jse.LineNumber, jse.LinePosition, jse.Path);
            }
        }

    }
}
