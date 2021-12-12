using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using CooperateAffairs.Logic;
using DotNetFirebase.Models;
using FirebaseAdmin;
using FirebaseAdmin.Auth;

namespace DotNetFirebase.Logic
{
    public class Dekobase
    {
        private string CreateUserInternal(UserRecordArgs userRecords)
        {
            try
            {
                var claims = new Dictionary<string, object> {{"role", "admin"}};
                UserRecord userRecord = FirebaseAuth.DefaultInstance.CreateUserAsync(userRecords).Result;
                FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(userRecord.Uid, claims);
                return userRecord.Uid;
            }
            catch (Exception ex)
            {
                return $"Exception caught: {ex.ToString()}";
            }
        }

        public static string CreateBankUser(BankUser bankUser)
        {
            try
            {
                DekoUtility.LogInfo("Inside CreateBankUser");
                var userRecords = new UserRecordArgs
                {
                    Email = bankUser.Email,
                    Password = bankUser.Password,
                    DisplayName = bankUser.DisplayName,
                    PhoneNumber = bankUser.PhoneNumber
                };
                var claims = new Dictionary<string, object> {{"role", "bank"}};
                UserRecord userRecord = FirebaseAuth.DefaultInstance.CreateUserAsync(userRecords).Result;
                FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(userRecord.Uid, claims);
                var result = DekoSharp.CreateBankTable(bankUser, out _);
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


        public static string CreateLawyer(LawyerBioData user)
        {
            try
            {
                if(!DekoSharp.RetrieveLawyerFromTable(user.FirmName, out var statusCode).ToString().Equals("null"))
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
                var result = DekoSharp.CreateLawyerTable(user, out _);
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

        public void SignInUser()
        {
        }

        public static string GetCustomToken(string uid) => FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(uid).Result;

        public static string VerifyIDToken(string id)
        {
            try
            {
                var verifiedToken = FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(id).Result;
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