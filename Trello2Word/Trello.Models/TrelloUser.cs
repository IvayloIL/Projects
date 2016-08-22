using Newtonsoft.Json;

namespace Trello.Models
{
    public class TrelloUser
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("fullName")]
        public string FullName { get; set; }
    }
}
