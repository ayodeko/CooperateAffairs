//using FirebaseAdmin.Auth;

namespace CooperateMVC.Models
{
    public class User
    {
        public string Uid { get; set; }
        public int Id { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string EmailVerified { get; set; }
        public string Role { get; set; }
    }
}