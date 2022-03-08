using System.Collections.Generic;

namespace CooperateMVC.Models
{
    public class KycInfo
    {
        
        public int Id { get; set; }
        public int CompanyOfficersListId { get; set; }
        public int ShareHoldersId { get; set; }

        public List<CompanyOfficer> CompanyOfficersList { get; set; }
        public List<ShareHolder> ShareHolders { get; set; }
        public string CompanyObjective { get; set; }
        public string CertificateUrl { get; set; }
        public string RCNumber { get; set; }
        
        
        public class CompanyOfficer
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Position { get; set; }
            public string SerialNumber { get; set; }
        }

        public class ShareHolder
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Percentage { get; set; }
        }
    }
}