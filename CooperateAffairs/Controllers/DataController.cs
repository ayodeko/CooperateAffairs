using CooperateAffairs.Logic;
using DotNetFirebase.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace CooperateAffairs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        IDekoSharp _dekoSharp;
		public DataController(IDekoSharp dekoSharp)
		{
            _dekoSharp = dekoSharp;
		}
        [HttpPost("[action]")]
        public HttpResponseMessage CreateCompanyBioData(Models.CompanyBioData inputRequest)
        {
            try
            {
                var response = new Models.DekobaseResponse
                {
                    ResponseString = DekoSharp.CreateCompanyBioData(inputRequest, out var statusCode),
                    ResponseCode = (statusCode == HttpStatusCode.OK) ? "00" : "06"
                };
                return new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(response)),
                    StatusCode = statusCode
                };
            }
            catch (Exception ex)
            {
                var response = new Models.DekobaseResponse
                {
                    ResponseString = ex.ToString(),
                    ResponseCode = "06",
                    ErrorMessage = ex.ToString()
                };
                return new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(response)),
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }

        [HttpPost("[action]")]
        public HttpResponseMessage UpdateCompanyBioData(Models.CompanyBioData inputRequest)
        {
            try
            {
                var response = new Models.DekobaseResponse
                {
                    ResponseString = DekoSharp.UpdateCompanyBioData(inputRequest, out var statusCode),
                    ResponseCode = (statusCode == HttpStatusCode.OK) ? "00" : "06"
                };
                return new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(response)),
                    StatusCode = statusCode
                };
            }
            catch (Exception ex)
            {
                var response = new Models.DekobaseResponse
                {
                    ResponseString = ex.ToString(),
                    ResponseCode = "06",
                    ErrorMessage = ex.ToString()
                };
                return new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(response)),
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }
        [HttpPost("[action]")]
        public HttpResponseMessage DeleteCompanyBioData(Models.CompanyBioData inputRequest)
        {
            try
            {
                var response = new Models.DekobaseResponse
                {
                    ResponseString = DekoSharp.DeleteCompanyBioData(inputRequest, out var statusCode),
                    ResponseCode = (statusCode == HttpStatusCode.OK) ? "00" : "06"
                };
                return new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(response)),
                    StatusCode = statusCode
                };
            }
            catch (Exception ex)
            {
                var response = new Models.DekobaseResponse
                {
                    ResponseString = ex.ToString(),
                    ResponseCode = "06",
                    ErrorMessage = ex.ToString()
                };
                return new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(response)),
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }

        [HttpPost("[action]")]
        public HttpResponseMessage GetCompanyBioData(Models.CompanyBioData inputRequest)
        {
            try
            {
                var response = new Models.DekobaseResponse
                {
                    ResponseString = DekoSharp.RetrieveCompanyBioData(inputRequest.Name, out var statusCode),
                    ResponseCode = "00"
                };
                return new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(response)),
                    StatusCode = statusCode
                };
            }
            catch (Exception ex)
            {
                var response = new Models.DekobaseResponse
                {
                    ResponseString = ex.ToString(),
                    ResponseCode = "06",
                    ErrorMessage = ex.ToString()
                };
                return new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(response)),
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }

        [HttpPost("[action]")]
        public IActionResult CreateKycRequest(CreateKycRequest inputRequest)
        {
                var response = new Models.DekobaseResponse
                {
                    ResponseString = _dekoSharp.CreateKycRequest(inputRequest.RequesterName, inputRequest.RCNumber, inputRequest.AdditionalInformation, out var statusCode),
                    ResponseCode = "00"
                };
                return Ok(JsonConvert.SerializeObject(response));
            
            //catch (Exception ex)
            //{
            //    var response = new Models.DekobaseResponse
            //    {
            //        ResponseString = ex.ToString(),
            //        ResponseCode = "06",
            //        ErrorMessage = ex.ToString()
            //    };
            //    return new HttpResponseMessage
            //    {
            //        Content = new StringContent(JsonConvert.SerializeObject(response)),
            //        StatusCode = HttpStatusCode.InternalServerError,
            //    };
            //}
        }

        [HttpPost("[action]")]
        public IActionResult RetrieveAllKycRequests()
        {
            try
            {
                var response = new Models.DekobaseResponse
                {
                    ResponseString = DekoSharp.RetrieveAllKycRequest(out var statusCode),
                    ResponseCode = "00"
                };
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                var response = new Models.DekobaseResponse
                {
                    ResponseString = ex.ToString(),
                    ResponseCode = "06",
                    ErrorMessage = ex.ToString()
                };
                return StatusCode(StatusCodes.Status500InternalServerError, JsonConvert.SerializeObject(response));
            }
        }
        [HttpPost("[action]")]
        public IActionResult RetrievePendingKycRequests()
        {
            try
            {
                var response = new Models.DekobaseResponse
                {
                    ResponseString = DekoSharp.RetrievePendingKycRequests(out var statusCode),
                    ResponseCode = "00"
                };
                return Ok(JsonConvert.SerializeObject(response));
            }
            catch (Exception ex)
            {
                var response = new Models.DekobaseResponse
                {
                    ResponseString = ex.ToString(),
                    ResponseCode = "06",
                    ErrorMessage = ex.ToString()
                };
                return StatusCode(StatusCodes.Status500InternalServerError, JsonConvert.SerializeObject(response));
            }
        }

        [HttpPost("[action]")]
        public HttpResponseMessage UpdateKycInfo(UploadKycRequest inputRequest)
        {
            try
            {
                var response = new Models.DekobaseResponse
                {
                    ResponseString = DekoSharp.UploadKycInfo(inputRequest, out var statusCode),
                    ResponseCode = "00"
                };
                return new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(response)),
                    StatusCode = statusCode
                };
            }
            catch (Exception ex)
            {
                var response = new Models.DekobaseResponse
                {
                    ResponseString = ex.ToString(),
                    ResponseCode = "06",
                    ErrorMessage = ex.ToString()
                };
                return new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(response)),
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }

        [HttpPost("[action]")]
        public HttpResponseMessage TryDrive(KycInfo inputRequest)
        {
            try
            {
                var response = new Models.DekobaseResponse
                {
                    //ResponseString = new GoogleDrive().IndexAsync(CancellationToken.None, this.ControllerContext.Controller as Controller),
                    ResponseCode = "00"
                };
                return new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(response)),
                };
            }
            catch (Exception ex)
            {
                var response = new Models.DekobaseResponse
                {
                    ResponseString = ex.ToString(),
                    ResponseCode = "06",
                    ErrorMessage = ex.ToString()
                };
                return new HttpResponseMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(response)),
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }
    }
}
