using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thrzn41.WebexTeams.Version1;
using Thrzn41.WebexTeams.Version1.OAuth2;

namespace UnitTestTool.EncryptIntegrationInfo
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class TeamsIntegrationInfo : TeamsData
    {
        [JsonProperty(PropertyName = "clientId")]
        public string ClientId { get; set; }

        [JsonProperty(PropertyName = "clientSecret")]
        public string ClientSecret { get; set; }

        [JsonProperty(PropertyName = "oAuthUrl")]
        public string OAuthUrl { get; set; }

        [JsonProperty(PropertyName = "redirectUri")]
        public string RedirectUri { get; set; }

        [JsonProperty(PropertyName = "tokenInfo")]
        public TokenInfo TokenInfo { get; set; }
    }
}
