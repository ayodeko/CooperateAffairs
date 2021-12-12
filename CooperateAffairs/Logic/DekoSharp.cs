using CooperateAffairs.Configs;
using DotNetFirebase.Logic;
using DotNetFirebase.Models;
using FireSharp;
using FireSharp.Config;
using Newtonsoft.Json;
using System.Net;

namespace CooperateAffairs.Logic;

public interface IDekoSharp
{
    public string CreateKycRequest(string requester, string rcNumber, string additionalInformation, out HttpStatusCode statusCode);
}

public class DekoSharp : IDekoSharp
{

    private static FirebaseConfig _config = new FirebaseConfig()
    {
        AuthSecret = "RtFON0RXT985SRpm9mU6ZNxSedpuVZnQVt9jOjfz",
        BasePath = "https://fir-dotnet-8146c-default-rtdb.firebaseio.com/",
    };
    static DekoSettings _dekoSettings;
    private readonly IConfiguration _configuration;
    private readonly FirebaseClient _firebaseClient;
    public DekoSharp(IConfiguration configuration)
    {
        _configuration = configuration;
    }

 //   void SetConfig()
	//{
 //       _config = new FirebaseConfig()
 //       {
 //           AuthSecret = GetConfig("FirebaseSettings:FirebaseAuthSecret"),
 //           BasePath = GetConfig("FirebaseSettings:FirebaseBaseURL"),
 //       };
 //   }

    FirebaseConfig GetConfig()
	{
        return new FirebaseConfig()
        {
            AuthSecret = _configuration["FirebaseSettings:FirebaseAuthSecret"],
            BasePath = _configuration["FirebaseSettings:FirebaseBaseURL"],
        };
    }

    public string GetConfig(string key) => _configuration[key];

    private static FirebaseClient _client = new FirebaseClient(_config);

    public static string SaveCompanyBioData(string dbPath, Models.CompanyBioData data, out HttpStatusCode statusCode)
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

    public static string SaveDataToPath(string dbPath, object data, out HttpStatusCode statusCode)
    {
        var retrieveCompanyData = RetrieveDataFromPath(dbPath, out var retrieveStatusCode);
        if (!retrieveCompanyData.ToString().Equals("null"))
        {
            statusCode = HttpStatusCode.BadRequest;
            return $"data path '{dbPath}' already exists: {retrieveCompanyData}";
        }
        var pushResponse = _client.Set($"{dbPath}", data);
        statusCode = pushResponse.StatusCode;
        return pushResponse.Body;
    }



    public static string CreateCompanyBioData(Models.CompanyBioData data, out HttpStatusCode statusCode)
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

    public static string UpdateCompanyBioData(Models.CompanyBioData data, out HttpStatusCode statusCode)
    {
        var pushResponse = _client.Set($"test/companies/{DekoUtility.ReplaceSpace(data.Name)}", data);
        statusCode = pushResponse.StatusCode;
        return pushResponse.Body;
    }
    public static string RetrieveCompanyBioData(string companyName, out HttpStatusCode statusCode)
    {
        var getResponse = _client.Get($"test/companies/{DekoUtility.ReplaceSpace(companyName)}");
        statusCode = getResponse.StatusCode;
        return getResponse.Body;
    }

    public static string CreateBankTable(BankUser data, out HttpStatusCode statusCode)
    {
        var pushResponse = _client.Set($"test/banks/{DekoUtility.ReplaceSpace(data.BankName)}", data);
        statusCode = pushResponse.StatusCode;
        return pushResponse.Body;
    }

    public static string RetrieveBankFromTable(string companyName, out HttpStatusCode statusCode)
    {
        var getResponse = _client.Get($"test/banks/{DekoUtility.ReplaceSpace(companyName)}");
        statusCode = getResponse.StatusCode;
        return getResponse.Body;
    }

    public static string CreateLawyerTable(LawyerBioData data, out HttpStatusCode statusCode)
    {
        var pushResponse = _client.Set($"test/lawyers/{DekoUtility.ReplaceSpace(data.FirmName)}", data);
        statusCode = pushResponse.StatusCode;
        List<string> f = new List<string>();
        return pushResponse.Body;
    }

    public static string RetrieveLawyerFromTable(string companyName, out HttpStatusCode statusCode)
    {
        var getResponse = _client.Get($"test/lawyers/{DekoUtility.ReplaceSpace(companyName)}");
        statusCode = getResponse.StatusCode;
        return getResponse.Body;
    }

    public static string RetrieveDataFromPath(string dbPath, out HttpStatusCode statusCode)
    {
        var getResponse = _client.Get($"{dbPath}");
        statusCode = getResponse.StatusCode;
        return getResponse.Body;
    }

    public static string UpdateDataToPath(string dbPath, object data, out HttpStatusCode statusCode)
    {
        var pushResponse = _client.Set($"{dbPath}", data);
        statusCode = pushResponse.StatusCode;
        return pushResponse.Body;
    }

    public static string DeleteCompanyBioData(Models.CompanyBioData company, out HttpStatusCode statusCode)
    {
        var deleteResponse = _client.Delete($"test/companies/{DekoUtility.ReplaceSpace(company.Name)}");
        statusCode = deleteResponse.StatusCode;
        return deleteResponse.Body;
    }

    public static string ActivateLawyer(string lawyerId, string dbPath)
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

    public static void InitiateKycRequest(KycInfo kyc, string lawyerId, string requester)
    {

    }

    public string CreateKycRequest(string requester, string rcNumber, string addtionalInformation, out HttpStatusCode statusCode)
    {
            var existingData = RetrieveDataFromPath($"request/{DekoUtility.ReplaceSpace(rcNumber)}",
                out statusCode);
            if (!existingData.ToString().Equals("null"))
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
            var result = SaveDataToPath($"request/{DekoUtility.ReplaceSpace(kycRequest.RCNumber)}", kycRequest, out var saveDataStatusCode);
            statusCode = saveDataStatusCode;
        string lawyerId = "";
            SendMail(lawyerId);
            return result;
    }

    public static string UploadKycInfo(UploadKycRequest inputRequest, out HttpStatusCode statusCode)
    {
        var existingData = RetrieveKycRequestByRcNumber(inputRequest.RCNumber, out var statusCode2);
        if (existingData.ToString().Equals("null"))
        {
            statusCode = statusCode2;
            return $"Request with RCNumber {DekoUtility.ReplaceSpace(inputRequest.RCNumber)} already exists: {existingData}";
        }
        var kycRequest = new
        {
            Status = KycRequest.KycRequestStatus.Processing,
            KycInfo = inputRequest.KycInfo,
            LastTimeUpdated = DateTime.Now.ToString()
        };
        //var pushResponse = _client.Update($"request/{DekoUtility.ReplaceSpace(inputRequest.RCNumber)}", kycRequest);
        var pushResponse = DekoHttp.FirebaseHttpPut($"request/{DekoUtility.ReplaceSpace(inputRequest.RCNumber)}", kycRequest);
        statusCode = HttpStatusCode.OK;
        return pushResponse;
    }

    public static string RetrieveKycRequestByRcNumber(string rcNumber, out HttpStatusCode statusCode)
    {
        var getResponse = _client.Get($"request/{DekoUtility.ReplaceSpace(rcNumber)}");
        statusCode = getResponse.StatusCode;
        return getResponse.Body;
    }
    public static string RetrieveAllKycRequest(out HttpStatusCode statusCode)
    {
        var getResponse = _client.Get($"request");
        statusCode = getResponse.StatusCode;
        return getResponse.Body;
    }
    public static string RetrievePendingKycRequests(out HttpStatusCode statusCode)
    {
        var getResponse = DekoHttp.FirebaseOrderByEqualTo($"request", "Status", (int) KycRequest.KycRequestStatus.Pending);
        statusCode = HttpStatusCode.OK;
        return getResponse;
    }


    static void ChangeRequestStatus(string requestId, KycRequest.KycRequestStatus status)
    {
        
        var update = new
        {
            Status = status
        };
        var updateResponse = _client.Update($"request/{requestId}", update);
    }

    public static string AddRequestToLawyerId(string lawyerId, KycRequest kycRequest, out HttpStatusCode statusCode)
    {
        var pushResponse = _client.Set($"Lawyer/{lawyerId}/Request", kycRequest);
        statusCode = pushResponse.StatusCode;
        return pushResponse.Body;
    }


    public static void SendMail(string lawyerId)
    {

    }
}
