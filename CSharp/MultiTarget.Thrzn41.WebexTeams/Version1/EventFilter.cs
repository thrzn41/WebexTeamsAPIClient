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
    /// Event filter.
    /// </summary>
    public class EventFilter
    {

        /// <summary>
        /// Event filter key.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Event filter value.
        /// </summary>
        public string Value { get; private set; }




        /// <summary>
        /// Creates an event filter.
        /// </summary>
        /// <param name="key">Filter key.</param>
        /// <param name="value">Filter value.</param>
        public EventFilter(string key, string value)
        {
            this.Key   = key;
            this.Value = value;
        }


        #region Specific Filters

        /// <summary>
        /// Event filter for space.
        /// </summary>
        public class SpaceFilter : EventFilter
        {

            /// <summary>
            /// Create an event filter for space.
            /// </summary>
            /// <param name="spaceId">Space id to be filtered.</param>
            public SpaceFilter(string spaceId)
                : base("roomId", spaceId)
            {
            }

            /// <summary>
            /// Create an event filter for space.
            /// </summary>
            /// <param name="space"><see cref="Space"/> to be filtered.</param>
            public SpaceFilter(Space space)
                : base("roomId", space.Id)
            {
            }

        }

        /// <summary>
        /// Event filter for message.
        /// </summary>
        public class MessageFilter : EventFilter
        {

            /// <summary>
            /// Create an event filter for message.
            /// </summary>
            /// <param name="messageId">Message id to be filtered.</param>
            public MessageFilter(string messageId)
                : base("messageId", messageId)
            {
            }

            /// <summary>
            /// Create an event filter for space.
            /// </summary>
            /// <param name="message"><see cref="Message"/> to be filtered.</param>
            public MessageFilter(Message message)
                : base("messageId", message.Id)
            {
            }

        }

        /// <summary>
        /// Event filter for space type.
        /// </summary>
        public class SpaceTypeFilter : EventFilter
        {

            /// <summary>
            /// Create an event filter for space type.
            /// </summary>
            /// <param name="spaceType">Space type to be filtered.</param>
            public SpaceTypeFilter(SpaceType spaceType)
                : base("roomType", spaceType.Name)
            {
            }

        }
        
        /// <summary>
        /// Event filter for person.
        /// </summary>
        public class PersonFilter : EventFilter
        {

            /// <summary>
            /// Create an event filter for person.
            /// </summary>
            /// <param name="personIdOrEmail">Person id or email to be filtered.</param>
            /// <param name="personIdType"><see cref="PersonIdType"/> for personIdOrEmail filter.</param>
            public PersonFilter(string personIdOrEmail, PersonIdType personIdType = PersonIdType.Detect)
                : base("personId", personIdOrEmail)
            {
                personIdType = TeamsAPIClient.DetectPersonIdType(personIdOrEmail, personIdType);

                if(personIdType == PersonIdType.Email)
                {
                    this.Key = "personEmail";
                }
            }

            /// <summary>
            /// Create an event filter for person.
            /// </summary>
            /// <param name="person"><see cref="Person"/> to be filtered.</param>
            public PersonFilter(Person person)
                : this(person.Id, PersonIdType.Id)
            {
            }

        }

        /// <summary>
        /// Event filter for space membership moderator mode.
        /// </summary>
        public class SpaceMembershipModeratorModeFilter : EventFilter
        {

            /// <summary>
            /// Create an event filter for space membership moderator mode.
            /// </summary>
            /// <param name="isModerator">Space membership moderator mode to be filtered.</param>
            public SpaceMembershipModeratorModeFilter(bool isModerator)
                : base("isModerator", isModerator.ToString().ToLower())
            {
            }

        }

        /// <summary>
        /// Event filter for space lock mode.
        /// </summary>
        public class SpaceLockModeFilter : EventFilter
        {

            /// <summary>
            /// Create an event filter for space lock mode.
            /// </summary>
            /// <param name="isLocked">Space lock mode to be filtered.</param>
            public SpaceLockModeFilter(bool isLocked)
                : base("isLocked", isLocked.ToString().ToLower())
            {
            }

        }

        /// <summary>
        /// Event filter for mentioned people.
        /// </summary>
        public class MentionedPeopleFilter : EventFilter
        {

            /// <summary>
            /// Create an event filter for mentioned people.
            /// </summary>
            /// <param name="mentionedPeople">Mentioned people to be filtered.</param>
            public MentionedPeopleFilter(IEnumerable<string> mentionedPeople)
                : base("mentionedPeople", TeamsAPIClient.BuildCommaSeparatedString(mentionedPeople))
            {
            }

            /// <summary>
            /// Create an event filter for mentioned people.
            /// </summary>
            /// <param name="mentionedPerson">Mentioned person to be filtered.</param>
            public MentionedPeopleFilter(string mentionedPerson)
                : this(new []{ mentionedPerson })
            {
            }

        }

        /// <summary>
        /// Event filter for files.
        /// </summary>
        public class FilesFilter : EventFilter
        {

            /// <summary>
            /// Create an event filter for files.
            /// </summary>
            /// <param name="hasFiles">Has files or not to be filtered.</param>
            public FilesFilter(bool hasFiles)
                : base("hasFiles", hasFiles.ToString().ToLower())
            {
            }

        }

        #endregion

    }
}
