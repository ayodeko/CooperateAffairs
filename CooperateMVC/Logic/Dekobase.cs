using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using CooperateMVC.Logic;
using CooperateMVC.Models;

using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace CooperateMVC.Logic
{
    public class Dekobase
    {
        DekoSharp _dekoSharp = new DekoSharp();
        private string CreateUserInternal(UserRecordArgs userRecords)
        {
                var claims = new Dictionary<string, object> {{"role", "admin"}};
                UserRecord userRecord = FirebaseAuth.DefaultInstance.CreateUserAsync(userRecords).Result;
                FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(userRecord.Uid, claims);
                return userRecord.Uid;
            
           
        }

        public string CreateBankUser(HttpContext context, BankUser bankUser)
        {
            try
            {
                var retrievedBank = _dekoSharp.RetrieveBankFromTable(bankUser.BankName, out var statusCode);
                if (retrievedBank != null)
                {
                    return $"Exception: BankName already exists";
                }
                DekoUtility.LogInfo("Inside CreateBankUser");
                var userRecords = new UserRecordArgs
                {
                    Email = bankUser.Email,
                    Password = bankUser.Password,
                    DisplayName = bankUser.BankName,
                    PhoneNumber = bankUser.PhoneNumber
                };
                var claims = new Dictionary<string, object> {{"role", "bank"}};
                UserRecord userRecord = FirebaseAuth.DefaultInstance.CreateUserAsync(userRecords).Result;
                var uid = userRecord.Uid;
                bankUser.Uid = uid;
                FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(uid, claims);
                var result = _dekoSharp.CreateBankTable(bankUser, out _);
                DekoUtility.LogInfo(result);
                return result;
            }
            catch (Exception ex)
            {
                DekoUtility.LogError(ex);
                return ex.InnerException != null
                    ? $"Exception: {ex.InnerException.Message}"
                    : $"Exception: {ex.ToString()}";
            }
        }


        public string UpdateBankUser(HttpContext context, BankUser bankUser)
		{
			try
			{
                var retrievedBank = _dekoSharp.RetrieveBankWithUid(bankUser.Uid, out var statusCode);
                if (retrievedBank == null)
                {
                    throw new Exception($"Exception: Cannot update because BankName does not exist in DB");
                }
                DekoUtility.LogInfo("Inside CreateBankUser");
                var userArgs = new UserRecordArgs
                {
                    Uid = bankUser.Uid,
                    Email = DekoUtility.ConvertWhiteSpaceOrEmptyToNull(bankUser.Email),
                    Password = DekoUtility.ConvertWhiteSpaceOrEmptyToNull(bankUser.Password),
                    DisplayName = DekoUtility.ConvertWhiteSpaceOrEmptyToNull(bankUser.BankName),
                    PhoneNumber = DekoUtility.ConvertWhiteSpaceOrEmptyToNull(bankUser.PhoneNumber)
                };
                UserRecord userRecord = FirebaseAuth.DefaultInstance.UpdateUserAsync(userArgs).Result;
                var result = _dekoSharp.UpdateBankTable(bankUser, out statusCode);
                if(statusCode != HttpStatusCode.OK)
				{
                    Console.WriteLine("Error while updating bankUser on DB");
                    return $"Exception, HttpStatusCode:{statusCode}";
                }
                Console.WriteLine("Successfully updated user info: " + userRecord.Uid);
                return JsonConvert.SerializeObject(userRecord);
            }
			catch(Exception ex)
			{
                return $"Exception: {ex.Message}";
			}
		}


        public string CreateLawyer(LawyerBioData user)
        {
            try
            {
                if(!_dekoSharp.RetrieveLawyerFromTable(user.FirmName, out var statusCode).ToString().Equals("null"))
                {
                    statusCode = HttpStatusCode.BadRequest;
                    return $"Organization with name {user.FirmName} already exists";
                }
                var userRecords = new UserRecordArgs
                {
                    Email = user.Email,
                    Password = user.Password,
                    DisplayName = user.DisplayName,
                    PhoneNumber = user.PhoneNumber
                };
                var claims = new Dictionary<string, object> {{"role", "lawyer"}};
                UserRecord userRecord = FirebaseAuth.DefaultInstance.CreateUserAsync(userRecords).Result;
                FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(userRecord.Uid, claims);
                var result = _dekoSharp.CreateLawyerTable(user, out _);
                DekoUtility.LogInfo(result);
                return result;
            }
            catch (Exception ex)
            {
                DekoUtility.LogError(ex);
                return ex.InnerException != null
                    ? $"Exception: {ex.InnerException.Message}"
                    : $"Exception: {ex.ToString()}";
            }
        }


        void GetIDToken(string customToken)
        {
            //FireSharp.FirebaseClient = new FireSharp.FirebaseClient(new FirebaseConfig(){});
        }

        public string CreateUserWithAuth(string signedInUserID, UserRecordArgs userRecords)
        {
            try
            {
                var verifiedToken = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(signedInUserID).Result;
                var customClaims = verifiedToken.Claims;
                if (customClaims.TryGetValue("role", out var role) && ((string) role == "admin"))
                {
                    return $"Create user with uid: {CreateUserInternal(userRecords)}";
                }
                else
                {
                    return "UnAuthorized";
                }
            }
            catch (Exception ex)
            {
                return $"Exception caught: {ex.ToString()}";
            }
        }

        public void CookieSignOut(HttpContext context)
		{
            new CookieManager().DeleteCookie("_firebaseUserId",context.Response);
            new CookieManager().DeleteCookie("_firebaseUser", context.Response);
        }

        public async System.Threading.Tasks.Task<string> SignInUserAsync(HttpContext context, string email, string password)
        {
                string webApiKey = "AIzaSyAc5-NKgx5pkomSHGNppvlTtmEPwR1O2hE";
                Firebase.Auth.FirebaseAuthProvider authProvider = new Firebase.Auth.FirebaseAuthProvider(new Firebase.Auth.FirebaseConfig(webApiKey));
                Firebase.Auth.FirebaseAuthLink auth = await authProvider.SignInWithEmailAndPasswordAsync(email, password);
                var token = auth.FirebaseToken;
                var userId = auth.User.LocalId;
                Console.WriteLine("LocalId: " + userId);
                new CookieManager().SetCookie("_firebaseUserId", userId, context.Response, auth.ExpiresIn);
                new CookieManager().SetCookie("_firebaseUser", token, context.Response, auth.ExpiresIn);
                return token;
            
        }

        public string GetCustomToken(string uid) => FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(uid).Result;

        public string VerifyIDToken(string id)
        {
            try
            {
                var verifiedToken = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(id, true).Result;
                var customClaims = verifiedToken.Claims;
                var result = customClaims.Aggregate("",
                    (current, claim) => current + $" Key: {claim.Key}, Value: {claim.Value}");
                return result;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public IEnumerable<object> GetUserClaims(string id, out bool isAuthorized)
        {
            try
            {
                var verifiedToken = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(id, true).Result;
                var customClaims = verifiedToken.Claims;
                var result = customClaims.Aggregate("",
                    (current, claim) => current + $" Key: {claim.Key}, Value: {claim.Value}");
                Console.WriteLine(result);
                isAuthorized = true;
                return customClaims.Values;
            }
            catch (Exception ex)
            {
                isAuthorized = false;
                return null;
            }
        }

        public string VerifyUser(string uId)
        {
            try
            {
                var claims = new Dictionary<string, object>();
                claims.Add("role", "user");
                claims.Add("verified", "true");
                var result = FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(uId, claims).Status;
                return result.ToString();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}