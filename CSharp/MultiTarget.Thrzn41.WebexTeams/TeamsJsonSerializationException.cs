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
    /// Exception occurs on serializing to Json.
    /// </summary>
    public class TeamsJsonSerializationException : TeamsException
    {

        /// <summary>
        /// Line Number.
        /// </summary>
        public int LineNumber { get; private set; }

        /// <summary>
        /// Line Position.
        /// </summary>
        public int LinePosition { get; private set; }

        /// <summary>
        /// Path.
        /// </summary>
        public string Path { get; private set; }


        /// <summary>
        /// Create <see cref="TeamsJsonSerializationException"/>.
        /// </summary>
        /// <param name="lineNumber">Line number the error occured.</param>
        /// <param name="linePosition">Position in the Line the error occured.</param>
        /// <param name="path">Path to the Json the error occured.</param>
        public TeamsJsonSerializationException(int lineNumber, int linePosition, string path)
            : base(String.Format(ResourceMessage.ErrorMessages.TeamsJsonSerializationError, lineNumber, linePosition, path))
        {
            this.LineNumber   = lineNumber;
            this.LinePosition = linePosition;
            this.Path         = path;
        }

        /// <summary>
        /// Create <see cref="TeamsJsonSerializationException"/>.
        /// </summary>
        /// <param name="path">Path to the Json the error occured.</param>
        public TeamsJsonSerializationException(string path)
            : this(0, 0, path)
        {
        }

    }

}
