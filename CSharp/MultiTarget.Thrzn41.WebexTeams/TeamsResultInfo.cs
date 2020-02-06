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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Thrzn41.WebexTeams
{


    /// <summary>
    /// The result info of Teams API request.
    /// </summary>
    public class TeamsResultInfo
    {

        /// <summary>
        /// Creates TeamsResultInfo.
        /// </summary>
        public TeamsResultInfo()
        {
            this.TransactionId = Guid.NewGuid();
        }


        /// <summary>
        /// Transaction id for this result.
        /// </summary>
        public Guid TransactionId { get; internal set; }

        /// <summary>
        /// Indicats the request has been succeeded or not.
        /// </summary>
        public bool IsSuccessStatus { get; internal set; }

        /// <summary>
        /// Http status code returned on the API request.
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; internal set; }

        /// <summary>
        /// Tracking id of the request.
        /// This id can be used for technical support.
        /// </summary>
        public string TrackingId { get; internal set; }

        /// <summary>
        /// Indicates whether the result has tracking id or not.
        /// </summary>
        public bool HasTrackingId
        {
            get
            {
                return (!String.IsNullOrEmpty(this.TrackingId));
            }
        }

        /// <summary>
        /// Retry-After header value.
        /// </summary>
        public RetryConditionHeaderValue RetryAfter { get; internal set; }

        /// <summary>
        /// Indicates the result has Retry-After header value.
        /// </summary>
        public bool HasRetryAfter
        {
            get
            {
                return (this.RetryAfter != null);
            }
        }

        /// <summary>
        /// Time to retry.
        /// </summary>
        public TimeSpan? TimeToRetry { get; internal set; }

        /// <summary>
        /// Indicates the result has time to retry.
        /// </summary>
        public bool HasTimeToRetry
        {
            get
            {
                return this.TimeToRetry.HasValue;
            }
        }

        /// <summary>
        /// <see cref="TeamsRequestInfo"/> for this request.
        /// </summary>
        internal TeamsRequestInfo RequestInfo { get; set; }


        /// <summary>
        /// Request line of this request.
        /// </summary>
        public string RequestLine
        {
            get
            {
                if (this.RequestInfo == null)
                {
                    return "UNKNOWN * HTTP/1.1";
                }

                return this.RequestInfo.GetRequestLine();
            }
        }



        /// <summary>
        /// Path and resource.
        /// </summary>
        private class PathAndResource
        {

            /// <summary>
            /// Requested path.
            /// </summary>
            public string Path { get; set; }

            /// <summary>
            /// Requested resource.
            /// </summary>
            public TeamsResource Resouce { get; set; }
        }


        /// <summary>
        /// Path and resource list.
        /// </summary>
        private static readonly List<PathAndResource> PATH_AND_RESOURCES;


        /// <summary>
        /// Static constructor.
        /// </summary>
        static TeamsResultInfo()
        {
            PATH_AND_RESOURCES = new List<PathAndResource>()
            {
                new PathAndResource{ Path = "/v1/messages",                  Resouce = TeamsResource.Message                 },
                new PathAndResource{ Path = "/v1/rooms",                     Resouce = TeamsResource.Space                   },
                new PathAndResource{ Path = "/v1/memberships",               Resouce = TeamsResource.SpaceMembership         },
                new PathAndResource{ Path = "/v1/teams",                     Resouce = TeamsResource.Team                    },
                new PathAndResource{ Path = "/v1/team/memberships",          Resouce = TeamsResource.TeamMembership          },
                new PathAndResource{ Path = "/v1/people",                    Resouce = TeamsResource.Person                  },
                new PathAndResource{ Path = "/v1/webhooks",                  Resouce = TeamsResource.Webhook                 },
                new PathAndResource{ Path = "/v1/events",                    Resouce = TeamsResource.Event                   },
                new PathAndResource{ Path = "/v1/licenses",                  Resouce = TeamsResource.License                 },
                new PathAndResource{ Path = "/v1/organizations",             Resouce = TeamsResource.Organization            },
                new PathAndResource{ Path = "/v1/roles",                     Resouce = TeamsResource.Role                    },
                new PathAndResource{ Path = "/v1/resourceGroups",            Resouce = TeamsResource.ResourceGroup           },
                new PathAndResource{ Path = "/v1/resourceGroup/memberships", Resouce = TeamsResource.ResourceGroupMembership },
                new PathAndResource{ Path = "/v1/access_token",              Resouce = TeamsResource.AccessToken             },
                new PathAndResource{ Path = "/v1/jwt/login",                 Resouce = TeamsResource.GuestUser               },
                new PathAndResource{ Path = "/v1/contents",                  Resouce = TeamsResource.FileData                },
                new PathAndResource{ Path = "/v1/attachment/actions",        Resouce = TeamsResource.AttachmentAction        },
            };

        }


        /// <summary>
        /// Parse the request into <see cref="TeamsResourceOperation"/>.
        /// </summary>
        /// <returns><see cref="TeamsResourceOperation"/> of the result.</returns>
        public TeamsResourceOperation ParseResourceOperation()
        {
            TeamsResource  resource  = TeamsResource.Unknown;
            TeamsOperation operation = TeamsOperation.Unknown;

            if(this.RequestInfo != null)
            {
                var method = this.RequestInfo.HttpMethod;

                if(method != null)
                {
                    if(method == HttpMethod.Post)
                    {
                        operation = TeamsOperation.Create;
                    }
                    else if(method == HttpMethod.Get)
                    {
                        operation = TeamsOperation.Get;
                    }
                    else if (method == HttpMethod.Put)
                    {
                        operation = TeamsOperation.Update;
                    }
                    else if (method == HttpMethod.Delete)
                    {
                        operation = TeamsOperation.Delete;
                    }
                    else if (method == HttpMethod.Head)
                    {
                        operation = TeamsOperation.GetHeader;
                    }
                }

                string path = this.RequestInfo.Uri?.AbsolutePath;

                if ( !String.IsNullOrEmpty(path) )
                {
                    foreach (var item in PATH_AND_RESOURCES)
                    {
                        if(path.StartsWith(item.Path))
                        {
                            resource = item.Resouce;

                            if(resource == TeamsResource.AccessToken && operation == TeamsOperation.Create)
                            {
                                operation = TeamsOperation.Get;
                            }
                            else if (operation == TeamsOperation.Get && path.EndsWith(item.Path))
                            {
                                operation = TeamsOperation.List;
                            }

                            break;
                        }
                    }
                }

                if(resource == TeamsResource.Space && path.EndsWith("/meetingInfo"))
                {
                    resource = TeamsResource.SpaceMeetingInfo;
                }

                if(resource == TeamsResource.FileData && operation == TeamsOperation.GetHeader)
                {
                    operation = TeamsOperation.Get;
                    resource  = TeamsResource.FileInfo;
                }
            }

            return (new TeamsResourceOperation(resource, operation));
        }


        /// <summary>
        /// Copies Info to ather <see cref="TeamsResultInfo"/>.
        /// </summary>
        /// <param name="destination"><see cref="TeamsResultInfo"/> to be copied.</param>
        internal void CopyInfoTo(TeamsResultInfo destination)
        {
            destination.TransactionId   = this.TransactionId;
            destination.IsSuccessStatus = this.IsSuccessStatus;
            destination.HttpStatusCode  = this.HttpStatusCode;
            destination.RequestInfo     = this.RequestInfo;
            destination.RetryAfter      = this.RetryAfter;
            destination.TimeToRetry     = this.TimeToRetry;
            destination.TrackingId      = this.TrackingId;
        }

    }

}
