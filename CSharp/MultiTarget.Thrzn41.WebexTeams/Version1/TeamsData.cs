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
using Thrzn41.WebexTeams.ResourceMessage;

namespace Thrzn41.WebexTeams.Version1
{

    /// <summary>
    /// Base Object for all Cisco Webex Teams v1 API objects.
    /// Some v1 specific features will be implemented.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class TeamsData : Thrzn41.WebexTeams.TeamsObject
    {

        /// <summary>
        /// Indicates whether the object has error or not.
        /// </summary>
        [JsonIgnore]
        public override bool HasErrors
        {
            get
            {
                return CheckHasErrors(this.JsonExtensionData);
            }
        }


        /// <summary>
        /// Indicates whether the object has error or not.
        /// </summary>
        /// <param name="jsonExtensionData">JsonExtensionData.</param>
        /// <returns>true if the data has errors.</returns>
        internal static bool CheckHasErrors(IDictionary<string, JToken> jsonExtensionData)
        {
            return (jsonExtensionData != null && jsonExtensionData.ContainsKey("errors"));
        }

        /// <summary>
        /// Gets Errors.
        /// </summary>
        /// <param name="jsonExtensionData">JsonExtensionData.</param>
        /// <returns>Errors or null.</returns>
        /// <exception cref="TeamsJsonSerializationException">Throws on serialization error.</exception>
        internal static ErrorData[] GetErrors(IDictionary<string, JToken> jsonExtensionData)
        {
            ErrorData[] result = null;

            if (CheckHasErrors(jsonExtensionData))
            {
                try
                {
                    var jtoken = jsonExtensionData["errors"];

                    if (jtoken.Type == Newtonsoft.Json.Linq.JTokenType.Array)
                    {
                        result = jsonExtensionData["errors"].ToObject<ErrorData[]>();
                    }

                    if (result == null)
                    {
                        result = new ErrorData[0];
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

            return result;
        }

        /// <summary>
        /// Get error message.
        /// </summary>
        /// <param name="jsonExtensionData">JsonExtensionData.</param>
        /// <returns>Error message.</returns>
        /// <exception cref="TeamsJsonSerializationException">Throws on serialization error.</exception>
        internal static string GetErrorMessage(IDictionary<string, JToken> jsonExtensionData)
        {
            if (CheckHasErrors(jsonExtensionData))
            {
                var errors = GetErrors(jsonExtensionData);

                if (errors != null && errors.Length > 0)
                {
                    int? errorCode     = errors[0].ErrorCode;
                    string description = errors[0].Description;

                    string message = null;

                    if (errorCode.HasValue)
                    {
                        message = String.Format(ErrorMessages.Version1TeamsResultErrorWithCode, description, errorCode.Value);
                    }
                    else if (!String.IsNullOrEmpty(description))
                    {
                        message = String.Format(ErrorMessages.Version1TeamsResultError, description);
                    }

                    if (message != null)
                    {
                        return message;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get error message.
        /// </summary>
        /// <returns>Error message.</returns>
        /// <exception cref="TeamsJsonSerializationException">Throws on serialization error.</exception>
        public override string GetErrorMessage()
        {
            return (GetErrorMessage(this.JsonExtensionData) ?? base.GetErrorMessage());
        }


        /// <summary>
        /// Gets Partial Errors.
        /// </summary>
        /// <returns>Partial Errors.</returns>
        /// <exception cref="TeamsJsonSerializationException">Throws on serialization error.</exception>
        public Dictionary<string, PartialErrorData> GetPartialErrors()
        {
            var result = new Dictionary<string, PartialErrorData>();

            if(this.HasErrors)
            {
                try
                {
                    var jtoken = this.JsonExtensionData["errors"];

                    if (jtoken.Type == Newtonsoft.Json.Linq.JTokenType.Object)
                    {
                        var data = this.JsonExtensionData["errors"].ToObject<TeamsObject>();

                        if (data.HasExtensionData)
                        {
                            foreach (var key in data.JsonExtensionData.Keys)
                            {
                                var value = data.JsonExtensionData[key].ToObject<PartialErrorData>();

                                if (value.CodeName != null)
                                {
                                    result.Add(key, value);
                                }
                            }
                        }
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

            return result;
        }


        /// <summary>
        /// Gets Errors.
        /// </summary>
        /// <returns>Errors or null.</returns>
        /// <exception cref="TeamsJsonSerializationException">Throws on serialization error.</exception>
        public ErrorData[] GetErrors()
        {
            return GetErrors(this.JsonExtensionData);
        }

    }

}
