using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhotoApp.Controllers
{
    public class MeController : Controller
    {
        // GET: Me
        public ActionResult Me()
        {
            ViewBag.Title = "My Page";
            return View();
        }
    }
}