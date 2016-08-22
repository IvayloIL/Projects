using System.Collections.Generic;
using Newtonsoft.Json;

namespace Trello.Models
{
    public class TrelloList
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("idBoard")]
        public string BoardId { get; set; }
        [JsonIgnore]
        public IEnumerable<Card> Cards { get; set; }
        [JsonIgnore]
        public bool Checked { get; set; }
        [JsonIgnore]
        public bool HasComments { get; set; }
    }
}
