using Firebase.Auth;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CooperateMVC.Logic
{
	public class FirebaseAuthSetup
	{
		public static async Task<SignInStatus> AccessFirebase(LoginViewModel model, HttpContext context)
		{
			string webApiKey = "AIzaSyAc5-NKgx5pkomSHGNppvlTtmEPwR1O2hE";
			FirebaseAuthProvider authProvider = new FirebaseAuthProvider(new FirebaseConfig(webApiKey));
			try
			{
				FirebaseAuthLink auth = await authProvider.SignInWithEmailAndPasswordAsync(model.Email, model.Password);
				string token = auth.FirebaseToken;
				if (token != null)
				{
					context.Session.SetString("_UserToken", token);
				}
			}
			catch (FirebaseAuthException)
			{
				return SignInStatus.Failure;
			}
			return SignInStatus.Success;
		}
	}

	public class LoginViewModel
	{
		public string Email { get; internal set; }
		public string Password { get; internal set; }
	}
}
