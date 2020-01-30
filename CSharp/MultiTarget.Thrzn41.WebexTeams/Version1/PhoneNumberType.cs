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

namespace Thrzn41.WebexTeams.Version1
{
    /// <summary>
    /// PhoneNumber type of Cisco Webex Teams.
    /// </summary>
    public class PhoneNumberType
    {

        /// <summary>
        /// Phone number is for work.
        /// </summary>
        public static readonly PhoneNumberType Work = new PhoneNumberType("work");


        /// <summary>
        /// Dictionary for phone number type.
        /// </summary>
        private static readonly Dictionary<string, PhoneNumberType> PHONE_NUMBER_TYPES;

        /// <summary>
        /// Static constuctor.
        /// </summary>
        static PhoneNumberType()
        {
            PHONE_NUMBER_TYPES = new Dictionary<string, PhoneNumberType>();

            PHONE_NUMBER_TYPES.Add(Work.Name, Work);
        }


        /// <summary>
        /// Name of the phone number type.
        /// </summary>
        public string Name { get; private set; }




        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name of the phone number type.</param>
        private PhoneNumberType(string name)
        {
            this.Name = name;
        }


        /// <summary>
        /// Parse phone number type.
        /// </summary>
        /// <param name="name">Name of the phone number type.</param>
        /// <returns><see cref="PhoneNumberType"/> for the name.</returns>
        public static PhoneNumberType Parse(string name)
        {
            PhoneNumberType spaceType = null;

            if ( name == null || !PHONE_NUMBER_TYPES.TryGetValue(name, out spaceType) )
            {
                spaceType = new PhoneNumberType(name);
            }

            return spaceType;
        }


        /// <summary>
        /// Determines whether this instance and another specified <see cref="PhoneNumberType"/> object have the same value.
        /// </summary>
        /// <param name="value">The phone number type to compare to this instance.</param>
        /// <returns>true if the value of the parameter is the same as the value of this instance; otherwise, false. If value is null, the method returns false.</returns>
        public bool Equals(PhoneNumberType value)
        {
            if( Object.ReferenceEquals(value, null) )
            {
                return false;
            }

            if( Object.ReferenceEquals(this, value) )
            {
                return true;
            }

            return (this.Name == value.Name);
        }

        /// <summary>
        /// Determines whether this instance and a specified object, which must also be a <see cref="PhoneNumberType"/> object, have the same value.
        /// </summary>
        /// <param name="obj">The phone number type to compare to this instance.</param>
        /// <returns>true if obj is a <see cref="PhoneNumberType"/> and its value is the same as this instance; otherwise, false. If obj is null, the method returns false.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as PhoneNumberType);
        }

        /// <summary>
        /// Returns the hash code for this phone number type.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }


        /// <summary>
        /// Determines whether two specified phone number types have the same value.
        /// </summary>
        /// <param name="lhs">Left hand side value.</param>
        /// <param name="rhs">Right hand side value.</param>
        /// <returns>true if the two values have the same value.</returns>
        public static bool operator ==(PhoneNumberType lhs, PhoneNumberType rhs)
        {
            if( Object.ReferenceEquals(lhs, null) )
            {
                if( Object.ReferenceEquals(rhs, null) )
                {
                    return true;
                }

                return false;
            }

            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Determines whether two specified phone number types have the different value.
        /// </summary>
        /// <param name="lhs">Left hand side value.</param>
        /// <param name="rhs">Right hand side value.</param>
        /// <returns>true if the two values have the different value.</returns>
        public static bool operator !=(PhoneNumberType lhs, PhoneNumberType rhs)
        {
            return !(lhs == rhs);
        }

    }

}
