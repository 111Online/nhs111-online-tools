using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NHS111.Online.Tools.Web.Controllers
{
    public class FeedbackController : Controller
    {
        public IActionResult Home()
        {
            return View();
        }
    }
}