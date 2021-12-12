using CooperateAffairs.Logic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CooperateAffairs.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UtilityController : ControllerBase
	{
		IMailService _mailService;
		IPdf _pdf;
		public UtilityController(IMailService mailService, IPdf pdf)
		{
			_mailService = mailService;
			_pdf = pdf;
		}
		[HttpPost("[action]")]
		public IActionResult SendMail()
		{
			_mailService.SendEmailAsync();
			return Ok("Mail Sent");
		}

		[HttpPost("[action]")]
		public IActionResult SavePdf()
		{
			_pdf.GetPdf();
			return Ok("Mail Sent");
		}
	}
}
