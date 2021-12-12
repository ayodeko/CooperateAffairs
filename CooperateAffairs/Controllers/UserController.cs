using CooperateAffairs.Logic;
using DotNetFirebase.Logic;
using DotNetFirebase.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CooperateAffairs.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
        [HttpPost("[action]")]
        public HttpResponseMessage CreateBankUser(BankUser user)
        {
            var responseString = Dekobase.CreateBankUser(user);

            var response = new Models.DekobaseResponse
            {
                ResponseString = responseString,
                ResponseCode = responseString.StartsWith("Exception") ? ResponseMessage.FAILED_CODE : ResponseMessage.SUCCESSFUL_CODE,

            };
            if (responseString.StartsWith("Exception"))
            {
                response.ErrorMessage = responseString;
            }
            var stringContent = new StringContent(JsonConvert.SerializeObject(response));
            return new HttpResponseMessage()
            {
                Content = stringContent
            };
        }

        [HttpPost("[action]")]
        public HttpResponseMessage CreateLawyerUser(LawyerBioData user)
        {
            var responseString = Dekobase.CreateLawyer(user);

            var response = new Models.DekobaseResponse
            {
                ResponseString = responseString,
                ResponseCode = responseString.StartsWith("Exception") ? ResponseMessage.FAILED_CODE : ResponseMessage.SUCCESSFUL_CODE,

            };
            if (responseString.StartsWith("Exception"))
            {
                response.ErrorMessage = responseString;
            }
            var stringContent = new StringContent(JsonConvert.SerializeObject(response));
            return new HttpResponseMessage()
            {
                Content = stringContent
            };
        }


        [HttpPost("[action]")]
        public HttpResponseMessage VerifyUserId(string uid)
        {
            try
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent(Dekobase.VerifyIDToken(uid))
                };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        [HttpPost("[action]")]
        public HttpResponseMessage GetCustomToken(string uid)
        {
            try
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent(Dekobase.GetCustomToken(uid))
                };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }

        [HttpPost("[action]")]
        public HttpResponseMessage SaveData(Models.CompanyBioData inputRequest)
        {
            try
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent(DekoSharp.CreateCompanyBioData(inputRequest, out var statusCode)),
                    StatusCode = statusCode
                };
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent(ex.Message)
                };
            }
        }


    }

    public class ResponseMessage
    {
        public static string SUCCESSFUL_CODE = "00";
        public static string FAILED_CODE = "06";
    }

}
