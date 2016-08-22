using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Trello.Models
{
    public class Card
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("desc")]
        public string Description { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("dateLastActivity")]
        public DateTime LastActivity { get; set; }
        [JsonIgnore]
        public IReadOnlyList<Attachment> Attachments { get; set; }
        [JsonIgnore]
        public IReadOnlyList<Checklist> Checklists { get; set; }
        [JsonIgnore]
        public IReadOnlyList<Action> Actions { get; set; }
    }

    public class Attachment
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("date")]
        public DateTime DateTime { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }

    }

    public class Checklist
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("checkItems")]
        public IEnumerable<ChecklistItem> Items { get; set; }
    }

    public class ChecklistItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }

    }
}
