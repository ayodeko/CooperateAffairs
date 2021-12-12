namespace DotNetFirebase.Models
{
    public class BankUser : User
    {
        public string BankName { get; set; }
        public string BankOfficerName { get; set; }
        public string BankOfficerPhone { get; set; }
        public string BankAuthToken { get; set; }
    }
}