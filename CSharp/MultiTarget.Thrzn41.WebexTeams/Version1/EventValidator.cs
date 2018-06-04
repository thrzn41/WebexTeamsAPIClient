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
using System;
using System.Collections.Generic;
using System.Text;
using Thrzn41.Util;

namespace Thrzn41.WebexTeams.Version1
{

    /// <summary>
    /// Event validator.
    /// </summary>
    public class EventValidator : IDisposable
    {




        /// <summary>
        /// The Hash string to validate event data.
        /// </summary>
        private HashString hashString = null;




        /// <summary>
        /// Constuctor.
        /// </summary>
        private EventValidator()
        {
        }


        /// <summary>
        /// Creates EventValidator instance.
        /// </summary>
        /// <param name="secret">Secret string.</param>
        /// <returns>EventValidator instance.</returns>
        internal static EventValidator Create(string secret)
        {
            var validator = new EventValidator();

            if( !String.IsNullOrEmpty(secret) )
            {
                validator.hashString = HashString.CreateHMACSHA1(secret);
            }

            return validator;
        }


        /// <summary>
        /// Validates event data.
        /// </summary>
        /// <param name="data">A data to be validated.</param>
        /// <param name="xTeamsSignature">X-Spark-Signature header value that is notified on event.</param>
        /// <returns>true if the event is valid, false if the event is invalid.</returns>
        public bool Validate(byte[] data, string xTeamsSignature)
        {
            // Default result is false.
            bool result = false;

            if( this.hashString != null && data != null && data.Length > 0 && !String.IsNullOrEmpty(xTeamsSignature) )
            {
                result = ( xTeamsSignature.ToLower() == hashString.ComputeString(data) );
            }

            return result;
        }




        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposing">disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    using (this.hashString)
                    {
                        // Disposed.
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~EventValidator() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }

}
