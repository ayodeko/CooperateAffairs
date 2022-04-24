using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CooperateMVC.Models
{
    public class KycInfo
    {
        public KycInfo()
			{
                Id = Guid.NewGuid().ToString();
			}
        
        public string Id { get; set; }
        public string CompanyOfficersListId { get; set; }
        public string ShareHoldersId { get; set; }

        public List<CompanyOfficer> CompanyOfficersList { get; set; }
        public List<ShareHolder> ShareHolders { get; set; }
        public string CompanyObjective { get; set; }
        public string CertificateUrl { get; set; }
        public string CompanyName { get; set; }
        public string RCNumber { get; set; }
        
        
        public class CompanyOfficer
        {
            public CompanyOfficer()
            {
                Id = Guid.NewGuid().ToString();
            }
            public string Id { get; set; }
            [Required]
            public string Name { get; set; }
            public string Position { get; set; }
            public string SerialNumber { get; set; }
        }

        public class ShareHolder
        {
			public ShareHolder()
			{
                Id = Guid.NewGuid().ToString();
			}
            public string Id { get; set; }
            [Required]
            public string Name { get; set; }
            public decimal Percentage { get; set; }
        }
    }
}