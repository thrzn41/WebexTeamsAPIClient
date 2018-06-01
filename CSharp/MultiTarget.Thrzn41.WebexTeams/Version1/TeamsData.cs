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
                return (this.JsonExtensionData != null && this.JsonExtensionData.ContainsKey("errors"));
            }
        }


        /// <summary>
        /// Get error message.
        /// </summary>
        /// <returns>Error message.</returns>
        public override string GetErrorMessage()
        {
            if(this.HasErrors)
            {
                var errors = GetErrors();

                if(errors != null && errors.Length > 0)
                {
                    int?   errorCode   = errors[0].ErrorCode;
                    string description = errors[0].Description;

                    string message = null;

                    if(errorCode.HasValue)
                    {
                        message = String.Format(ErrorMessages.Version1TeamsResultErrorWithCode, description, errorCode.Value);
                    }
                    else if( !String.IsNullOrEmpty(description) )
                    {
                        message = String.Format(ErrorMessages.Version1TeamsResultError, description);
                    }

                    if(message != null)
                    {
                        return message;
                    }
                }
            }

            return base.GetErrorMessage();
        }


        /// <summary>
        /// Gets Partial Errors.
        /// </summary>
        /// <returns>Partial Errors.</returns>
        public Dictionary<string, PartialErrorData> GetPartialErrors()
        {
            var result = new Dictionary<string, PartialErrorData>();

            if(this.HasErrors)
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

            return result;
        }


        /// <summary>
        /// Gets Errors.
        /// </summary>
        /// <returns>Errors or null.</returns>
        public ErrorData[] GetErrors()
        {
            ErrorData[] result = null;

            if (this.HasErrors)
            {
                var jtoken = this.JsonExtensionData["errors"];

                if (jtoken.Type == Newtonsoft.Json.Linq.JTokenType.Array)
                {
                    result = this.JsonExtensionData["errors"].ToObject<ErrorData[]>();
                }

                if(result == null)
                {
                    result = new ErrorData[0];
                }
            }

            return result;
        }

    }

}
