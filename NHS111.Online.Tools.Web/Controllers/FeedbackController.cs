using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NHS111.Online.Tools.Web.Builders;

namespace NHS111.Online.Tools.Web.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly IListFeedbackViewModelBuilder _listFeedbackViewModelBuilder;

        public FeedbackController(IListFeedbackViewModelBuilder listFeedbackViewModelBuilder)
        {
            _listFeedbackViewModelBuilder = listFeedbackViewModelBuilder;
        }

        [HttpGet]
        public async Task<IActionResult> Home(int pagenumber = 0, int pagesize = 1000)
        {
            var model = await _listFeedbackViewModelBuilder
                .Build(pagenumber, pagesize);
            return View(model.ToList());
        }
    }
}