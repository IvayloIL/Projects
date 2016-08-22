using Newtonsoft.Json;

namespace Trello.Models
{
    public class Action
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("idMemberCreator")]
        public string IdMemberCreator { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }

        [JsonProperty("type")]
        public Type Type { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("memberCreator")]
        public MemberCreator MemberCreator { get; set; }

    }

    public class MemberCreator
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("username")]
        public string UserName { get; set; }

        [JsonProperty("fullname")]
        public string FullName { get; set; }
    }

    public class Data
    {
        [JsonProperty("card")]
        public DataCard Card { get; set; }

        [JsonProperty("list")]
        public DataList DataList { get; set; }

        [JsonProperty("board")]
        public DataBoard DataBoard { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public class DataBoard
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class DataList
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class DataCard
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }

    }

    public enum Type
    {
        commentCard,
    }
}