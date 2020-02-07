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

namespace Thrzn41.WebexTeams.Version1
{

    /// <summary>
    /// Teams v1 list object.
    /// </summary>
    /// <typeparam name="TTeamsObject">Teams Object that is contained in the list.</typeparam>
    [JsonObject(MemberSerialization.OptIn)]
    public class TeamsListData<TTeamsObject> : TeamsListObject<TTeamsObject>
        where TTeamsObject : TeamsObject, new()
    {
        /// <summary>
        /// Item list.
        /// </summary>
        [JsonProperty(PropertyName = "items")]
        public TTeamsObject[] Items { get; set; }

        /// <summary>
        /// Indicates the object contains items or not.
        /// </summary>
        [JsonIgnore]
        public override bool HasItems
        {
            get
            {
                return (this.Items != null && this.Items.Length > 0);
            }
        }

        /// <summary>
        /// Item count.
        /// </summary>
        [JsonIgnore]
        public override int ItemCount
        {
            get
            {
                return ((this.Items != null) ? this.Items.Length : 0);
            }
        }

        /// <summary>
        /// Indexer.
        /// </summary>
        /// <param name="index">The index of the items.</param>
        /// <returns>The item of this list.</returns>
        [JsonIgnore]
        public override TTeamsObject this[int index]
        {
            get
            {
                return this.Items[index];
            }
        }

        /// <summary>
        /// Indicates whether the object has error or not.
        /// </summary>
        [JsonIgnore]
        public override bool HasErrors
        {
            get
            {
                return TeamsData.CheckHasErrors(this.JsonExtensionData);
            }
        }

        /// <summary>
        /// Gets <see cref="IEnumerable{TTeamsObject}"/>.
        /// </summary>
        /// <returns><see cref="IEnumerable{TTeamsObject}"/>.</returns>
        public override IEnumerator<TTeamsObject> GetEnumerator()
        {
            for (int i = 0; i < this.ItemCount; i++)
            {
                yield return this.Items[i];
            }
        }


        /// <summary>
        /// Get error message.
        /// </summary>
        /// <returns>Error message.</returns>
        /// <exception cref="TeamsJsonSerializationException">Throws on serialization error.</exception>
        public override string GetErrorMessage()
        {
            return (TeamsData.GetErrorMessage(this.JsonExtensionData) ?? base.GetErrorMessage());
        }

        /// <summary>
        /// Gets Errors.
        /// </summary>
        /// <returns>Errors or null.</returns>
        /// <exception cref="TeamsJsonSerializationException">Throws on serialization error.</exception>
        public ErrorData[] GetErrors()
        {
            return TeamsData.GetErrors(this.JsonExtensionData);
        }

    }

}
