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
    /// Event resource of Cisco Webex Teams.
    /// </summary>
    public class EventResource
    {

        /// <summary>
        /// All resources.
        /// </summary>
        public static readonly EventResource All = new EventResource("all");

        /// <summary>
        /// Space membership resource.
        /// </summary>
        public static readonly EventResource SpaceMembership = new EventResource("memberships");

        /// <summary>
        /// Message resource.
        /// </summary>
        public static readonly EventResource Message = new EventResource("messages");

        /// <summary>
        /// Space resource.
        /// </summary>
        public static readonly EventResource Space = new EventResource("rooms");

        /// <summary>
        /// Attachment Actions resource.
        /// </summary>
        public static readonly EventResource AttachmentActions = new EventResource("attachmentActions");



        /// <summary>
        /// Dictionary for event resource.
        /// </summary>
        private static readonly Dictionary<string, EventResource> EVENT_RESOURCES;

        /// <summary>
        /// Static constuctor.
        /// </summary>
        static EventResource()
        {
            EVENT_RESOURCES = new Dictionary<string, EventResource>();

            EVENT_RESOURCES.Add(All.Name,               All);
            EVENT_RESOURCES.Add(SpaceMembership.Name,   SpaceMembership);
            EVENT_RESOURCES.Add(Message.Name,           Message);
            EVENT_RESOURCES.Add(Space.Name,             Space);
            EVENT_RESOURCES.Add(AttachmentActions.Name, AttachmentActions);
        }


        /// <summary>
        /// Name of the event resource.
        /// </summary>
        public string Name { get; private set; }




        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name of the event resource.</param>
        private EventResource(string name)
        {
            this.Name = name;
        }


        /// <summary>
        /// Parse event resource.
        /// </summary>
        /// <param name="name">Name of the event resource.</param>
        /// <returns><see cref="EventResource"/> for the name.</returns>
        public static EventResource Parse(string name)
        {
            EventResource eventResource = null;

            if ( name == null || !EVENT_RESOURCES.TryGetValue(name, out eventResource) )
            {
                eventResource = new EventResource(name);
            }

            return eventResource;
        }


        /// <summary>
        /// Determines whether this instance and another specified <see cref="EventResource"/> object have the same value.
        /// </summary>
        /// <param name="value">The event resource to compare to this instance.</param>
        /// <returns>true if the value of the parameter is the same as the value of this instance; otherwise, false. If value is null, the method returns false.</returns>
        public bool Equals(EventResource value)
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
        /// Determines whether this instance and a specified object, which must also be a <see cref="EventResource"/> object, have the same value.
        /// </summary>
        /// <param name="obj">The event resource to compare to this instance.</param>
        /// <returns>true if obj is a <see cref="EventResource"/> and its value is the same as this instance; otherwise, false. If obj is null, the method returns false.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as EventResource);
        }

        /// <summary>
        /// Returns the hash code for this event resource.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }


        /// <summary>
        /// Determines whether two specified event resource have the same value.
        /// </summary>
        /// <param name="lhs">Left hand side value.</param>
        /// <param name="rhs">Right hand side value.</param>
        /// <returns>true if the two values have the same value.</returns>
        public static bool operator ==(EventResource lhs, EventResource rhs)
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
        /// Determines whether two specified event resource have the different value.
        /// </summary>
        /// <param name="lhs">Left hand side value.</param>
        /// <param name="rhs">Right hand side value.</param>
        /// <returns>true if the two values have the different value.</returns>
        public static bool operator !=(EventResource lhs, EventResource rhs)
        {
            return !(lhs == rhs);
        }

    }

}
