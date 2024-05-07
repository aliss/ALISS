namespace ALISS.Business.ViewModels.User
{
    public class CurrentUserViewModel
    {
        public int UserProfileId { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
    }
}
