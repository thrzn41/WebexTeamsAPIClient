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

namespace Thrzn41.WebexTeams.Version1.GuestIssuer
{


    /// <summary>
    /// JWT Token Type.
    /// </summary>
    internal enum JWTTokenType
    {
        /// <summary>
        /// Token type is JWT.
        /// </summary>
        JWT
    };


    /// <summary>
    /// JWT Signature Algorithm.
    /// </summary>
    internal enum JWTSignatureAlgorithm
    {
        /// <summary>
        /// HMAC-SHA256.
        /// </summary>
        HMACSHA256
    };




    /// <summary>
    /// JWT Header.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    internal class JWTHeader : TeamsData
    {

        /// <summary>
        /// JWT Token Type.
        /// </summary>
        [JsonIgnore]
        public JWTTokenType Type { get; internal set; } = JWTTokenType.JWT;

        /// <summary>
        /// JWT Token type name.
        /// </summary>
        [JsonProperty(PropertyName = "typ")]
        public string TypeName { get; private set; } = "JWT";


        /// <summary>
        /// JWT Signature algorithm.
        /// </summary>
        [JsonIgnore]
        public JWTSignatureAlgorithm Algorithm { get; internal set; } = JWTSignatureAlgorithm.HMACSHA256;

        /// <summary>
        /// JWT Signature algorithm name.
        /// </summary>
        [JsonProperty(PropertyName = "alg")]
        public string AlgorithmName { get; private set; } = "HS256";

    }

}
