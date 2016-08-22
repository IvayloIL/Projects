using System.Collections.Generic;
using Newtonsoft.Json;

namespace Trello.Models
{
    public class Board
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("desc")]
        public string Description { get; set; }
        [JsonProperty("idOrganization")]
        public string OrganisationId { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonIgnore]
        public string Username { get; set; }
        [JsonIgnore]
        public IReadOnlyList<TrelloList> Lists { get; set; }
    }
}
