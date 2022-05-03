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
using static CooperateMVC.Models.KycRequest;
using System.Threading.Tasks;

namespace CooperateMVC.Logic {
    public interface IDekoSharp
    {
        public Task<string> CreateKycRequest(string requester, string requesterId, string rcNumber, string addtionalInformation);
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
            var pushResponse = _client.Set($"test/lawyers/{DekoUtility.CheckNullOrEmpty(DekoUtility.ReplaceSpace(data.Uid))}", DekoUtility.RemoveNulls(data));
            statusCode = pushResponse.StatusCode;
            return pushResponse.Body;
        }
        public string UpdeateLawyerTable(LawyerBioData data, out HttpStatusCode statusCode)
        {
            var pushResponse = _client.Update($"test/lawyers/{DekoUtility.CheckNullOrEmpty(DekoUtility.ReplaceSpace(data.Uid))}", DekoUtility.RemoveNulls(data));
            statusCode = pushResponse.StatusCode;
            return pushResponse.Body;
        }
        public async Task<string> IncreaseLawyerKycStatusAsync(KycRequestStatus status, string uid)
        {
            Console.WriteLine("Inside IncreaseLawyerKycStatusAsync method..");
            var statusData = await RetrieveLawyerByIdAsync(uid);
            var dataHolder = new LawyerBioData();
            if (statusData == null)
			{
                Console.WriteLine("Increase lawyer data is null");
                //statusCode = HttpStatusCode.BadRequest;
                return null;
			}
			switch (status)
			{
				case KycRequestStatus.Pending:
                    dataHolder = new LawyerBioData { PendingStatusCount = (statusData.PendingStatusCount + 1) };
                    Console.WriteLine($"Increase Pending count of {dataHolder.PendingStatusCount}");
                    break;
				case KycRequestStatus.Processing:
                    dataHolder = new LawyerBioData { 
                        PendingStatusCount = (statusData.PendingStatusCount - 1),
                        ProcessingStatusCount = (statusData.ProcessingStatusCount + 1)
                    };
                    Console.WriteLine($"Decrease Pending count of {dataHolder.PendingStatusCount}");
                    Console.WriteLine($"Increase Processing count of {dataHolder.ProcessingStatusCount}");
                    break;
				case KycRequestStatus.Completed:
                    dataHolder = new LawyerBioData
                    {
                        ProcessingStatusCount = (statusData.ProcessingStatusCount - 1),
                        CompletedStatusCount = (statusData.CompletedStatusCount + 1)
                    };
                    Console.WriteLine($"Decrease Processing count of {dataHolder.PendingStatusCount}");
                    Console.WriteLine($"Increase Completed count of {dataHolder.CompletedStatusCount}");
                    break;
				case KycRequestStatus.Exception:
					break;
				default:
                    Console.WriteLine("Inside default switch");
					break;
			}
            Console.WriteLine("Increase Status Data:  " + JsonConvert.SerializeObject(DekoUtility.RemoveNulls(dataHolder)));
            var pushResponse = await _client.UpdateAsync($"test/lawyers/{DekoUtility.CheckNullOrEmpty(DekoUtility.ReplaceSpace(statusData.Uid))}", DekoUtility.RemoveNulls(dataHolder));
            //statusCode = pushResponse.StatusCode;
            return pushResponse.Body;
        }

        public async Task<LawyerBioData> RetrieveLawyerByIdAsync(string id)
        {
            try
            {
                var getResponse = await _client.GetAsync($"test/lawyers/{DekoUtility.ReplaceSpace(id)}");
                //statusCode = getResponse.StatusCode;
                var lawyerString = getResponse.Body;
                if (string.IsNullOrEmpty(lawyerString))
                {
                    //statusCode = HttpStatusCode.BadRequest;
                    return null;
                }
                var lawyerRequest = JsonConvert.DeserializeObject<LawyerBioData>(lawyerString);
                //statusCode = HttpStatusCode.OK;
                return lawyerRequest;
            }
			catch
			{
                //statusCode = HttpStatusCode.BadRequest;
                throw;
            }
        }

        public async Task<List<LawyerBioData>> RetrieveAllLawyerAsync()
        {
            try
            {
                var getResponse = await _client.GetAsync($"test/lawyers");
                //statusCode = getResponse.StatusCode;
                var lawyerString = getResponse.Body;
                if (string.IsNullOrEmpty(lawyerString))
                {
                    //statusCode = HttpStatusCode.BadRequest;
                    return null;
                }
                var lawyerRequestHolder = JsonConvert.DeserializeObject<Dictionary<string, LawyerBioData>>(lawyerString);
                var lawyerRequest = new List<LawyerBioData>(lawyerRequestHolder.Values);
                //statusCode = HttpStatusCode.OK;
                return lawyerRequest;
            }
            catch
            {
                //statusCode = HttpStatusCode.BadRequest;
                throw;
            }
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

        public async Task<string> CreateKycRequest(string requester, string requesterId, string rcNumber, string addtionalInformation)
        {
            var guid = Guid.NewGuid().ToString();
            string lawyerId = await GenerateLawyerAssginment();
            Console.WriteLine($"Generated Assigned Lawyer UID: {lawyerId}");
            var existingData = RetrieveDataFromPath($"request/{DekoUtility.CheckNullOrEmpty(DekoUtility.ReplaceSpace(rcNumber))}",
                out var statusCode);
            if (!string.IsNullOrEmpty(existingData))
            {
                throw new InvalidOperationException( $"Request with RCNumber {DekoUtility.ReplaceSpace(rcNumber)} already exists");
            }
            var kycRequest = new KycRequest
            {
                Status = KycRequest.KycRequestStatus.Pending,
                RequesterName = requester,
                RequesterId = requesterId,
                RCNumber = rcNumber,
                TimeRequested = DateTime.Now.ToString(),
                AdditionalInformation = addtionalInformation,
                AssignedLawyerId = lawyerId
            };
            var result = SaveDataToPath($"request/{DekoUtility.CheckNullOrEmpty(DekoUtility.ReplaceSpace(kycRequest.RCNumber))}", DekoUtility.RemoveNulls(kycRequest), out var saveDataStatusCode);
            statusCode = saveDataStatusCode;
            await IncreaseLawyerKycStatusAsync(KycRequestStatus.Pending, lawyerId);
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

		private async Task<string> GenerateLawyerAssginment()
		{
            var lawyerList = await RetrieveAllLawyerAsync();
            Console.WriteLine("LawyerList Count: " + lawyerList.Count);
            var pendingLawyer = lawyerList.Where(y => y.PendingStatusCount == 0).FirstOrDefault();
            if (pendingLawyer != null) return pendingLawyer.Uid;
            var pendingCountLawyer = lawyerList.OrderBy(x => x.PendingStatusCount).FirstOrDefault();
            if (pendingCountLawyer != null) return pendingCountLawyer.Uid;
            var processingLawyer = lawyerList.Where(y => y.ProcessingStatusCount == 0).FirstOrDefault();
            if (processingLawyer != null) return processingLawyer.Uid;
            var processingCountLawyer = lawyerList.OrderBy(x => x.ProcessingStatusCount).FirstOrDefault();
            if (processingCountLawyer != null) return processingCountLawyer.Uid;
            var completedLawyer = lawyerList.OrderBy(x => x.CompletedStatusCount).FirstOrDefault();
            if (completedLawyer != null) return completedLawyer.Uid;
            var defaultLawyer = lawyerList.FirstOrDefault();
            if (defaultLawyer != null) return defaultLawyer.Uid;
            throw new Exception("GenerateLawyerAssignment Exception, No Assignable lawyer for the request");
		}

		public  string UploadKycInfo(UploadKycRequest inputRequest, HttpRequest request, out HttpStatusCode statusCode)
        {
            var kycData = RetrieveKycRequestByRcNumber(inputRequest.RCNumber, out var statusCode2);
            if (kycData == null || kycData.ToString().Equals("null"))
            {
                statusCode = statusCode2;
                return $"Exception: Request with RCNumber {DekoUtility.ReplaceSpace(inputRequest.RCNumber)} does not exist";
            }   
            kycData.KycInfo = inputRequest.KycInfo;
            if(kycData.KycInfo != null) { kycData.KycInfo.RCNumber = inputRequest.RCNumber; }
            kycData.LastTimeUpdated = DateTime.Now.ToString();
            //var pushResponse = _client.Update($"request/{DekoUtility.ReplaceSpace(inputRequest.RCNumber)}", kycRequest);
            if (kycData.Status == KycRequestStatus.Pending)
            {
                Console.WriteLine("Change Pending to processing");
                kycData.Status = KycRequest.KycRequestStatus.Processing;

                var uid = new CookieManager().GetCookie("_firebaseUserId", request);
                IncreaseLawyerKycStatusAsync(KycRequest.KycRequestStatus.Processing, uid);
            }
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

        public List<KycRequest> RetrieveKycRequestByRequesterId(string requesterId, out HttpStatusCode statusCode)
        {
            //var getResponse = _client.Get($"request");
            var kycListString = DekoHttp.FirebaseOrderByEqualToString($"request", "RequesterId", requesterId);
			if (kycListString.StartsWith("Error") || string.IsNullOrEmpty(kycListString))
			{
                statusCode = HttpStatusCode.BadRequest;
                return null;
			}

            var responseDictionary = JsonConvert.DeserializeObject<Dictionary<string, KycRequest>>(kycListString);
            List<KycRequest> kycRequests = new List<KycRequest>(responseDictionary.Values);
            statusCode = HttpStatusCode.OK;
            return kycRequests;
        }

        public List<KycRequest> RetrieveKycRequestByAssignedLawyerId(string assignedLawyerId, out HttpStatusCode statusCode)
        {
            //var getResponse = _client.Get($"request");
            var kycListString = DekoHttp.FirebaseOrderByEqualToString($"request", "AssignedLawyerId", assignedLawyerId);
            if (kycListString.StartsWith("Error") || string.IsNullOrEmpty(kycListString))
            {
                statusCode = HttpStatusCode.BadRequest;
                return null;
            }

            var responseDictionary = JsonConvert.DeserializeObject<Dictionary<string, KycRequest>>(kycListString);
            List<KycRequest> kycRequests = new List<KycRequest>(responseDictionary.Values);
            statusCode = HttpStatusCode.OK;
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
