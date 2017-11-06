using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhotoApp.Controllers
{
    public class FriendsController : Controller
    {
        // GET: Friends
        public ActionResult Friends()
        {
            ViewBag.Title = "Friends";
            return View();
        }
    }
}