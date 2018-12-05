using System;
using System.Collections.Generic;
using System.Net;
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
        /// Indicates the request has Retry-After header value.
        /// </summary>
        public bool HasRetryAfter
        {
            get
            {
                return (this.RetryAfter != null);
            }
        }


    }
}
