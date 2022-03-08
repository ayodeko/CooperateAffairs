using System.ComponentModel.DataAnnotations;

namespace CooperateMVC.ViewModels
{
	public class UserLoginViewModel
	{
		public int Id { get; set; }
		[Required]
		[DataType(DataType.EmailAddress)]
		public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }
	}
	public class CreateBankUserViewModel
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
		public string BankOfficerPhoneNumber { get; set; }

		[Required]
		public string BankName { get; set; }
		[Required]
		public string BankOfficerName { get; set; }
	}
	public class EditBankUserProfileViewModel
	{
		[Required]
		public string Uid { get; set; }
		public int Id { get; set; }
		[DataType(DataType.EmailAddress)]
		public string Email { get; set; }
		[DataType(DataType.PhoneNumber)]
		public string BankOfficerPhoneNumber { get; set; }
		public string BankName { get; set; }
		public string BankOfficerName { get; set; }
	}
	public class ChangePasswordViewModel
	{
		[Required]
		public string Uid { get; set; }
		[DataType(DataType.Password)]
		[Required]
		public string Password { get; set; }
		[Required]
		[DataType(DataType.Password)]
		public string ConfirmPassword { get; set; }
	}

	public class EditBankUserViewModel
	{
		public string Uid { get; set; }
		public string BankName { get; set; }
		[Required]
		public string BankOfficerName { get; set; }
	}
}
