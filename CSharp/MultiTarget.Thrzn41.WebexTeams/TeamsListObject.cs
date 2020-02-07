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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Thrzn41.WebexTeams
{

    /// <summary>
    /// Base Object for all Cisco Webex Teams list objects.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class TeamsListObject<TTeamsObject> : TeamsObject, IEnumerable<TTeamsObject>
        where TTeamsObject : TeamsObject, new()
    {

        /// <summary>
        /// Indexer.
        /// </summary>
        /// <param name="index">The index of the items.</param>
        /// <returns>The item of this list.</returns>
        public abstract TTeamsObject this[int index]
        {
            get;
        }

        /// <summary>
        /// Gets the item count.
        /// </summary>
        public abstract int ItemCount { get; }

        /// <summary>
        /// Indicates the object contains items or not.
        /// </summary>
        public abstract bool HasItems { get; }

        /// <summary>
        /// Gets <see cref="IEnumerable{TTeamsObject}"/>.
        /// </summary>
        /// <returns><see cref="IEnumerable{TTeamsObject}"/>.</returns>
        public abstract IEnumerator<TTeamsObject> GetEnumerator();

        /// <summary>
        /// Gets <see cref="IEnumerable"/>.
        /// </summary>
        /// <returns><see cref="IEnumerable"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

    }
}
