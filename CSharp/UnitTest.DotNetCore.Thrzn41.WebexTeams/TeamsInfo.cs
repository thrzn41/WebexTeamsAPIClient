using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thrzn41.WebexTeams.Version1;

namespace UnitTest.DotNetCore.Thrzn41.WebexTeams
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class TeamsInfo : TeamsData
    {

        [JsonProperty(PropertyName = "apiToken")]
        public string APIToken { get; set; }

        [JsonProperty(PropertyName = "guestIssuerId")]
        public string GuestIssuerId { get; set; }

        [JsonProperty(PropertyName = "guestIssuerSecret")]
        public string GuestIssuerSecret { get; set; }

    }
}
