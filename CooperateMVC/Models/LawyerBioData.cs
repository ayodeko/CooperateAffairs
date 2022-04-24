namespace CooperateMVC.Models
{
    public class LawyerBioData : User
    {

        public string Id { get; set; }
        public string FirmName { get; set; }
        public string ContactName { get; set; }
        public bool IsActive { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string FullAddress { get; set; }
    }
}