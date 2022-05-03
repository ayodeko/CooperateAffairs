using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static CooperateMVC.Models.KycInfo;
using static CooperateMVC.Models.KycRequest;

namespace CooperateMVC.Models
{
    public class KycRequest
    {
        public int Id { get; set; }
        public string RequestId { get; set; }
        [Required]
        public string RCNumber { get; set; }
        public string RequesterName { get; set; }
        public string RequesterId { get; set; }
        public string? AssignedLawyerId { get; set; }
        public string? TimeRequested { get; set; }
        public string? LastTimeUpdated { get; set; }
        public string AdditionalInformation { get; set; }
        public KycRequestStatus Status { get; set; }
        public KycInfo KycInfo { get; set; }

        public enum KycRequestStatus
        {
            Pending,
            Processing,
            Completed,
            Exception
        }
    }

    public class CreateKycRequest
	{
        public int Id { get; set; }
        [Required]
		public string RCNumber { get; set; }
		public string RequesterName { get; set; }
		public string AdditionalInformation { get; set; }
	}

    public class EditKycRequest
    {
        public int Id { get; set; }
        [Required]
        public string RCNumber { get; set; }
        public string RequesterName { get; set; }
        public string AdditionalInformation { get; set; }
        public KycRequestStatus Status { get; set; }
    }

    public class UploadKycRequest
	{
        public string RCNumber { get; set; }
        public string LawyerID { get; set; }
        public KycInfo KycInfo { get; set; }
	}

    public class KycInfoViewModel
	{
		public KycInfoViewModel()
		{

		}

        [Required]
        public string RCNumber { get; set; }
        public string KycRequestJsonHolder { get; set; }
        public string CompanyObjective { get; set; }
        public string CertificateUrl { get; set; }
        public string CompanyName { get; set; }
        public List<CompanyOfficer> CompanyOfficersList { get; set; }
        public List<ShareHolder> ShareHolders { get; set; }
        public string CompanyOfficerName { get; set; }
        public string CompanyOfficerId { get; set; }
        public string CompanyOfficerPosition { get; set; }
        public string CompanyOfficerSerialNumber { get; set; }
        public string ShareHolderId { get; set; }
        public string ShareHolderName { get; set; }
        public decimal ShareHolderPercentage { get; set; }
		public string EditId { get; set; }
	}

    public class KycInfoViewModel2
	{
        public KycRequest KycHolder { get; set; }
        public int Id { get; set; }
        public string RequestId { get; set; }
        [Required]
        public string RCNumber { get; set; }
        public string RequesterName { get; set; }
        public string? AssignedLawyerId { get; set; }
        public string? TimeRequested { get; set; }
        public string? LastTimeUpdated { get; set; }
        public string AdditionalInformation { get; set; }
        public KycRequestStatus Status { get; set; }
        public string CompanyObjective { get; set; }
        public string CertificateUrl { get; set; }
        [Required]
        public string CompanyOfficerName { get; set; }
        public string CompanyOfficerPosition { get; set; }
        public string CompanyOfficerSerialNumber { get; set; }
        [Required]
        public string ShareHolderName { get; set; }
        public decimal ShareHolderPercentage { get; set; }
    }





}