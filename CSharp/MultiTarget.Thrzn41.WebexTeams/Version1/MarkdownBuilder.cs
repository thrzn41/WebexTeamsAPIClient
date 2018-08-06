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
using System.Net;
using System.Text;

namespace Thrzn41.WebexTeams.Version1
{

    /// <summary>
    /// Simple markdown builder.
    /// This class provides limited feature of markdown that is supported on Cisco Webex Teams API.
    /// </summary>
    public class MarkdownBuilder
    {

        /// <summary>
        /// <see cref="StringBuilder"/> for markdown.
        /// </summary>
        private StringBuilder markdown;




        /// <summary>
        /// Creates <see cref="MarkdownBuilder"/> instance.
        /// </summary>
        public MarkdownBuilder()
        {
            this.markdown = new StringBuilder();
        }

        /// <summary>
        /// Creates <see cref="MarkdownBuilder"/> instance with initial capacity.
        /// </summary>
        /// <param name="capacity">Initial capatity.</param>
        public MarkdownBuilder(int capacity)
        {
            this.markdown = new StringBuilder(capacity);
        }




        /// <summary>
        /// Appends line break.
        /// </summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder AppendLine()
        {
            this.markdown.Append("  \n");

            return this;
        }

        /// <summary>
        /// Appends line break.
        /// </summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder AppendParagraphSeparater()
        {
            this.markdown.Append("\n\n");

            return this;
        }

        /// <summary>
        /// Appends string.
        /// </summary>
        /// <param name="value">string value to be appended.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder Append(string value)
        {
            this.markdown.Append(value);

            return this;
        }

        /// <summary>
        /// Appends formatted string.
        /// </summary>
        /// <param name="format">string format.</param>
        /// <param name="args">Arguments for format.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder AppendFormat(string format, params object[] args)
        {
            this.markdown.AppendFormat(format, args);

            return this;
        }


        /// <summary>
        /// Begins Bold.
        /// </summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder BeginBold()
        {
            this.markdown.Append("**");

            return this;
        }

        /// <summary>
        /// Ends Bold.
        /// </summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder EndBold()
        {
            this.markdown.Append("**");

            return this;
        }

        /// <summary>
        /// Appends Bold string.
        /// </summary>
        /// <param name="value">string value to be appended.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder AppendBold(string value)
        {
            return BeginBold().Append(value).EndBold();
        }

        /// <summary>
        /// Appends Bold string with format.
        /// </summary>
        /// <param name="format">string format.</param>
        /// <param name="args">Arguments for format.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder AppendBoldFormat(string format, params object[] args)
        {
            return BeginBold().AppendFormat(format, args).EndBold();
        }


        /// <summary>
        /// Begins Italic.
        /// </summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder BeginItalic()
        {
            this.markdown.Append("*");

            return this;
        }

        /// <summary>
        /// Ends Italic.
        /// </summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder EndItalic()
        {
            this.markdown.Append("*");

            return this;
        }

        /// <summary>
        /// Appends Italic string.
        /// </summary>
        /// <param name="value">string value to be appended.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder AppendItalic(string value)
        {
            return BeginItalic().Append(value).EndItalic();
        }

        /// <summary>
        /// Appends Italic string with format.
        /// </summary>
        /// <param name="format">string format.</param>
        /// <param name="args">Arguments for format.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder AppendItalicFormat(string format, params object[] args)
        {
            return BeginItalic().AppendFormat(format, args).EndItalic();
        }


        /// <summary>
        /// Appends link.
        /// </summary>
        /// <param name="text">Link text.</param>
        /// <param name="uri">Link uri.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder AppendLink(string text, Uri uri)
        {
            this.markdown.AppendFormat("[{0}]({1})", text, uri.AbsoluteUri);

            return this;
        }


        /// <summary>
        /// Begins Ordered List.
        /// </summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder BeginOrderedList()
        {
            this.markdown.Append("1. ");

            return this;
        }

        /// <summary>
        /// Ends Ordered List.
        /// </summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder EndOrderedList()
        {
            this.markdown.Append("\n");

            return this;
        }


        /// <summary>
        /// Appends Ordered List.
        /// </summary>
        /// <param name="value">string value to be appended.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder AppendOrderedList(string value)
        {
            return BeginOrderedList().Append(value).EndOrderedList();
        }

        /// <summary>
        /// Appends Ordered List with format.
        /// </summary>
        /// <param name="format">string format.</param>
        /// <param name="args">Arguments for format.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder AppendOrderedListFormat(string format, params object[] args)
        {
            return BeginOrderedList().AppendFormat(format, args).EndOrderedList();
        }


        /// <summary>
        /// Appends Ordered List.
        /// </summary>
        /// <param name="value">string value to be appended.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        [Obsolete("There is a typo in the method name. Please use AppendOrderedList().")]
        public MarkdownBuilder AppendOrderdList(string value)
        {
            return BeginOrderedList().Append(value).EndOrderedList();
        }

        /// <summary>
        /// Appends Ordered List with format.
        /// </summary>
        /// <param name="format">string format.</param>
        /// <param name="args">Arguments for format.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        [Obsolete("There is a typo in the method name. Please use AppendOrderedListFormat().")]
        public MarkdownBuilder AppendOrderdListFormat(string format, params object[] args)
        {
            return BeginOrderedList().AppendFormat(format, args).EndOrderedList();
        }


        /// <summary>
        /// Begins Unordered List.
        /// </summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder BeginUnorderedList()
        {
            this.markdown.Append("- ");

            return this;
        }

        /// <summary>
        /// Ends Unordered List.
        /// </summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder EndUnorderedList()
        {
            this.markdown.Append("\n");

            return this;
        }

        /// <summary>
        /// Appends Unordered List.
        /// </summary>
        /// <param name="value">string value to be appended.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder AppendUnorderedList(string value)
        {
            return BeginUnorderedList().Append(value).EndUnorderedList();
        }

        /// <summary>
        /// Appends Unordered List with format.
        /// </summary>
        /// <param name="format">string format.</param>
        /// <param name="args">Arguments for format.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder AppendUnorderedListFormat(string format, params object[] args)
        {
            return BeginUnorderedList().AppendFormat(format, args).EndUnorderedList();
        }

        /// <summary>
        /// Appends Unordered List.
        /// </summary>
        /// <param name="value">string value to be appended.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        [Obsolete("There is a typo in the method name. Please use AppendUnorderedList().")]
        public MarkdownBuilder AppendUnorderdList(string value)
        {
            return BeginUnorderedList().Append(value).EndUnorderedList();
        }

        /// <summary>
        /// Appends Unordered List with format.
        /// </summary>
        /// <param name="format">string format.</param>
        /// <param name="args">Arguments for format.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        [Obsolete("There is a typo in the method name. Please use AppendUnorderedListFormat().")]
        public MarkdownBuilder AppendUnorderdListFormat(string format, params object[] args)
        {
            return BeginUnorderedList().AppendFormat(format, args).EndUnorderedList();
        }

        /// <summary>
        /// Begins Block Quote.
        /// </summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder BeginBlockQuote()
        {
            this.markdown.Append("> ");

            return this;
        }

        /// <summary>
        /// Ends Block Quote.
        /// </summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder EndBlockQuote()
        {
            this.markdown.Append("\n");

            return this;
        }


        /// <summary>
        /// Appends Block Quote.
        /// </summary>
        /// <param name="value">string value to be appended.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder AppendBlockQuote(string value)
        {
            return BeginBlockQuote().Append(value).EndBlockQuote();
        }

        /// <summary>
        /// Appends Block Quote with format.
        /// </summary>
        /// <param name="format">string format.</param>
        /// <param name="args">Arguments for format.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder AppendBlockQuoteFormat(string format, params object[] args)
        {
            return BeginBlockQuote().AppendFormat(format, args).EndBlockQuote();
        }


        /// <summary>
        /// Appends In-Line Code.
        /// </summary>
        /// <param name="value">string value to be appended.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder AppendInLineCode(string value)
        {
            this.markdown.AppendFormat("`{0}`", value);

            return this;
        }

        /// <summary>
        /// Begins Code Block.
        /// </summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder BeginCodeBlock()
        {
            this.markdown.Append("```\n");

            return this;
        }

        /// <summary>
        /// Ends Code Block.
        /// </summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder EndCodeBlock()
        {
            this.markdown.Append("\n```\n");

            return this;
        }


        /// <summary>
        /// Appends Code Block.
        /// </summary>
        /// <param name="value">string value to be appended.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder AppendCodeBlock(string value)
        {
            return BeginCodeBlock().Append(value).EndCodeBlock();
        }


        /// <summary>
        /// Encodes html string.
        /// </summary>
        /// <param name="value">string value to be escaped.</param>
        /// <returns>Escaped string.</returns>
        private static string encodeHtml(string value)
        {
            if(String.IsNullOrEmpty(value))
            {
                return value;
            }

            return WebUtility.HtmlEncode(value);

//#if (DOTNETSTANDARD1_3 || DOTNETCORE1_0)
//            var result = new StringBuilder();
//            var chars  = value.ToCharArray();

//            foreach (var item in chars)
//            {
//                if (item >= 0xa0 && item <= 0xff)
//                {
//                    result.AppendFormat("&#{0:D};", (int)item);
//                }
//                else
//                {
//                    switch (item)
//                    {
//                        case '"':
//                            result.Append("&quot;");
//                            break;
//                        case '&':
//                            result.Append("&amp;");
//                            break;
//                        case '\'':
//                            result.Append("&#39;");
//                            break;
//                        case '<':
//                            result.Append("&lt;");
//                            break;
//                        case '>':
//                            result.Append("&gt;");
//                            break;
//                        default:
//                            result.Append(item);
//                            break;
//                    }
//                }
//            }

//            return result.ToString();
//#else
//            return System.Web.HttpUtility.HtmlEncode(value);
//#endif
        }

        /// <summary>
        /// Appends mention to a person.
        /// </summary>
        /// <param name="personIdOrEmail">PersonId or PersonEmail to be mentioned.</param>
        /// <param name="name">Mentioned name.</param>
        /// <param name="personIdType"><see cref="PersonIdType"/> of personIdOrEmail parameter.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder AppendMentionToPerson(string personIdOrEmail, string name, PersonIdType personIdType = PersonIdType.Detect)
        {
            personIdType = TeamsAPIClient.DetectPersonIdType(personIdOrEmail, personIdType);

            switch (personIdType)
            {
                case PersonIdType.Email:
                    this.markdown.AppendFormat("<@personEmail:{0}|{1}>", encodeHtml(personIdOrEmail), encodeHtml(name));
                    break;
                default:
                    this.markdown.AppendFormat("<@personId:{0}|{1}>",    encodeHtml(personIdOrEmail), encodeHtml(name));
                    break;
            }

            return this;
        }

        /// <summary>
        /// Appends mention to a person.
        /// </summary>
        /// <param name="person"><see cref="Person"/> to be mentioned.</param>
        /// <param name="nameType"><see cref="PersonNameType"/> to be used on mention.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder AppendMentionToPerson(Person person, PersonNameType nameType = PersonNameType.DisplayName)
        {
            string name = null;

            switch (nameType)
            {
                case PersonNameType.NickName:
                    name = person.NickName;
                    break;
                case PersonNameType.FirstName:
                    name = person.FirstName;
                    break;
                case PersonNameType.LastName:
                    name = person.LastName;
                    break;
                default:
                    name = person.DisplayName;
                    break;
            }

            if(String.IsNullOrEmpty(name))
            {
                name = person.DisplayName;
            }

            return AppendMentionToPerson(person.Id, name, PersonIdType.Id);
        }


        /// <summary>
        /// Appends mention to a person.
        /// </summary>
        /// <param name="group"><see cref="MentionedGroup"/> to be mentioned.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder AppendMentionToGroup(MentionedGroup group)
        {
            if(group == MentionedGroup.All)
            {
                this.Append("<@all>");
            }
            else
            {
                this.AppendFormat("<@{0}>", encodeHtml(group.Name));
            }

            return this;
        }

        /// <summary>
        /// Appends mention to all.
        /// </summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder AppendMentionToAll()
        {
            return AppendMentionToGroup(MentionedGroup.All);
        }


        /// <summary>
        /// Clears the markdown.
        /// </summary>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public MarkdownBuilder Clear()
        {
            this.markdown.Clear();

            return this;
        }


        /// <summary>
        /// Gets markdown string.
        /// </summary>
        /// <returns>Markdown string.</returns>
        public override string ToString()
        {
            return this.markdown.ToString();
        }

    }

}
