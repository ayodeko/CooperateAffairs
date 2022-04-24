using CooperateMVC.Models;
using FireSharp;
using FireSharp.Config;
using Newtonsoft.Json;
using System;
using System.Net;
using CooperateMVC.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace CooperateMVC.Logic { 
public interface IDekoSharp
{
    public string CreateKycRequest(string requester, string rcNumber, string additionalInformation, out HttpStatusCode statusCode);
}

    public class DekoSharp : IDekoSharp
    {
        private readonly FirebaseClient _firebaseClient;
       

        private  FirebaseClient _client = new FirebaseClient(new FirebaseConfig()
        {
            AuthSecret = "RtFON0RXT985SRpm9mU6ZNxSedpuVZnQVt9jOjfz",
            BasePath = "https://fir-dotnet-8146c-default-rtdb.firebaseio.com/",
        });

        public  string SaveCompanyBioData(string dbPath, Models.Models.CompanyBioData data, out HttpStatusCode statusCode)
        {
            var retrieveCompanyData = RetrieveCompanyBioData(data.Name, out var retrieveStatusCode);
            if (!retrieveCompanyData.ToString().Equals("null"))
            {
                statusCode = HttpStatusCode.BadRequest;
                return $"Organization with name {data.Name} already exists: {retrieveCompanyData}";
            }
            var pushResponse = _client.Set($"test/companies/{DekoUtility.ReplaceSpace(data.Name)}", data);
            statusCode = pushResponse.StatusCode;
            return pushResponse.Body;
        }

        public  string SaveDataToPath(string dbPath, object data, out HttpStatusCode statusCode)
        {
            var retrieveCompanyData = RetrieveDataFromPath(dbPath, out var retrieveStatusCode);
            if (retrieveCompanyData != null)
            {
                statusCode = HttpStatusCode.BadRequest;
                return $"data path '{dbPath}' already exists: {retrieveCompanyData}";
            }
            var pushResponse = _client.Set($"{dbPath}", data);
            statusCode = pushResponse.StatusCode;
            return pushResponse.Body;
        }



        public  string CreateCompanyBioData(Models.Models.CompanyBioData data, out HttpStatusCode statusCode)
        {
            var retrieveCompanyData = RetrieveCompanyBioData(data.Name, out var retrieveStatusCode);
            if (!retrieveCompanyData.ToString().Equals("null"))
            {
                statusCode = HttpStatusCode.BadRequest;
                return $"Organization with name {data.Name} already exists: {retrieveCompanyData}";
            }
            var pushResponse = _client.Set($"test/companies/{DekoUtility.ReplaceSpace(data.Name)}", data);
            statusCode = pushResponse.StatusCode;
            return pushResponse.Body;
        }

        public  string UpdateCompanyBioData(Models.Models.CompanyBioData data, out HttpStatusCode statusCode)
        {
            var pushResponse = _client.Set($"test/companies/{DekoUtility.ReplaceSpace(data.Name)}", data);
            statusCode = pushResponse.StatusCode;
            return pushResponse.Body;
        }
        public  string RetrieveCompanyBioData(string companyName, out HttpStatusCode statusCode)
        {
            var getResponse = _client.Get($"test/companies/{DekoUtility.ReplaceSpace(companyName)}");
            statusCode = getResponse.StatusCode;
            return getResponse.Body;
        }

        public  string CreateBankTable(BankUser data, out HttpStatusCode statusCode)
        {
            var pushResponse = _client.Set($"test/banks/{DekoUtility.ReplaceSpace(data.Uid)}", data);
            statusCode = pushResponse.StatusCode;
            return pushResponse.Body;
        }

        public string UpdateBankTable(BankUser bankUser, out HttpStatusCode statusCode)
		{
            try
            {
                var key = bankUser.Uid;
				if (string.IsNullOrEmpty(key))
				{
                    throw new Exception("Update path key is either null or empty");
				}
                var response = UpdateDataToPath($"test/banks/{DekoUtility.CheckNullOrEmpty(DekoUtility.ReplaceSpace(bankUser.Uid))}", DekoUtility.RemoveNulls(bankUser), out var statusCode_);
                statusCode = statusCode_;
                if (statusCode != HttpStatusCode.OK)
                {
                    response = $"Exception, Status Code: {statusCode}";
                }
                return response;
            }
			catch(Exception ex)
			{
                statusCode = HttpStatusCode.InternalServerError;
                return $"Exception: {ex.Message}";
			}
        }

        public  BankUser RetrieveBankFromTable(string uid, out HttpStatusCode statusCode)
        {
            var getResponse = _client.Get($"test/banks/{DekoUtility.ReplaceSpace(uid)}");
            statusCode = getResponse.StatusCode;
            var responseBody = getResponse.Body;
            if(string.IsNullOrEmpty(responseBody) || responseBody.Equals(null))
			{

			}
            var responseObject = JsonConvert.DeserializeObject<BankUser>(responseBody);
            return responseObject;
        }

        public BankUser RetrieveBankWithBankName(string bankName, out HttpStatusCode statusCode)
        {
            try
            {
                var bankString = DekoHttp.FirebaseOrderByEqualToString($"test/banks", "BankName", bankName);
                Console.WriteLine("Request String:" + bankString);
                if (string.IsNullOrEmpty(bankString))
                {
                    statusCode = HttpStatusCode.BadRequest;
                    return null;
                }
                var bankUserHolder = JsonConvert.DeserializeObject<Dictionary<string, BankUser>>(bankString);
                var bankUser = bankUserHolder.Values.FirstOrDefault();

                statusCode = HttpStatusCode.OK;
                return bankUser;
            }
            catch (Exception _)
            {
                statusCode = HttpStatusCode.BadRequest;
                throw;
            }
        }

        public BankUser RetrieveBankWithUid(string uid, out HttpStatusCode statusCode)
        {
            try
            {
                var bankString = DekoHttp.FirebaseOrderByEqualToString($"test/banks", "Uid", uid);
                Console.WriteLine("Request String:" + bankString);
                if (string.IsNullOrEmpty(bankString))
                {
                    statusCode = HttpStatusCode.BadRequest;
                    return null;
                }
                var bankUserHolder = JsonConvert.DeserializeObject<Dictionary<string, BankUser>>(bankString);
                var bankUser = bankUserHolder.Values.FirstOrDefault();

                statusCode = HttpStatusCode.OK;
                return bankUser;
            }
            catch (Exception _)
            {
                statusCode = HttpStatusCode.BadRequest;
                throw;
            }
        }

        public BankUser RetrieveBankUserWithCookie(HttpRequest request)
		{
            var uid = new CookieManager().GetCookie("_firebaseUserId", request);
            if (string.IsNullOrEmpty(uid)) { Console.WriteLine($"cookie uid is null or empty"); return null; }
            var user = new DekoSharp().RetrieveBankFromTable(uid, out var statusCode);
            return user;
        }

        public List<BankUser> RetrieveAllBanksFromTable(out HttpStatusCode statusCode)
        {
            var getResponse = _client.Get($"test/banks");
            statusCode = getResponse.StatusCode;
            var responseBody = getResponse.Body;
			if (string.IsNullOrEmpty(responseBody) || responseBody == "null")
			{
                return new List<BankUser>();
			}
            var responseDictionary = JsonConvert.DeserializeObject<Dictionary<string, BankUser>>(responseBody);
            List<BankUser> bankUsers = new List<BankUser>(responseDictionary.Values);
            return bankUsers;
        }

        public  string CreateLawyerTable(LawyerBioData data, out HttpStatusCode statusCode)
        {
            var pushResponse = _client.Set($"test/lawyers/{DekoUtility.CheckNullOrEmpty(DekoUtility.ReplaceSpace(data.Id))}", DekoUtility.RemoveNulls(data));
            statusCode = pushResponse.StatusCode;
            return pushResponse.Body;
        }

        public  string RetrieveLawyerFromTable(string id, out HttpStatusCode statusCode)
        {
            var getResponse = _client.Get($"test/lawyers/{DekoUtility.ReplaceSpace(id)}");
            statusCode = getResponse.StatusCode;
            return getResponse.Body;
        }

        public LawyerBioData RetrieveLawyerFromTableByName(string name, out HttpStatusCode statusCode)
        {
			try { 
            var lawyerString = DekoHttp.FirebaseOrderByEqualToString($"test/lawyers", "FirmName", name);
            Console.WriteLine("Request String:" + lawyerString);

            if (string.IsNullOrEmpty(lawyerString))
            {
                statusCode = HttpStatusCode.BadRequest;
                return null;
            }
            var lawyerRequestHolder = JsonConvert.DeserializeObject<Dictionary<string, LawyerBioData>>(lawyerString);
            var lawyerRequest = lawyerRequestHolder.Values.FirstOrDefault();

            statusCode = HttpStatusCode.OK;
            return lawyerRequest;
        }
            catch(Exception _)
			{
                statusCode = HttpStatusCode.BadRequest;
				throw;
			}
}

        public  string RetrieveDataFromPath(string dbPath, out HttpStatusCode statusCode)
        {
            var getResponse = _client.Get($"{dbPath}");
            statusCode = getResponse.StatusCode;

            var responseBody = getResponse.Body;
            if(string.IsNullOrEmpty(responseBody) || responseBody == "null") { return null; }
            return responseBody;
        }

        public  string UpdateDataToPath(string dbPath, object data, out HttpStatusCode statusCode)
        {
            var pushResponse = _client.Update($"{dbPath}", data);
            statusCode = pushResponse.StatusCode;
            var responseBody = pushResponse.Body;
            return (string.IsNullOrEmpty(responseBody) || responseBody == "null") ? null : responseBody;
        }

        public string DeleteBankUser(string bankName, out HttpStatusCode statusCode)
        {
            var deleteResponse = _client.Delete($"test/banks/{DekoUtility.ReplaceSpace(bankName)}");
            statusCode = deleteResponse.StatusCode;
            return deleteResponse.Body;
        }

        public  string DeleteCompanyBioData(Models.Models.CompanyBioData company, out HttpStatusCode statusCode)
        {
            var deleteResponse = _client.Delete($"test/companies/{DekoUtility.CheckNullOrEmpty(DekoUtility.ReplaceSpace(company.Name))}");
            statusCode = deleteResponse.StatusCode;
            return deleteResponse.Body;
        }

        public  string ActivateLawyer(string lawyerId, string dbPath)
        {
            try
            {
                var stringResponse = RetrieveDataFromPath("", out var httpStatusCode);
                if (httpStatusCode != HttpStatusCode.OK || string.IsNullOrEmpty(stringResponse))
                    return $"Could not retrieve Lawyer from path {dbPath}";
                var lawyerData = JsonConvert.DeserializeObject<LawyerBioData>(stringResponse);
                lawyerData.IsActive = true;
                var responseString = UpdateDataToPath("", lawyerData, out var statusCode);
                return statusCode == HttpStatusCode.OK ? responseString : $"failed to activate lawyer, statusCode {statusCode}";
            }
            catch (Exception ex)
            {
                return $"Exception caught during lawyer activation {ex.ToString()}";
            }
        }

        public  void InitiateKycRequest(KycInfo kyc, string lawyerId, string requester)
        {

        }

        public string CreateKycRequest(string requester, string rcNumber, string addtionalInformation, out HttpStatusCode statusCode)
        {
            var guid = Guid.NewGuid().ToString();
            var existingData = RetrieveDataFromPath($"request/{DekoUtility.CheckNullOrEmpty(DekoUtility.ReplaceSpace(rcNumber))}",
                out statusCode);
            if (!string.IsNullOrEmpty(existingData))
            {
                return $"Request with RCNumber {DekoUtility.ReplaceSpace(rcNumber)} already exists: {existingData}";
            }
            var kycRequest = new KycRequest
            {
                Status = KycRequest.KycRequestStatus.Pending,
                RequesterName = requester,
                RCNumber = rcNumber,
                TimeRequested = DateTime.Now.ToString(),
                AdditionalInformation = addtionalInformation
            };
            var result = SaveDataToPath($"request/{DekoUtility.CheckNullOrEmpty(DekoUtility.ReplaceSpace(kycRequest.RCNumber))}", DekoUtility.RemoveNulls(kycRequest), out var saveDataStatusCode);
            statusCode = saveDataStatusCode;
            string lawyerId = GenerateLawyerAssginment();
            SendMail(lawyerId);
            return result;
        }

        public KycRequest RetrieveKYCRequestWithRCNumber(string rcNumber, out HttpStatusCode statusCode)
		{
            try
            {
                var kycString = DekoHttp.FirebaseOrderByEqualToString($"request", "RCNumber", rcNumber);
                Console.WriteLine("Request String:" + kycString);
                if (string.IsNullOrEmpty(kycString))
				{
                    statusCode = HttpStatusCode.BadRequest;
                    return null;
				}
                var kycRequestHolder = JsonConvert.DeserializeObject<Dictionary<string, KycRequest>>(kycString);
                var kycRequest = kycRequestHolder.Values.FirstOrDefault();

                statusCode = HttpStatusCode.OK;
                return kycRequest;
            }
            catch(Exception _)
			{
                statusCode = HttpStatusCode.BadRequest;
				throw;
			} 
        }

		private string GenerateLawyerAssginment()
		{
            return "";
		}

		public  string UploadKycInfo(UploadKycRequest inputRequest, out HttpStatusCode statusCode)
        {
            var kycData = RetrieveKycRequestByRcNumber(inputRequest.RCNumber, out var statusCode2);
            if (kycData == null || kycData.ToString().Equals("null"))
            {
                statusCode = statusCode2;
                return $"Exception: Request with RCNumber {DekoUtility.ReplaceSpace(inputRequest.RCNumber)} does not exist";
            }
            kycData.KycInfo = inputRequest.KycInfo;
            if(kycData.KycInfo != null) { kycData.KycInfo.RCNumber = inputRequest.RCNumber; }
            kycData.Status = KycRequest.KycRequestStatus.Processing;
            kycData.LastTimeUpdated = DateTime.Now.ToString();
            //var pushResponse = _client.Update($"request/{DekoUtility.ReplaceSpace(inputRequest.RCNumber)}", kycRequest);
            var pushResponse = DekoHttp.FirebaseHttpPatch($"request/{DekoUtility.CheckNullOrEmpty(DekoUtility.ReplaceSpace(inputRequest.RCNumber))}", DekoUtility.RemoveNulls(kycData));
            statusCode = HttpStatusCode.OK;
            return pushResponse;
        }

        public KycRequest RetrieveKycRequestByRcNumber(string rcNumber, out HttpStatusCode statusCode)
        {
            var getResponse = _client.Get($"request/{DekoUtility.ReplaceSpace(rcNumber)}");
            statusCode = getResponse.StatusCode;
            var responseBody = getResponse.Body;
            if(responseBody == "null" || string.IsNullOrEmpty(responseBody))
			{
                return null;
			}
            return JsonConvert.DeserializeObject<KycRequest>(responseBody);
        }
        public List<KycRequest> RetrieveAllKycRequest(out HttpStatusCode statusCode)
        {
            var getResponse = _client.Get($"request");
            statusCode = getResponse.StatusCode;
            var responseBody = getResponse.Body;

            if (string.IsNullOrEmpty(responseBody) || responseBody == "null")
            {
                return new List<KycRequest>();
            }
            var responseDictionary = JsonConvert.DeserializeObject<Dictionary<string, KycRequest>>(responseBody);
            List<KycRequest> kycRequests = new List<KycRequest>(responseDictionary.Values);
            return kycRequests;
        }
        public  string RetrievePendingKycRequests(out HttpStatusCode statusCode)
        {
            var getResponse = DekoHttp.FirebaseOrderByEqualTo($"request", "Status", (int)KycRequest.KycRequestStatus.Pending);
            statusCode = HttpStatusCode.OK;
            return getResponse;
        }



        public string DeleteKycRequest(string rcNumber, out HttpStatusCode statusCode)
        {
            var deleteResponse = _client.Delete($"request/{DekoUtility.CheckNullOrEmpty(DekoUtility.ReplaceSpace(rcNumber))}");
            statusCode = deleteResponse.StatusCode;
            return deleteResponse.Body;
        }

        void ChangeRequestStatus(string requestId, KycRequest.KycRequestStatus status)
        {

            var update = new
            {
                Status = status
            };
            var updateResponse = _client.Update($"request/{requestId}", update);
        }

        public  string AssignRequestToLawyerId(string lawyerId, KycRequest kycRequest, out HttpStatusCode statusCode)
        {
            var statusAndLawyerId = new
            {
                statusAndLawyerId = $"{KycRequest.KycRequestStatus.Pending}|{lawyerId}"
            };

            var pushResponse = _client.Set($"Lawyer/{lawyerId}/Request", kycRequest);
            statusCode = pushResponse.StatusCode;
            return pushResponse.Body;
        }



        public  void SendMail(string lawyerId)
        {

        }
    }
}
