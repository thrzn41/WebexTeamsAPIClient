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

namespace Thrzn41.WebexTeams.Version1
{

    /// <summary>
    /// Event owner type of Cisco Webex Teams.
    /// </summary>
    public class EventOwnerType
    {

        /// <summary>
        /// The owner is creator.
        /// </summary>
        public static readonly EventOwnerType Creator = new EventOwnerType("creator");

        /// <summary>
        /// The owner is organizaiton.
        /// </summary>
        public static readonly EventOwnerType Organization = new EventOwnerType("org");




        /// <summary>
        /// Dictionary for event owner type.
        /// </summary>
        private static readonly Dictionary<string, EventOwnerType> EVENT_OWNER_TYPES;

        /// <summary>
        /// Static constuctor.
        /// </summary>
        static EventOwnerType()
        {
            EVENT_OWNER_TYPES = new Dictionary<string, EventOwnerType>();

            EVENT_OWNER_TYPES.Add(Creator.Name,      Creator);
            EVENT_OWNER_TYPES.Add(Organization.Name, Organization);
        }


        /// <summary>
        /// Name of the event type.
        /// </summary>
        public string Name { get; private set; }




        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name of the event type.</param>
        private EventOwnerType(string name)
        {
            this.Name = name;
        }


        /// <summary>
        /// Parse event owner type.
        /// </summary>
        /// <param name="name">Name of the event owner type.</param>
        /// <returns><see cref="EventOwnerType"/> for the name.</returns>
        public static EventOwnerType Parse(string name)
        {
            EventOwnerType eventOwnerType = null;

            if ( name == null || !EVENT_OWNER_TYPES.TryGetValue(name, out eventOwnerType) )
            {
                eventOwnerType = new EventOwnerType(name);
            }

            return eventOwnerType;
        }


        /// <summary>
        /// Determines whether this instance and another specified <see cref="EventOwnerType"/> object have the same value.
        /// </summary>
        /// <param name="value">The event owner type to compare to this instance.</param>
        /// <returns>true if the value of the parameter is the same as the value of this instance; otherwise, false. If value is null, the method returns false.</returns>
        public bool Equals(EventOwnerType value)
        {
            if ( Object.ReferenceEquals(value, null) )
            {
                return false;
            }

            if ( Object.ReferenceEquals(this, value) )
            {
                return true;
            }

            return (this.Name == value.Name);
        }

        /// <summary>
        /// Determines whether this instance and a specified object, which must also be a <see cref="EventOwnerType"/> object, have the same value.
        /// </summary>
        /// <param name="obj">The event owner type to compare to this instance.</param>
        /// <returns>true if obj is a <see cref="EventOwnerType"/> and its value is the same as this instance; otherwise, false. If obj is null, the method returns false.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as EventOwnerType);
        }

        /// <summary>
        /// Returns the hash code for this event owner type.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }


        /// <summary>
        /// Determines whether two specified event owner type have the same value.
        /// </summary>
        /// <param name="lhs">Left hand side value.</param>
        /// <param name="rhs">Right hand side value.</param>
        /// <returns>true if the two values have the same value.</returns>
        public static bool operator ==(EventOwnerType lhs, EventOwnerType rhs)
        {
            if ( Object.ReferenceEquals(lhs, null) )
            {
                if ( Object.ReferenceEquals(rhs, null) )
                {
                    return true;
                }

                return false;
            }

            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Determines whether two specified event owner type have the different value.
        /// </summary>
        /// <param name="lhs">Left hand side value.</param>
        /// <param name="rhs">Right hand side value.</param>
        /// <returns>true if the two values have the different value.</returns>
        public static bool operator !=(EventOwnerType lhs, EventOwnerType rhs)
        {
            return !(lhs == rhs);
        }

    }

}
