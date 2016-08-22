using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Trello.Models;

namespace Trello.Web.Helpers
{
    public static class HtmlHelpers
    {
        public static string CheckListProgress(this HtmlHelper htmlHelper, IEnumerable<ChecklistItem> checklistItems)
        {
            var items = checklistItems as IList<ChecklistItem> ?? checklistItems.ToList();
            float countItems = items.Count;
            float trueStatus = items.Count(x => x.State == "complete");
            var percentage = (trueStatus / countItems) * 100;
            
            return String.Format("width: {0}%", percentage);
        }
    }
}