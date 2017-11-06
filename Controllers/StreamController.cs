using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhotoApp.Controllers
{
    public class StreamController : Controller
    {
        // GET: Stream
        public ActionResult Stream()
        {
            ViewBag.Title = "Stream";
            return View();
        }
    }
}