using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ALISS.CMS.Models.User
{
    public class CurrentUserViewModel
    {
        public int UserProfileId { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}