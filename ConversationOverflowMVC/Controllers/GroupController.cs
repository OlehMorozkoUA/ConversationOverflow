using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConversationOverflowMVC.Controllers
{
    public class GroupController : Controller
    {
        public IActionResult List()
        {
            ViewData["Title"] = "List Group";
            return View();
        }
    }
}
