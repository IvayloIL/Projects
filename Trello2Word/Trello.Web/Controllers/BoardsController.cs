using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using RestSharp;
using Trello.Models;
using Trello.Utils;

namespace Trello.Web.Controllers
{

    public class BoardsController : BaseController
    {
        public ActionResult Index()
        {
            if (TrelloToken == null)
            {
                return RedirectToAction("AuthorizeToken");
            }

            AddDefaultParameters();

            var boards = RestMethods.GetUserBoards(LoggedUserId, RestClient).OrderBy(x => x.Name);

            return View(boards);
        }

        public ActionResult Details(string id)
        {
            if (TrelloToken == null)
            {
                return RedirectToAction("AuthorizeToken");
            }

            AddDefaultParameters();

            var board = RestMethods.GetBoard(id, RestClient);
            var boardLists = RestMethods.GetAllLists(board.Id, RestClient);
            var trelloLists = boardLists as TrelloList[] ?? boardLists.ToArray();
            foreach (var x in trelloLists)
            {
                x.HasComments = false;
                x.Checked = true;
            }
            board.Lists = trelloLists;

            return View(board);
        }

        public ActionResult AuthorizeToken()
        {
            var requestUri = "Authorize";
            var request = new RestRequest(requestUri, Method.GET);
            request.AddParameter("callback_method", "fragment");
            request.AddParameter("scope", "read");
            request.AddParameter("expiration", "1day");
            request.AddParameter("name", "Trello2Word");
            request.AddParameter("key", Startup.ConsumerKey);
            request.AddParameter("return_url", CreateReturnUrl());

            var url = RestClient.BuildUri(request).ToString();

            return Redirect(url);
        }

        public ActionResult ReceiveToken(string token)
        {
            if (token == "")
            {
                return RedirectToAction("Index", "Home");
            }

            if (token != null)
            {
                TrelloToken = token;
                RestClient.AddDefaultParameter("key", Startup.ConsumerKey);
                var tokenInfo = RestMethods.GetTokenInformation(token, RestClient);
                LoggedUserId = tokenInfo.UserId;
                var trelloUser = RestMethods.GetTrelloUser(LoggedUserId, RestClient);
                Username = trelloUser.Username;

                return RedirectToAction("Index");
            }

            return View("ReceiveToken");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FilterLists(IEnumerable<TrelloList> list)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            AddDefaultParameters();

            var trelloLists = list as TrelloList[] ?? list.ToArray();
            var boardId = trelloLists.First(x => x.BoardId != null).BoardId;

            var lists = trelloLists.Where(x => x.Checked)
                .Select(listId => listId.Id)
                .ToArray();

            var listsWithComments = trelloLists.Where(x => x.HasComments)
                .Select(listId => listId.Id)
                .ToArray();

            var stringId = string.Join(",", lists);
            var listsWithCommentsIds = string.Join(",", listsWithComments);
            return RedirectToAction(
                "GenerateBoardDoc",
                "Boards",
                new { id = boardId, boardLists = stringId, listsWithCommentsIds });
        }


        public ActionResult GenerateBoardDoc(string id, string boardLists, string listsWithCommentsIds)
        {
            AddDefaultParameters();

            var board = RestMethods.GetBoard(id, RestClient);
            if (board == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (boardLists != null)
            {
                var arrOfId = boardLists.Split(',');
                string[] commentsListIds = {};
                if (listsWithCommentsIds != null)
                {
                    commentsListIds = listsWithCommentsIds.Split(',');
                }
                var trelloList = arrOfId.Select(listId => RestMethods.GetList(listId, RestClient)).ToList();
                board.Lists = trelloList;

                foreach (var list in board.Lists)
                {
                    list.Cards = RestMethods.GetListCards(list.Id, RestClient);
                    foreach (var card in list.Cards)
                    {
                        card.Attachments = RestMethods.GetCardAttachments(card.Id, RestClient).ToList();
                        card.Checklists = RestMethods.GetCardChecklists(card.Id, RestClient).ToList();
                        if (commentsListIds.Any(x => x == list.Id.ToString()))
                        {
                            card.Actions = RestMethods.GetCardActions(card.Id, RestClient).ToList();
                        }
                    }
                }
            }

            byte[] fileContents;
            using (var memoryStream = new MemoryStream())
            {
                WordExtraction.DocumentWithOpenXml(memoryStream, board);
                fileContents = memoryStream.ToArray();
            }

            Response.AppendHeader("Content-Disposition", string.Format("inline; filename={0}.docx", board.Name));
            return new FileContentResult(fileContents, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }

        private string CreateReturnUrl()
        {
            return Url.Action("ReceiveToken", "Boards", null, Url.RequestContext.HttpContext.Request.Url.Scheme);
        }

        public string TrelloToken
        {
            get { return (string)Session["oauth_token"]; }
            set { Session["oauth_token"] = value; }
        }

        public string LoggedUserId
        {
            get { return (string)Session["UserId"]; }
            set { Session["UserId"] = value; }
        }

        public string Username
        {
            get { return (string)Session["Username"]; }
            set { Session["Username"] = value; }
        }
        /// <summary>
        ///  Add this method to each new action!
        /// </summary>
        private void AddDefaultParameters()
        {
            RestClient.AddDefaultParameter("oauth_token", TrelloToken);
            RestClient.AddDefaultParameter("oauth_consumer_key", Startup.ConsumerKey);
            RestClient.AddDefaultParameter("oauth_consumer_secret", Startup.ConsumerSecret);
        }
    }
}