namespace DotNetFirebase.Models
{
    public class Models
    {
        public class CreateUserRequest
        {
            public string CurrentUserUID { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string DisplayName { get; set; }
            public string Password { get; set; }
        }

        public class DekobaseResponse
        {
            public string ResponseCode { get; set; }
            public string ResponseString { get; set; }
            public string ErrorMessage { get; set; }
        }

        public class CompanyBioData
        {
            public string Name { get; set; }
            public string Location { get; set; }
            public string CompanyType { get; set; }
            public int NumberOfBoardMembers { get; set; }
        }

    }
}