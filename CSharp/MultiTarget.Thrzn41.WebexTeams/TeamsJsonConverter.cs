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
using System;
using System.Collections.Generic;
using System.Text;

namespace Thrzn41.WebexTeams
{

    /// <summary>
    /// Json converter to serialize or deserialize to/from Json.
    /// </summary>
    public abstract class TeamsJsonConverter
    {


        /// <summary>
        /// Serialize the object to Json string.
        /// </summary>
        /// <param name="obj">The object to be serialized.</param>
        /// <returns>The serialized Json string.</returns>
        /// <exception cref="TeamsJsonSerializationException">Throws on serialization error.</exception>
        public abstract string SerializeObject(object obj);


        /// <summary>
        /// Deserialize the Json string to the object.
        /// </summary>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <param name="jsonString">The Json string to be deserialized.</param>
        /// <returns>The deserialized object.</returns>
        /// <exception cref="TeamsJsonDeserializationException">Throws on deserialization error.</exception>
        public abstract T DeserializeObject<T>(string jsonString);

    }

}
