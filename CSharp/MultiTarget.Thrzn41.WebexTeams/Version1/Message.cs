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
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Thrzn41.WebexTeams.Version1
{

    /// <summary>
    /// Cisco Webex Teams Message object.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Message : TeamsData
    {

        /// <summary>
        /// Remove mentioned Tag pattern.
        /// </summary>
        [JsonIgnore]
        private readonly static Regex REMOVE_MENTIONED_TAG_PATTERN = new Regex("(<spark-mention.*?>.*?</spark-mention>)|(<.+?>)", RegexOptions.Compiled, TimeSpan.FromSeconds(60.0f));


        /// <summary>
        /// Id of the message.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; internal set; }

        /// <summary>
        /// Id of Teams space that this message exists in.
        /// </summary>
        [JsonProperty(PropertyName = "roomId")]
        public string SpaceId { get; internal set; }

        /// <summary>
        /// Type name of Teams space that this message exists in.
        /// </summary>
        [JsonProperty(PropertyName = "roomType")]
        public string SpaceTypeName { get; internal set; }

        /// <summary>
        /// Type of Teams space that this message exists in.
        /// </summary>
        [JsonIgnore]
        public SpaceType SpaceType
        {
            get
            {
                return SpaceType.Parse(this.SpaceTypeName);
            }
        }

        /// <summary>
        /// Id of person that this message to be posted.
        /// </summary>
        [JsonProperty(PropertyName = "toPersonId")]
        internal string ToPersonId { get; set; }

        /// <summary>
        /// Email of person that this message to be posted.
        /// </summary>
        [JsonProperty(PropertyName = "toPersonEmail")]
        internal string ToPersonEmail { get; set; }

        /// <summary>
        /// Text of the message.
        /// </summary>
        [JsonProperty(PropertyName = "text")]
        public string Text { get; internal set; }

        /// <summary>
        /// Indicates this message has text or not.
        /// </summary>
        [JsonIgnore]
        public bool HasText
        {
            get
            {
                return (this.Text != null);
            }
        }

        /// <summary>
        /// Try to get text without mentioned part.
        /// </summary>
        [JsonIgnore]
        public string TextWithoutMention
        {
            get
            {
                if(String.IsNullOrEmpty(this.Text) || (!this.HasMentionedPersons && !this.HasMentionedGroups) || !this.HasHtml)
                {
                    return this.Text;
                }

                return WebUtility.HtmlDecode(REMOVE_MENTIONED_TAG_PATTERN.Replace(this.Html, String.Empty).Trim());
            }
        }

        /// <summary>
        /// Markdonw text to be posted.
        /// </summary>
        [JsonProperty(PropertyName = "markdown")]
        internal string Markdown { get; set; }

        /// <summary>
        /// File list that are attached to the message.
        /// </summary>
        [JsonProperty(PropertyName = "files")]
        public string[] Files { get; internal set; }

        /// <summary>
        /// Indicates the message has files attached.
        /// </summary>
        [JsonIgnore]
        public bool HasFiles
        {
            get
            {
                return (this.Files != null && this.Files.Length > 0);
            }
        }

        /// <summary>
        /// Attached file count of the message.
        /// </summary>
        [JsonIgnore]
        public int FileCount
        {
            get
            {
                return ((this.Files != null) ? this.Files.Length : 0);
            }
        }

        /// <summary>
        /// File Uri list that are attached to the message.
        /// </summary>
        [JsonIgnore]
        public Uri[] FileUris
        {
            get
            {
                Uri[] result = null;

                if(this.HasFiles)
                {
                    string[] files = this.Files;

                    result = new Uri[files.Length];

                    for (int i = 0; i < result.Length; i++)
                    {
                        result[i] = new Uri(files[i]);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Id of person who owns the message.
        /// </summary>
        [JsonProperty(PropertyName = "personId")]
        public string PersonId { get; internal set; }

        /// <summary>
        /// Email of person who owns the message.
        /// </summary>
        [JsonProperty(PropertyName = "personEmail")]
        public string PersonEmail { get; internal set; }

        /// <summary>
        /// <see cref="DateTime"/> when the message was created.
        /// </summary>
        [JsonProperty(PropertyName = "created")]
        public DateTime? Created { get; internal set; }

        /// <summary>
        /// Id list of person who are mentioned in the message.
        /// </summary>
        [JsonProperty(PropertyName = "mentionedPeople")]
        public string[] MentionedPersons { get; internal set; }

        /// <summary>
        /// Id list of person who are mentioned in the message.
        /// </summary>
        [JsonIgnore]
        public string[] MentionedPeople {
            get
            {
                return this.MentionedPersons;
            }
        }

        /// <summary>
        /// Indicates the message has mentioned people or not.
        /// </summary>
        [JsonIgnore]
        public bool HasMentionedPersons
        {
            get
            {
                return (this.MentionedPersons != null && this.MentionedPersons.Length > 0);
            }
        }

        /// <summary>
        /// Indicates the message has mentioned people or not.
        /// </summary>
        [JsonIgnore]
        public bool HasMentionedPeople
        {
            get
            {
                return this.HasMentionedPersons;
            }
        }

        /// <summary>
        /// Mentioned person count.
        /// </summary>
        [JsonIgnore]
        public int MentionedPersonCount
        {
            get
            {
                return ((this.MentionedPersons != null) ? this.MentionedPersons.Length : 0);
            }
        }

        /// <summary>
        /// Name list of group which are mentioned in the message.
        /// </summary>
        [JsonProperty(PropertyName = "mentionedGroups")]
        public string[] MentionedGroupNames { get; internal set; }

        /// <summary>
        /// List of group which are mentioned in the message.
        /// </summary>
        [JsonIgnore]
        public MentionedGroup[] MentionedGroups
        {
            get
            {
                MentionedGroup[] result = null;

                if (this.HasMentionedGroups)
                {
                    string[] names = this.MentionedGroupNames;

                    result = new MentionedGroup[names.Length];

                    for (int i = 0; i < names.Length; i++)
                    {
                        result[i] = MentionedGroup.Parse(names[i]);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Indicates the message has mentioned group or not.
        /// </summary>
        [JsonIgnore]
        public bool HasMentionedGroups
        {
            get
            {
                return (this.MentionedGroupNames != null && this.MentionedGroupNames.Length > 0);
            }
        }

        /// <summary>
        /// Mentioned group count.
        /// </summary>
        [JsonIgnore]
        public int MentionedGroupCount
        {
            get
            {
                return ((this.MentionedGroupNames != null) ? this.MentionedGroupNames.Length : 0);
            }
        }

        /// <summary>
        /// Html text that is posted.
        /// </summary>
        [JsonProperty(PropertyName = "html")]
        public string Html { get; internal set; }

        /// <summary>
        /// Indicates this message has html or not.
        /// </summary>
        [JsonIgnore]
        public bool HasHtml
        {
            get
            {
                return !String.IsNullOrEmpty(this.Html);
            }
        }


        /// <summary>
        /// Checks if the message is posted by other person or not.
        /// This is mainly used on webhook notification, because apps need to reply a message from others(not from app itself) in most cases.
        /// </summary>
        /// <param name="person"><see cref="Person"/> to be checked.</param>
        /// <returns>true if the message is posted by other.</returns>
        public bool IsPostedByOther(Person person)
        {
            return (person != null && (person.CheckOwnershipStatus(this) == OwnershipStatus.NotHold));
        }

    }

}
