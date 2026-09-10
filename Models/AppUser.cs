namespace SJBRAssociatesWeb.Models
{
    public class AppUser
    {
        public AdminUser User { get; set; }
        public bool IsLoggedIn { get; set; }
    }
}
