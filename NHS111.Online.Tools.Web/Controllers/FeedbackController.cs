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
        private readonly IConfiguration _configuration;

        public FeedbackController(IListFeedbackViewModelBuilder listFeedbackViewModelBuilder, IConfiguration configuration)
        {
            _listFeedbackViewModelBuilder = listFeedbackViewModelBuilder;
            _configuration = configuration;
        }

        public async Task<IActionResult> Home()
        {
            var model = await _listFeedbackViewModelBuilder.Build();
            return View(model);
        }
    }
}