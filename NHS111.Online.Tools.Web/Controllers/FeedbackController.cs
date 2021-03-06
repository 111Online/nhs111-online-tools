﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Home([FromQuery]int page = 1, [FromQuery]int size = 500, [FromQuery]string startDate = "", [FromQuery]string endDate = "", [FromQuery]string query = "")
        {
            var model = await _listFeedbackViewModelBuilder
                .Build(page, size, startDate, endDate, query);
            return View(model.ToList());
        }
    }
}