using System.Collections.Generic;
using Newtonsoft.Json;
using RestSharp;
using Trello.Models;
using Attachment = Trello.Models.Attachment;
using Board = Trello.Models.Board;
using Card = Trello.Models.Card;

namespace Trello.Utils
{
    public static class RestMethods
    {

        public static Token GetTokenInformation(string token, RestClient restClient)
        {
            var url = string.Format("tokens/{0}", token);
            var request = new RestRequest(url, Method.GET);
            var response = restClient.Execute(request);
            var deserializeObject = JsonConvert.DeserializeObject<Token>(response.Content);

            return deserializeObject;
        }

        public static TrelloUser GetTrelloUser(string userId, RestClient restClient)
        {
            var url = string.Format("members/{0}", userId);
            var request = new RestRequest(url, Method.GET);
            var response = restClient.Execute(request);
            var deserializeObject = JsonConvert.DeserializeObject<TrelloUser>(response.Content);

            return deserializeObject;
        }

        public static TrelloList GetList(string listId, RestClient restClient)
        {
            var request = new RestRequest("/lists/" + listId, Method.GET);
            var response = restClient.Execute(request);
            var deserializeObject = JsonConvert.DeserializeObject<TrelloList>(response.Content);

            return deserializeObject;
            
        }

        public static IReadOnlyList<Board> GetUserBoards(string username, RestClient restClient)
        {
            var request = new RestRequest("members/" + username + "/boards", Method.GET);
            
            var response = restClient.Execute(request);
            var deserializeObject = JsonConvert.DeserializeObject<Board[]>(response.Content);
           
            return deserializeObject;
        }

        public static Board GetBoard(string id, RestClient restClient)
        {
            var request = new RestRequest( "/boards/" +id, Method.GET);
            var response = restClient.Execute(request);
            var deserializeObject = JsonConvert.DeserializeObject<Board>(response.Content);

            return deserializeObject;
        }

        public static IEnumerable<TrelloList> GetAllLists(string boardId, RestClient restClient)
        {
            var request = new RestRequest("boards/" + boardId + "/lists", Method.GET);
            var response = restClient.Execute(request);
            var deserializeObject = JsonConvert.DeserializeObject<TrelloList[]>(response.Content);

            return deserializeObject;
        }

        public static IReadOnlyList<Card> GetListCards(string listId, RestClient restClient)
        {
            var request = new RestRequest("lists/" + listId +"/cards", Method.GET);
            var response = restClient.Execute(request);
            var deserializeObject = JsonConvert.DeserializeObject<Card[]>(response.Content);

            return deserializeObject;
        }

        public static Card GetCard(string id, RestClient restClient)
        {
            var request = new RestRequest("cards/" + id, Method.GET);
            var response = restClient.Execute(request);
            var deserializeObject = JsonConvert.DeserializeObject<Card>(response.Content);

            return deserializeObject;
        }

        public static IReadOnlyList<Attachment> GetCardAttachments(string cardId, RestClient restClient)
        {
            var request = new RestRequest("cards/" + cardId + "/attachments", Method.GET);
            var response = restClient.Execute(request);
            var deserializeObject = JsonConvert.DeserializeObject<Attachment[]>(response.Content);

            return deserializeObject;
        }

        public static IReadOnlyList<Checklist> GetCardChecklists(string cardId, RestClient restClient)
        {
            var request = new RestRequest("cards/" + cardId + "/checklists", Method.GET);
            var response = restClient.Execute(request);
            var deserializeObject = JsonConvert.DeserializeObject<Checklist[]>(response.Content);

            return deserializeObject;
        }

        public static IReadOnlyList<Action> GetCardActions(string cardId, RestClient restClient)
        {
            var request = new RestRequest("cards/" + cardId + "/actions", Method.GET);
            var response = restClient.Execute(request);
            var deserializeObject = JsonConvert.DeserializeObject<Action[]>(response.Content);

            return deserializeObject;
        }
    }
}
