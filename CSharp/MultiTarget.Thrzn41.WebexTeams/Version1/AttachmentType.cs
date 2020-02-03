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
    /// Attachment type of Cisco Webex Teams.
    /// </summary>
    public class AttachmentType
    {

        /// <summary>
        /// Adaptive Card.
        /// </summary>
        public static readonly AttachmentType AdaptiveCard = new AttachmentType("application/vnd.microsoft.card.adaptive");


        /// <summary>
        /// Dictionary for attachment type.
        /// </summary>
        private static readonly Dictionary<string, AttachmentType> ATTACHMENT_TYPES;

        /// <summary>
        /// Static constuctor.
        /// </summary>
        static AttachmentType()
        {
            ATTACHMENT_TYPES = new Dictionary<string, AttachmentType>();

            ATTACHMENT_TYPES.Add(AdaptiveCard.Name, AdaptiveCard);
        }


        /// <summary>
        /// Name of the attachment type.
        /// </summary>
        public string Name { get; private set; }




        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name of the attachment type.</param>
        private AttachmentType(string name)
        {
            this.Name = name;
        }


        /// <summary>
        /// Parse attachment type.
        /// </summary>
        /// <param name="name">Name of the attachment type.</param>
        /// <returns><see cref="AttachmentType"/> for the name.</returns>
        public static AttachmentType Parse(string name)
        {
            AttachmentType attachmentType = null;

            if (name == null || !ATTACHMENT_TYPES.TryGetValue(name, out attachmentType))
            {
                attachmentType = new AttachmentType(name);
            }

            return attachmentType;
        }


        /// <summary>
        /// Determines whether this instance and another specified <see cref="AttachmentType"/> object have the same value.
        /// </summary>
        /// <param name="value">The attachment type to compare to this instance.</param>
        /// <returns>true if the value of the parameter is the same as the value of this instance; otherwise, false. If value is null, the method returns false.</returns>
        public bool Equals(AttachmentType value)
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
        /// Determines whether this instance and a specified object, which must also be a <see cref="AttachmentType"/> object, have the same value.
        /// </summary>
        /// <param name="obj">The attachment type to compare to this instance.</param>
        /// <returns>true if obj is a <see cref="AttachmentType"/> and its value is the same as this instance; otherwise, false. If obj is null, the method returns false.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as AttachmentType);
        }

        /// <summary>
        /// Returns the hash code for this attachment type.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }


        /// <summary>
        /// Determines whether two specified attachment types have the same value.
        /// </summary>
        /// <param name="lhs">Left hand side value.</param>
        /// <param name="rhs">Right hand side value.</param>
        /// <returns>true if the two values have the same value.</returns>
        public static bool operator ==(AttachmentType lhs, AttachmentType rhs)
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
        /// Determines whether two specified attachment types have the different value.
        /// </summary>
        /// <param name="lhs">Left hand side value.</param>
        /// <param name="rhs">Right hand side value.</param>
        /// <returns>true if the two values have the different value.</returns>
        public static bool operator !=(AttachmentType lhs, AttachmentType rhs)
        {
            return !(lhs == rhs);
        }

    }
}
