using ALISS.Business.PresentationTransferObjects.User;
using System.Collections.Generic;

namespace ALISS.Business.ViewModels.User
{
    public class UserListingViewModel
    {
        public List<UserPTO> Users { get; set; }
        public string SearchTerm { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int TotalResults { get; set; }
        public string Role { get; set; }
        public bool IsEditor { get; set; }
        public bool IsAdmin { get; set; }


    }
}
