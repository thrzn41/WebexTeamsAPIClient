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
    /// Attachment Action type of Cisco Webex Teams.
    /// </summary>
    public class AttachmentActionType
    {
    	
        /// <summary>
        /// Adaptive Card.
        /// </summary>
        public static readonly AttachmentActionType Submit = new AttachmentActionType("submit");


        /// <summary>
        /// Dictionary for attachment action type.
        /// </summary>
        private static readonly Dictionary<string, AttachmentActionType> ATTACHMENT_ACTION_TYPES;

        /// <summary>
        /// Static constuctor.
        /// </summary>
        static AttachmentActionType()
        {
            ATTACHMENT_ACTION_TYPES = new Dictionary<string, AttachmentActionType>();

            ATTACHMENT_ACTION_TYPES.Add(Submit.Name, Submit);
        }


        /// <summary>
        /// Name of the attachment action type.
        /// </summary>
        public string Name { get; private set; }




        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name of the attachment action type.</param>
        private AttachmentActionType(string name)
        {
            this.Name = name;
        }


        /// <summary>
        /// Parse attachment action type.
        /// </summary>
        /// <param name="name">Name of the attachment action type.</param>
        /// <returns><see cref="AttachmentActionType"/> for the name.</returns>
        public static AttachmentActionType Parse(string name)
        {
            AttachmentActionType attachmentActionType = null;

            if (name == null || !ATTACHMENT_ACTION_TYPES.TryGetValue(name, out attachmentActionType))
            {
                attachmentActionType = new AttachmentActionType(name);
            }

            return attachmentActionType;
        }


        /// <summary>
        /// Determines whether this instance and another specified <see cref="AttachmentActionType"/> object have the same value.
        /// </summary>
        /// <param name="value">The attachment action type to compare to this instance.</param>
        /// <returns>true if the value of the parameter is the same as the value of this instance; otherwise, false. If value is null, the method returns false.</returns>
        public bool Equals(AttachmentActionType value)
        {
            if (Object.ReferenceEquals(value, null))
            {
                return false;
            }

            if (Object.ReferenceEquals(this, value))
            {
                return true;
            }

            return (this.Name == value.Name);
        }

        /// <summary>
        /// Determines whether this instance and a specified object, which must also be a <see cref="AttachmentActionType"/> object, have the same value.
        /// </summary>
        /// <param name="obj">The attachment action type to compare to this instance.</param>
        /// <returns>true if obj is a <see cref="AttachmentActionType"/> and its value is the same as this instance; otherwise, false. If obj is null, the method returns false.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as AttachmentActionType);
        }

        /// <summary>
        /// Returns the hash code for this attachment action type.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }


        /// <summary>
        /// Determines whether two specified attachment action types have the same value.
        /// </summary>
        /// <param name="lhs">Left hand side value.</param>
        /// <param name="rhs">Right hand side value.</param>
        /// <returns>true if the two values have the same value.</returns>
        public static bool operator ==(AttachmentActionType lhs, AttachmentActionType rhs)
        {
            if (Object.ReferenceEquals(lhs, null))
            {
                if (Object.ReferenceEquals(rhs, null))
                {
                    return true;
                }

                return false;
            }

            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Determines whether two specified attachment action types have the different value.
        /// </summary>
        /// <param name="lhs">Left hand side value.</param>
        /// <param name="rhs">Right hand side value.</param>
        /// <returns>true if the two values have the different value.</returns>
        public static bool operator !=(AttachmentActionType lhs, AttachmentActionType rhs)
        {
            return !(lhs == rhs);
        }
    }
}
