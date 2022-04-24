using System.ComponentModel.DataAnnotations;

namespace CooperateMVC.ViewModels
{
	public class LawyerViewModels
	{
	}

	public class CreateLawyererViewModel
	{
		public string Uid { get; set; }
		public int Id { get; set; }
		[Required]
		[DataType(DataType.EmailAddress)]
		public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Required]
		[DataType(DataType.PhoneNumber)]
		public string LawyerPhoneNumber { get; set; }

		[Required]
		public string FirmName { get; set; }
		[Required]
		public string FirmContactName { get; set; }

		public string City { get; set; }
		public string State { get; set; }
		public string FullAddress { get; set; }
	}
}
