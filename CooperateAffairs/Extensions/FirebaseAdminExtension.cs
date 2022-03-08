using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace CooperateAffairs.Extensions
{
	public static class FirebaseAdminExtension
	{
		public static void FirebaseInitialize(this IApplicationBuilder applicationBuilder)
		{
			var stockPath = AppDomain.CurrentDomain.BaseDirectory;
			var jsonPath = "fir-dotnet-8146c-firebase-adminsdk-5i6y8-bed6b12344.json";
			FirebaseApp.Create(new AppOptions
			{
				Credential = GoogleCredential.FromFile(Path.Combine(stockPath, jsonPath))
			});
		}
	}
}
