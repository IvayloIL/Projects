using System;
using Newtonsoft.Json;

namespace Trello.Models
{
    public class Token
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("dateCreated")]
        public DateTime Created { get; set; }
        [JsonProperty("dateExpires")]
        public DateTime Expires { get; set; }
        [JsonProperty("idMember")]
        public string UserId { get; set; }
    }
}
