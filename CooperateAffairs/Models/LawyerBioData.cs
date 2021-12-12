namespace DotNetFirebase.Models
{
    public class LawyerBioData : User
    {
        public string FirmName { get; set; }
        public string ContactName { get; set; }
        public bool IsActive { get; set; }
        public class Address
        {
            public string City { get; set; }
            public string State { get; set; }
            public string FullAddress { get; set; }
        }
    }
}