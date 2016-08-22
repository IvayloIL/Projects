using System.Web.Mvc;
using Trello.Utils;

namespace Trello.Web.Controllers
{
    public class CardsController : BaseController
    {
        public ActionResult Details(string id)
        {
            var card = RestMethods.GetCard(id, this.RestClient);
            card.Attachments = RestMethods.GetCardAttachments(id, this.RestClient);
            card.Checklists = RestMethods.GetCardChecklists(id, this.RestClient);
            
            return View(card);
        }
    }
}