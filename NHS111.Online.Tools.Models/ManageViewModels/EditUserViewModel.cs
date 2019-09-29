using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHS111.Online.Tools.Models.SharedViewModels;

namespace NHS111.Online.Tools.Models.ManageViewModels
{
    public class EditUserViewModel : UserViewModel
    {
        public EditUserViewModel()
        {
            Roles = new List<SelectListItem>();
        }
        
        public IEnumerable<SelectListItem> Roles { get; set; }
    }
}
