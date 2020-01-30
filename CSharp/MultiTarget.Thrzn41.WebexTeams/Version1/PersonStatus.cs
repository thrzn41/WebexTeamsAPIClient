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
    /// Person status of Cisco Webex Teams.
    /// </summary>
    public class PersonStatus
    {

        /// <summary>
        /// Person is active within the last 10 minutes.
        /// </summary>
        public static readonly PersonStatus Active = new PersonStatus("active");

        /// <summary>
        /// Person is in a call.
        /// </summary>
        public static readonly PersonStatus Call = new PersonStatus("call");

        /// <summary>
        /// Person has manually set their status to "Do Not Disturb".
        /// </summary>
        public static readonly PersonStatus DoNotDisturb = new PersonStatus("DoNotDisturb");

        /// <summary>
        /// Person's last activity occurred more than 10 minutes ago.
        /// </summary>
        public static readonly PersonStatus Inactive = new PersonStatus("inactive");

        /// <summary>
        /// Person is in a meeting.
        /// </summary>
        public static readonly PersonStatus Meeting = new PersonStatus("meeting");

        /// <summary>
        /// Person or a Hybrid Calendar service has indicated that they are "Out of Office".
        /// </summary>
        public static readonly PersonStatus OutOfOffice = new PersonStatus("OutOfOffice");

        /// <summary>
        /// Person has never logged in; a status cannot be determined.
        /// </summary>
        public static readonly PersonStatus Pending = new PersonStatus("pending");

        /// <summary>
        /// Person is sharing content.
        /// </summary>
        public static readonly PersonStatus Presenting = new PersonStatus("presenting");

        /// <summary>
        /// Person's status could not be determined.
        /// </summary>
        public static readonly PersonStatus Unknown = new PersonStatus("unknown");




        /// <summary>
        /// Dictionary for person status.
        /// </summary>
        private static readonly Dictionary<string, PersonStatus> PERSON_STATUSES;

        /// <summary>
        /// Static constuctor.
        /// </summary>
        static PersonStatus()
        {
            PERSON_STATUSES = new Dictionary<string, PersonStatus>();

            PERSON_STATUSES.Add(Active.Name,       Active);
            PERSON_STATUSES.Add(Call.Name,         Call);
            PERSON_STATUSES.Add(DoNotDisturb.Name, DoNotDisturb);
            PERSON_STATUSES.Add(Inactive.Name,     Inactive);
            PERSON_STATUSES.Add(Meeting.Name,      Meeting);
            PERSON_STATUSES.Add(OutOfOffice.Name,  OutOfOffice);
            PERSON_STATUSES.Add(Pending.Name,      Pending);
            PERSON_STATUSES.Add(Presenting.Name,   Presenting);
        }


        /// <summary>
        /// Name of the person status.
        /// </summary>
        public string Name { get; private set; }




        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name of the person status.</param>
        private PersonStatus(string name)
        {
            this.Name = name;
        }


        /// <summary>
        /// Parse person status.
        /// </summary>
        /// <param name="name">Name of the person status.</param>
        /// <returns><see cref="PersonStatus"/> for the name.</returns>
        public static PersonStatus Parse(string name)
        {
            PersonStatus personStatus = null;

            if(name == null)
            {
                personStatus = PersonStatus.Unknown;
            }
            else if ( !PERSON_STATUSES.TryGetValue(name, out personStatus) )
            {
                personStatus = new PersonStatus(name);
            }

            return personStatus;
        }


        /// <summary>
        /// Determines whether this instance and another specified <see cref="PersonStatus"/> object have the same value.
        /// </summary>
        /// <param name="value">The person status to compare to this instance.</param>
        /// <returns>true if the value of the parameter is the same as the value of this instance; otherwise, false. If value is null, the method returns false.</returns>
        public bool Equals(PersonStatus value)
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
        /// Determines whether this instance and a specified object, which must also be a <see cref="PersonStatus"/> object, have the same value.
        /// </summary>
        /// <param name="obj">The person status to compare to this instance.</param>
        /// <returns>true if obj is a <see cref="PersonStatus"/> and its value is the same as this instance; otherwise, false. If obj is null, the method returns false.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as PersonStatus);
        }

        /// <summary>
        /// Returns the hash code for this person status.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }


        /// <summary>
        /// Determines whether two specified person status have the same value.
        /// </summary>
        /// <param name="lhs">Left hand side value.</param>
        /// <param name="rhs">Right hand side value.</param>
        /// <returns>true if the two values have the same value.</returns>
        public static bool operator ==(PersonStatus lhs, PersonStatus rhs)
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
        /// Determines whether two specified person status have the different value.
        /// </summary>
        /// <param name="lhs">Left hand side value.</param>
        /// <param name="rhs">Right hand side value.</param>
        /// <returns>true if the two values have the different value.</returns>
        public static bool operator !=(PersonStatus lhs, PersonStatus rhs)
        {
            return !(lhs == rhs);
        }

    }

}
