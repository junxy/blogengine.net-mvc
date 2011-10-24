using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlogEngine.Admin.Controllers
{
    public class PostsController : BaseController
    {
        //
        // GET: /Posts/

        public ActionResult Index()
        {
            return RedirectToAction("Posts");
        }

        public ActionResult Posts()
        {
            return View();
        }

        public ActionResult Categories()
        {
            return View();
        }

        public ActionResult Tags()
        {
            return View();
        }

        public ActionResult AddEntity()
        {
            return View();
        }



    }
}
