using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALISS.Business.ViewModels.User
{
    public class DeleteUserViewModel
    {
        public int UserProfileId { get; set; }
        public string Name { get; set; }
        public bool IsDeleteValid { get; set; }
    }
}
