using ALISS.Business.PresentationTransferObjects.Claim;
using ALISS.Business.PresentationTransferObjects.User;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;

namespace ALISS.Business.Services
{
    public class ExportService
    {
        public static byte[] ExportAllUsers()
        {
            using (ALISSContext dc = new ALISSContext())
            {
                var allUsers = dc.UserProfiles.Where(a => a.Active).Select(x => new UserExportPTO()
                {
                    Name = x.Name ?? "",
                    Email = x.Email ?? "",
                    Postcode = x.Postcode ?? ""
                }).OrderBy(e => e.Email).ToList();

                MemoryStream stream = new MemoryStream();
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.WriteLine("Name, Email, Postcode");
                    foreach (var user in allUsers)
                    {
                        writer.WriteLine($"{user.Name.Replace(",", "")}, {user.Email}, {user.Postcode}");
                    }
                }

                return stream.ToArray();
            }
        }

        public static byte[] ExportClaimedUsers()
        {
            using (ALISSContext dc = new ALISSContext())
            {

                var claimedUsers = dc.Claims.Where(u => u.Status == 10).Select(x => new ClaimExportPTO()
                {

                    OrganisationName = x.Organisation.Name ?? "",
                    Email = x.ClaimedUser.Email ?? "",
                    RepresentativeName = x.ClaimedUser.Name ?? "",
                    CreatedOn = x.CreatedOn

                }).OrderBy(y => y.RepresentativeName).ToList();

                MemoryStream stream = new MemoryStream();
                if (claimedUsers != null && claimedUsers.Count() > 0)
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.WriteLine("Name, Email, OrganisationName, ClaimedOn");
                        foreach (var claimedUser in claimedUsers)
                        {
                            writer.WriteLine($"{claimedUser.RepresentativeName.Replace(",", "")},{claimedUser.Email},{claimedUser.OrganisationName.Replace(",", "")},{claimedUser.CreatedOn.ToString("dd/MM/yyyy")}");
                        }
                    }
                }

                return stream.ToArray();
            }

        }

        public static byte[] ExportEditorUsers()
        {

            List<string> userList = new List<string>();
            using (var appDbContext = new ApplicationDbContext())
            {
                string roleId = appDbContext.Roles.FirstOrDefault(n => n.Name == "Editor").Id;
                userList = appDbContext.Users.Include(s => s.Roles).Where(u => u.Roles.Any(r => r.RoleId == roleId)).Select(e => e.UserName).ToList();
            }
            using (ALISSContext dc = new ALISSContext())
            {

                var allUsers = dc.UserProfiles.Where(u => userList.Contains(u.Username)).Select(x => new UserExportPTO()
                {
                    Name = x.Name ?? "",
                    Email = x.Email ?? "",
                    Postcode = x.Postcode ?? ""

                }).OrderBy(y => y.Name).ToList();

                MemoryStream stream = new MemoryStream();
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.WriteLine("Name, Email, Postcode");
                    foreach (var user in allUsers)
                    {
                        writer.WriteLine($"{user.Name.Replace(",", "")}, {user.Email}, {user.Postcode}");
                    }
                }

                return stream.ToArray();
            }

        }
    }
}



