using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DocumentManagementCommon;

namespace DoucmentManagementWeb.Controllers
{
    public class DocumentController : Controller
    {
        public ActionResult DocumentList()
        {
            return View(new List<DocumentInfo>());
        }

        public ActionResult AddDocument()
        {
            return View();
        }
    }
}