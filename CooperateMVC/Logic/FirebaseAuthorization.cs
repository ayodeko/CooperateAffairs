using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Security.Claims;

namespace CooperateMVC.Logic
{
	public class FirebaseAuthorization
	{

	}
    public class ClaimRequirementAttribute : TypeFilterAttribute
    {
        public ClaimRequirementAttribute(string claimType, string claimValue) : base(typeof(ClaimRequirementFilter))
        {
            Arguments = new object[] { new Claim(claimType, claimValue) };
        }
    }

    public class ClaimRequirementFilter : IAuthorizationFilter
    {
        readonly Claim _claim;

        public ClaimRequirementFilter(Claim claim)
        {
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //var hasClaim = context.HttpContext.User.Claims.Any(c => c.Type == _claim.Type && c.Value == _claim.Value);
            bool hasClaim;
            if (context.HttpContext.Request.Cookies.Count < 1)
            {
                hasClaim = false;
            }
            else
            {
                hasClaim = AuthorizeFirebaseClaims(context.HttpContext);
            }
            if (!hasClaim)
            {
                context.Result = new RedirectToActionResult("BankSignIn", "Users", "");
                //context.Result = new ForbidResult();
            }
        }

        bool AuthorizeFirebaseClaims(Microsoft.AspNetCore.Http.HttpContext httpContext)
		{
            var firebaseCookie = httpContext.Request.Cookies["_firebaseUser"];
            var claimsList = new Dekobase().GetUserClaims(firebaseCookie, out var isAuthorized);
            if (!isAuthorized)
			{
                return false;
			}
            if(claimsList.ToList().Count < 1)
			{
                return false;
			}
			else
			{
                var result = claimsList.Any(obj => Convert.ToString(obj) == "admin" || Convert.ToString(obj) == "bank");
                return result;
			}
        }
    }


    [Route("api/resource")]
    public class MyController : Controller
    {
        
        [HttpGet]
        public IActionResult GetResource()
        {
            return Ok();
        }
    }
}
