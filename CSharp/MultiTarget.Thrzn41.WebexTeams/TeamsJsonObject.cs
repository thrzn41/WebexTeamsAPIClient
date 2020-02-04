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
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Thrzn41.WebexTeams
{

    /// <summary>
    /// Json object to use in Webex Teams API.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class TeamsJsonObject
    {

        /// <summary>
        /// Errors on deserializing.
        /// </summary>
        [JsonIgnore]
        private List<TeamsJsonSerializationException> serializationErrors = null;

        /// <summary>
        /// Check if there are errors on deserializing.
        /// </summary>
        [JsonIgnore]
        public bool HasSerializationErrors
        {
            get
            {
                return (this.serializationErrors != null && this.serializationErrors.Count > 0);
            }
        }

        /// <summary>
        /// Get errors on serializing or deserializing.
        /// </summary>
        [JsonIgnore]
        public TeamsJsonSerializationException[] SerializationErrors
        {
            get
            {
                return (this.serializationErrors?.ToArray());
            }
        }


        /// <summary>
        /// Error handler on serailizing or deserializing.
        /// </summary>
        /// <param name="context">Streaming context.</param>
        /// <param name="errorContext">Error context.</param>
        [OnError]
        internal void OnJsonConvertError(StreamingContext context, ErrorContext errorContext)
        {
            var e = errorContext.Error as JsonReaderException;

            if(e != null)
            {
                if(this.serializationErrors == null)
                {
                    this.serializationErrors = new List<TeamsJsonSerializationException>();
                }

                this.serializationErrors.Add(new TeamsJsonSerializationException(TeamsSerializationOperation.Deserialize, e.LineNumber, e.LinePosition, e.Path));

                errorContext.Handled = true;
            }
        }

    }
}
