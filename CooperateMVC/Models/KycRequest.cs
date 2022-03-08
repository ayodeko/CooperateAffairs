using System.ComponentModel.DataAnnotations;

namespace CooperateMVC.Models
{
    public class KycRequest
    {
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

    public class UploadKycRequest
	{
        public string RCNumber { get; set; }
        public string LawyerID { get; set; }
        public KycInfo KycInfo { get; set; }
	}


}