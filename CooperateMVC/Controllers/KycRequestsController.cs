using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CooperateMVC.Data;
using CooperateMVC.Models;
using CooperateMVC.Logic;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CooperateMVC.Controllers
{
	[ClaimRequirement("", "")]
	public class KycRequestsController : Controller
	{
		private readonly CooperateMVCContext _context;

		public KycRequestsController(CooperateMVCContext context)
		{
			_context = context;
		}

		// GET: KycRequests
		public async Task<IActionResult> Index()
		{
			var dekoSharp = new DekoSharp();
			var bankList = dekoSharp.RetrieveAllKycRequest(out var statusCode);
			return View(bankList);
		}

		// GET: KycRequests/Details/5
		public async Task<IActionResult> Details(string id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var kycRequest = new DekoSharp().RetrieveKYCRequestWithRCNumber(id, out var statusCode);
			if (kycRequest == null)
			{
				return NotFound();
			}

			return View(kycRequest);
		}

		// GET: KycRequests/Details/5
		public async Task<IActionResult> ViewKycInfo(string id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var kycRequest = new DekoSharp().RetrieveKYCRequestWithRCNumber(id, out var statusCode);
			if (kycRequest == null)
			{
				return NotFound();
			}

			return View(kycRequest.KycInfo);
		}

		// GET: KycRequests/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: KycRequests/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,RequestId,RCNumber,RequesterName,AssignedLawyerId,TimeRequested,LastTimeUpdated,AdditionalInformation,Status")] KycRequest kycRequest)
		{
			if (ModelState.IsValid)
			{
				var bankUser = new DekoSharp().RetrieveBankUserWithCookie(Request);
				if (bankUser == null)
				{
					return NotFound();
				}
				new DekoSharp().CreateKycRequest(bankUser.BankName, kycRequest.RCNumber, kycRequest.AdditionalInformation, out var statusCode);
				return RedirectToAction(nameof(Index));
			}
			return View(kycRequest);
		}

		// GET: KycRequests/Edit/5
		public async Task<IActionResult> Edit(string? id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return NotFound();
			}

			var kycRequest = new DekoSharp().RetrieveKycRequestByRcNumber(id, out var statusCode);
			if (kycRequest == null)
			{
				return NotFound();
			}
			var uid = new CookieManager().GetCookie("_firebaseUser", Request);
			if (Convert.ToString(new Dekobase().GetUserClaimsWithKey(uid, "role", out var isAuthorized)) == "bank")
			{
				return RedirectToAction(nameof(BankEditKycRequest), new { id = id });
			}
			return View(kycRequest);
		}

		// POST: KycRequests/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(string id, [Bind("Id,RequestId,RCNumber,RequesterName,AssignedLawyerId,TimeRequested,LastTimeUpdated,AdditionalInformation,Status")] KycRequest kycRequest)
		{

			if (ModelState.IsValid)
			{
				try
				{
					var context = HttpContext;
					new DekoSharp().UpdateDataToPath($"request/{DekoUtility.CheckNullOrEmpty(DekoUtility.ReplaceSpace(kycRequest.RCNumber))}", kycRequest, out var statusCode);
					if (statusCode != System.Net.HttpStatusCode.OK)
					{
						ModelState.AddModelError("statusCode", "" + statusCode);
						return View(ModelState);
					}
					return RedirectToAction(nameof(Index));
				}
				catch (Exception ex)
				{
					if (!KycRequestExists(kycRequest.RCNumber))
					{
						return NotFound();
					}
					else
					{
						ExceptionHandler.HandleException(ex);
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
			return View(kycRequest);
		}


		// GET: KycRequests/Edit/5
		public async Task<IActionResult> BankEditKycRequest(string? id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return NotFound();
			}

			var kycRequest = new DekoSharp().RetrieveKycRequestByRcNumber(id, out var statusCode);
			if (kycRequest == null)
			{
				return NotFound();
			}
			if (kycRequest.Status != KycRequest.KycRequestStatus.Pending)
			{
				// Bank cannot edit KycRequest that has already begun processing
			}
			var editKycRequest = new EditKycRequest
			{
				RCNumber = kycRequest.RCNumber,
				AdditionalInformation = kycRequest.AdditionalInformation,
				Status = kycRequest.Status,
				Id = kycRequest.Id,
				RequesterName = kycRequest.RequesterName
			};
			return View(editKycRequest);
		}

		// POST: KycRequests/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> BankEditKycRequest(string id, EditKycRequest kycRequest)
		{

			if (ModelState.IsValid)
			{
				try
				{
					if (kycRequest.Status != KycRequest.KycRequestStatus.Pending)
					{
						// Bank cannot edit KycRequest that has already begun processing
						ModelState.AddModelError("", "Bank cannot edit KycRequest once it has been accepted by the lawyer and processing has begun");
						return View();
					}
					var context = HttpContext;
					new DekoSharp().UpdateDataToPath($"request/{DekoUtility.CheckNullOrEmpty(DekoUtility.ReplaceSpace(kycRequest.RCNumber))}", kycRequest, out var statusCode);
					if (statusCode != System.Net.HttpStatusCode.OK)
					{
						ModelState.AddModelError("statusCode", "" + statusCode);
						return View();
					}
					return RedirectToAction(nameof(Index));
				}
				catch (Exception ex)
				{
					ExceptionHandler.HandleException(ex);
					ModelState.AddModelError("", ex.Message);
					return View();
				}
				return RedirectToAction(nameof(Index));
			}
			return View(kycRequest);
		}

		// GET: KycRequests/Delete/5
		public async Task<IActionResult> Delete(string id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var kycRequest = new DekoSharp().RetrieveKycRequestByRcNumber(id, out var statusCode);
			if (kycRequest == null)
			{
				return NotFound();
			}

			return View(kycRequest);
		}

		// POST: KycRequests/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(string id)
		{
			try
			{
				var kycRequest = new DekoSharp().DeleteKycRequest(id, out var statusCode);
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ExceptionHandler.HandleException(ex);
				return View();
			}
		}

		private bool KycRequestExists(string id)
		{
			return new DekoSharp().RetrieveKycRequestByRcNumber(id, out var statusCode) == null;
		}

		public override void OnActionExecuted(ActionExecutedContext context)
		{
			if (TempData["ModelState"] != null && !ModelState.Equals(TempData["ModelState"]))
				ModelState.Merge(FetchModelStateFromTempData());

			base.OnActionExecuted(context);
		}

		[HttpGet]
		//[ValidateAntiForgeryToken]
		//[AllowAnonymous]
		public IActionResult CreateKycInfo(string id)
		{
			KycInfoViewModel kycInfoViewModel = null;
			
			if (string.IsNullOrEmpty(id))
			{
				var kycInfoTempData = (string)TempData["KycInfoViewModelString"];
				Console.WriteLine("ID is null, summoning KycTempData");
				var kycInfoTempObject = JsonConvert.DeserializeObject<KycInfoViewModel>(kycInfoTempData);
				if(kycInfoTempData != null)
                {
					kycInfoViewModel = kycInfoTempObject;
                }
                else
                {
					return NotFound();
				}
			}
			else { 
				Console.WriteLine("# CreateKycInfo:  ID is not null, about to fetch KYC from DB");
				var kycRequest = new DekoSharp().RetrieveKycRequestByRcNumber(id, out _);
				kycInfoViewModel = ConvertKycRequestToViewModel(kycRequest);
			}
			if (kycInfoViewModel != null)
			{
				//kycInfoViewModel.RCNumber = "4444411111";
				Console.WriteLine("TempData: " + JsonConvert.SerializeObject(kycInfoViewModel));

				TempData["KycInfoViewModelString"] = JsonConvert.SerializeObject(kycInfoViewModel);
				return View(kycInfoViewModel);
			}
			return View(new KycInfoViewModel { RCNumber = id});
		}

        KycInfoViewModel ConvertKycRequestToViewModel(KycRequest kycRequest) => new()
        {
			CertificateUrl = kycRequest?.KycInfo?.CertificateUrl,
            CompanyName = kycRequest?.KycInfo?.CompanyName,
            CompanyObjective = kycRequest?.KycInfo?.CompanyObjective,
			CompanyOfficersList = kycRequest?.KycInfo?.CompanyOfficersList,
			RCNumber = kycRequest?.KycInfo?.RCNumber,
			ShareHolders = kycRequest?.KycInfo?.ShareHolders,
		};
        

		[HttpPost]
		public IActionResult CreateKycInfo(KycInfoViewModel kycInfoViewModel)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var kycTempDataString = (string) TempData["KycInfoViewModelString"];
					TempData["KycInfoViewModelString"] = kycTempDataString;
					var kycTempData = JsonConvert.DeserializeObject<KycInfoViewModel>(kycTempDataString);
					kycInfoViewModel.CompanyOfficersList = kycTempData.CompanyOfficersList;
					kycInfoViewModel.ShareHolders = kycTempData.ShareHolders;
					kycTempData.CompanyName = kycInfoViewModel.CompanyName;
					kycTempData.CompanyObjective = kycInfoViewModel.CompanyObjective;
					TempData["KycInfoViewModelString"] = JsonConvert.SerializeObject(kycTempData);

					var kycInfo = MapKycInfoToViewModel(kycTempData);
					var uploadKyc = new UploadKycRequest()
					{
						KycInfo = kycInfo,
						LawyerID = "Aluko_&_Oyebode",
						RCNumber = "1210"
					};

					var response = new DekoSharp().UploadKycInfo(uploadKyc, out var statusCode);
					if (response.StartsWith("Exception"))
					{
						Console.WriteLine(response);
						ModelState.AddModelError("", response);
						return View();
					}
					Console.WriteLine("About to redirect..");
					//return RedirectToAction(nameof(Index));
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
					ModelState.AddModelError("", ex.Message);
					SaveModelStateInTempData(ModelState);
					return RedirectToAction(nameof(CreateKycInfo));
				}
			}
			return RedirectToAction(nameof(CreateKycInfo));
		}



		[HttpPost]
		public IActionResult NavigateToCreateCompanyOfficer(KycInfoViewModel kycInfoViewModel)
		{
			var kycInfoTempObject = new KycInfoViewModel();
			var kycInfoTempDataString = (string)TempData["KycInfoViewModelString"];
			if (kycInfoTempDataString != null)
			{
				kycInfoTempObject = JsonConvert.DeserializeObject<KycInfoViewModel>(kycInfoTempDataString);
				kycInfoTempObject.CompanyName = kycInfoViewModel.CompanyName;
				kycInfoTempObject.CompanyObjective = kycInfoViewModel.CompanyObjective;
				TempData["KycInfoViewModelString"] = JsonConvert.SerializeObject(kycInfoTempObject);
			}
			return RedirectToAction(nameof(CreateCompanyOfficer));
		}
		[HttpPost]
		public IActionResult NavigateToDeleteCompanyOfficer(string id)
		{
			var kycInfoTempDataString = (string)TempData["KycInfoViewModelString"];
			if (kycInfoTempDataString == null)
			{
				return RedirectToAction(nameof(CreateKycInfo));
			}
			var kycInfoTempObject = JsonConvert.DeserializeObject<KycInfoViewModel>(kycInfoTempDataString);
			var companyOfficers = kycInfoTempObject.CompanyOfficersList;
			if (!string.IsNullOrEmpty(id))
			{
				companyOfficers.RemoveAt(Convert.ToInt32(id));
			}
			TempData["KycInfoViewModelString"] = JsonConvert.SerializeObject(kycInfoTempObject);
			return RedirectToAction(nameof(CreateKycInfo));
		}

		[HttpPost]
		public IActionResult NavigateToDeleteShareHolder(string id)
		{
			var kycInfoTempDataString = (string)TempData["KycInfoViewModelString"];
			if (kycInfoTempDataString == null)
			{
				return RedirectToAction(nameof(CreateKycInfo));
			}
			var kycInfoTempObject = JsonConvert.DeserializeObject<KycInfoViewModel>(kycInfoTempDataString);
			var shareHolders = kycInfoTempObject.ShareHolders;
			if (!string.IsNullOrEmpty(id))
			{
				shareHolders.RemoveAt(Convert.ToInt32(id));
			}
			TempData["KycInfoViewModelString"] = JsonConvert.SerializeObject(kycInfoTempObject);
			return RedirectToAction(nameof(CreateKycInfo));
		}
		[HttpPost]
		public IActionResult NavigateToEditCompanyOfficer(string id)
		{
			var updatedModel = new KycInfoViewModel();
			var isModelUpdated = TryUpdateModelAsync(updatedModel).Result;
			if (isModelUpdated)
			{
				Console.WriteLine("New company name: "+updatedModel.CompanyName);
			}
			var kycInfoViewModel = updatedModel;
			var kycInfoTempObject = new KycInfoViewModel();
			var kycInfoTempDataString = (string)TempData["KycInfoViewModelString"];
			if (kycInfoTempDataString != null)
			{
				kycInfoTempObject = JsonConvert.DeserializeObject<KycInfoViewModel>(kycInfoTempDataString);
				kycInfoTempObject.CompanyName = (isModelUpdated) ? kycInfoViewModel.CompanyName : kycInfoTempObject.CompanyName;
				kycInfoTempObject.CompanyObjective = (isModelUpdated) ? kycInfoViewModel.CompanyObjective : kycInfoTempObject.CompanyObjective;
				TempData["KycInfoViewModelString"] = JsonConvert.SerializeObject(kycInfoTempObject);
			}
			Console.WriteLine("ID........................." + id);
			if (string.IsNullOrEmpty(id)) { RedirectToAction(nameof(CreateKycInfo)); }
			var kycTempDataString = (string)TempData["KycInfoViewModelString"];
			TempData["KycInfoViewModelString"] = kycTempDataString;
			var kycTempData = JsonConvert.DeserializeObject<KycInfoViewModel>(kycTempDataString);
			var kycList = kycTempData.CompanyOfficersList;
			var company = kycList[Convert.ToInt32(id)];


			TempData["CompanyOfficerName"] = company.Name;
			TempData["CompanyOfficerPosition"] = company.Position;
			TempData["CompanyOfficerId"] = company.Id;
			TempData["CompanyOfficerSerialNumber"] = company.SerialNumber;

			return RedirectToAction(nameof(CreateCompanyOfficer));
		}

		[HttpPost]
		public IActionResult NavigateToEditShareHolder(string id)
		{
			Console.WriteLine("ID: " + id);
			var updatedModel = new KycInfoViewModel();
			var isModelUpdated = TryUpdateModelAsync(updatedModel).Result;
			if (isModelUpdated)
			{
				Console.WriteLine("New company name: "+updatedModel.CompanyName);
			}
			var kycInfoViewModel = updatedModel;
			var kycInfoTempDataString = (string)TempData["KycInfoViewModelString"];
			if (kycInfoTempDataString != null)
			{
				var kycInfoTempObject = JsonConvert.DeserializeObject<KycInfoViewModel>(kycInfoTempDataString);
				kycInfoTempObject.CompanyName = (isModelUpdated) ? kycInfoViewModel.CompanyName : kycInfoTempObject.CompanyName;
				kycInfoTempObject.CompanyObjective = (isModelUpdated) ? kycInfoViewModel.CompanyObjective : kycInfoTempObject.CompanyObjective;
				TempData["KycInfoViewModelString"] = JsonConvert.SerializeObject(kycInfoTempObject);
			}
			Console.WriteLine("ID........................." + id);
			if (string.IsNullOrEmpty(id)) { RedirectToAction(nameof(CreateKycInfo)); }
			var kycTempDataString = (string)TempData["KycInfoViewModelString"];
			TempData["KycInfoViewModelString"] = kycTempDataString;
			var kycTempData = JsonConvert.DeserializeObject<KycInfoViewModel>(kycTempDataString);
			var kycList = kycTempData.ShareHolders;
			var shareHolder = kycList[Convert.ToInt32(id)];
			Console.WriteLine(JsonConvert.SerializeObject(shareHolder));

			TempData["ShareHolderName"] = shareHolder.Name;
			TempData["ShareHolderPercentage"] = shareHolder.Percentage.ToString();
			TempData["ShareHolderId"] = shareHolder.Id;
			return RedirectToAction(nameof(CreateShareHolder));
		}

		[HttpPost]
		public IActionResult NavigateToCreateShareHolder(KycInfoViewModel kycInfoViewModel)
		{
			var kycInfoTempObject = new KycInfoViewModel();
			var kycInfoTempDataString = (string)TempData["KycInfoViewModelString"];
			if (kycInfoTempDataString != null)
			{
				kycInfoTempObject = JsonConvert.DeserializeObject<KycInfoViewModel>(kycInfoTempDataString);
				kycInfoTempObject.CompanyName = kycInfoViewModel.CompanyName;
				kycInfoTempObject.CompanyObjective = kycInfoViewModel.CompanyObjective;
				TempData["KycInfoViewModelString"] = JsonConvert.SerializeObject(kycInfoTempObject);
			}
			return RedirectToAction(nameof(CreateShareHolder));
		}

		[HttpGet]
		//[ValidateAntiForgeryToken]
		//[AllowAnonymous]
		public IActionResult CreateCompanyOfficer(string id)
		{
			Console.WriteLine("Inside Get Create Company Officer: ");
			var kycInfoTempData = (string)TempData["KycInfoViewModelString"];
			if (kycInfoTempData == null)
			{
				Console.WriteLine("Kyc TempData is null ");
			}
			var kycInfoViewModel = JsonConvert.DeserializeObject<KycInfoViewModel>(kycInfoTempData);
			TempData["KycInfoViewModelString"] = kycInfoTempData;
			return View(kycInfoViewModel);
		}

		[HttpPost]
		public IActionResult ClearTempData()
		{
			TempData.Remove("KycInfoViewModelString");
			return RedirectToAction(nameof(CreateKycInfo));
		}

		[HttpPost]
		public IActionResult CreateCompanyOfficer(KycInfoViewModel kycInfo)
		{
			Console.WriteLine("Inside Post Create Company Officer: ");

			try
			{
				if (ModelState.IsValid)
				{
					var kycInfoTempData = (string)TempData["KycInfoViewModelString"];
					if (kycInfoTempData == null)
					{
						Console.WriteLine("Kyc TempData is null ");
						ModelState.AddModelError("", "Model error: Kyc TempData is null");
						return View();
					}
					TempData["KycInfoViewModelString"] = kycInfoTempData;
					var kycInfoViewModel = JsonConvert.DeserializeObject<KycInfoViewModel>(kycInfoTempData);
					if (string.IsNullOrEmpty(kycInfo?.RCNumber))
					{
						ModelState.AddModelError("", "Model error: RCNumber is empty or null");
						return View();
					}
					if (kycInfoViewModel?.CompanyOfficersList == null || kycInfoViewModel.CompanyOfficersList.Count < 1)
					{
						Console.WriteLine("Kyc TempData OfficerList is empty ");
						kycInfoViewModel.CompanyOfficersList = new List<KycInfo.CompanyOfficer>();
					}
					kycInfoViewModel.CompanyOfficersList.Add(new KycInfo.CompanyOfficer
					{
						Name = kycInfo.CompanyOfficerName,
						Position = kycInfo.CompanyOfficerPosition,
						SerialNumber = kycInfo.CompanyOfficerSerialNumber
					});

					var kycModelString = JsonConvert.SerializeObject(kycInfoViewModel);
					var vr = TempData["KycInfoViewModelString"];
					TempData["KycInfoViewModelString"] = kycModelString;
					return RedirectToAction(nameof(CreateKycInfo));
				}
				Console.WriteLine("ModelState Invalid");
				return View();
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", ex.Message);
				throw;
			}
		}

		[HttpPost]
		public IActionResult EditShareHolder(KycInfoViewModel kycInfo)
		{
			Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@Inside Post Edit Share Holder: ");

			try
			{
				if (ModelState.IsValid)
				{
					var kycInfoTempData = (string)TempData["KycInfoViewModelString"];
					if (kycInfoTempData == null)
					{
						Console.WriteLine("Kyc TempData is null ");
						ModelState.AddModelError("", "Model error: Kyc TempData is null");
						return View();
					}
					TempData["KycInfoViewModelString"] = kycInfoTempData;
					var kycInfoViewModel = JsonConvert.DeserializeObject<KycInfoViewModel>(kycInfoTempData);

					Console.WriteLine($"********************* TempData String: {kycInfoTempData}");
					Console.WriteLine($"********************* Deserialized TempData: {JsonConvert.SerializeObject(kycInfoViewModel)}");
					if (string.IsNullOrEmpty(kycInfo?.RCNumber))
					{
						ModelState.AddModelError("", "Model error: RCNumber is empty or null");
						return View();
					}
					if (kycInfoViewModel?.ShareHolders == null || kycInfoViewModel.ShareHolders.Count < 1)
					{
						Console.WriteLine("Kyc TempData ShareHolders List is empty ");
						RedirectToAction(nameof(CreateShareHolder));
					}
					var shareHolder = kycInfoViewModel.ShareHolders.FirstOrDefault(x => x.Id == kycInfo.ShareHolderId);
					if(shareHolder == null)
					{
						Console.WriteLine($"No shareholder in list matches the shareholderID: {kycInfo.ShareHolderId}");
						return RedirectToAction(nameof(CreateKycInfo));
					}
					shareHolder.Name = kycInfo.ShareHolderName;
					shareHolder.Percentage = kycInfo.ShareHolderPercentage;

					Console.WriteLine($"Old: {shareHolder.Name}, New: {kycInfo.ShareHolderName}");
					Console.WriteLine($"Old: {shareHolder.Percentage}, New: {kycInfo.ShareHolderPercentage}");

					var kycModelString = JsonConvert.SerializeObject(kycInfoViewModel) ;
					var vr = TempData["KycInfoViewModelString"];
					TempData["KycInfoViewModelString"] = kycModelString;
					return RedirectToAction(nameof(CreateKycInfo));
				}
				Console.WriteLine("ModelState Invalid");
				return RedirectToAction(nameof(CreateShareHolder));
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", ex.Message);
				throw;
			}
		}
		[HttpPost]
		public IActionResult EditCompanyOfficer(KycInfoViewModel kycInfo)
		{
			Console.WriteLine("Inside Post Edit Company Officer: ");

			try
			{
				if (ModelState.IsValid)
				{
					var kycInfoTempData = (string)TempData["KycInfoViewModelString"];
					if (kycInfoTempData == null)
					{
						Console.WriteLine("Kyc TempData is null ");
						ModelState.AddModelError("", "Model error: Kyc TempData is null");
						return View();
					}
					TempData["KycInfoViewModelString"] = kycInfoTempData;
					var kycInfoViewModel = JsonConvert.DeserializeObject<KycInfoViewModel>(kycInfoTempData);
					if (string.IsNullOrEmpty(kycInfo?.RCNumber))
					{
						ModelState.AddModelError("", "Model error: RCNumber is empty or null");
						return View();
					}
					if (kycInfoViewModel?.CompanyOfficersList == null || kycInfoViewModel.CompanyOfficersList.Count < 1)
					{
						Console.WriteLine("Kyc TempData OfficerList is empty ");
						kycInfoViewModel.CompanyOfficersList = new List<KycInfo.CompanyOfficer>();
					}
					var companyOfficer = kycInfoViewModel.CompanyOfficersList.FirstOrDefault(x => x.Id == kycInfo.CompanyOfficerId);
					if(companyOfficer == null)
					{
						return RedirectToAction(nameof(CreateKycInfo));
					}
					companyOfficer.Name = kycInfo.CompanyOfficerName;
					companyOfficer.Position = kycInfo.CompanyOfficerPosition;
					companyOfficer.SerialNumber = kycInfo.CompanyOfficerSerialNumber;
					

					var kycModelString = JsonConvert.SerializeObject(kycInfoViewModel) ;
					var vr = TempData["KycInfoViewModelString"];
					TempData["KycInfoViewModelString"] = kycModelString;
					return RedirectToAction(nameof(CreateKycInfo));
				}
				Console.WriteLine("ModelState Invalid");
				return View();
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", ex.Message);
				throw;
			}
		}

		[HttpGet]
		//[ValidateAntiForgeryToken]
		//[AllowAnonymous]
		public IActionResult CreateShareHolder(string id)
		{
			Console.WriteLine("Inside Get Create Company Officer: ");
			var kycInfoTempData = (string)TempData["KycInfoViewModelString"];
			if (kycInfoTempData == null)
			{
				Console.WriteLine("Kyc TempData is null ");
			}
			var kycInfoViewModel = JsonConvert.DeserializeObject<KycInfoViewModel>(kycInfoTempData);
			TempData["KycInfoViewModelString"] = kycInfoTempData;
			return View(kycInfoViewModel);
		}

		[HttpPost]
		//[ValidateAntiForgeryToken]
		//[AllowAnonymous]
		public IActionResult CreateShareHolder(KycInfoViewModel kycInfo)
		{
			Console.WriteLine("Inside Post Create Share Holder: ");
			if (ModelState.IsValid)
			{
				try
				{
					var kycInfoTempData = (string)TempData["KycInfoViewModelString"];
					if (kycInfoTempData == null)
					{
						Console.WriteLine("Kyc TempData is null ");
						ModelState.AddModelError("", "Model error: Kyc TempData is null");
						return View();
					}
					TempData["KycInfoViewModelString"] = kycInfoTempData;
					var kycInfoViewModel = JsonConvert.DeserializeObject<KycInfoViewModel>(kycInfoTempData);
					if (string.IsNullOrEmpty(kycInfo?.RCNumber))
					{
						ModelState.AddModelError("", "Model error: RCNumber is empty or null");
						return View();
					}
					if (kycInfoViewModel?.ShareHolders == null || kycInfoViewModel.ShareHolders.Count < 1)
					{
						Console.WriteLine("Kyc TempData OfficerList is empty ");
						kycInfoViewModel.ShareHolders = new List<KycInfo.ShareHolder>();
					}
					kycInfoViewModel.ShareHolders.Add(new KycInfo.ShareHolder
					{
						Name = kycInfo.ShareHolderName,
						Percentage = kycInfo.ShareHolderPercentage,
					});

					var kycModelString = JsonConvert.SerializeObject(kycInfoViewModel);
					var vr = TempData["KycInfoViewModelString"];
					TempData["KycInfoViewModelString"] = kycModelString;

				}
				catch (Exception ex)
				{
					throw;
				}
			}
			else
			{
				return View();
			}
			return RedirectToAction(nameof(CreateKycInfo));
		}

		public KycInfo MapKycInfoToViewModel(KycInfoViewModel kycInfoViewModel)
		{
			return new KycInfo
			{
				RCNumber = kycInfoViewModel.RCNumber,
				CompanyName = kycInfoViewModel.CompanyName,
				CompanyObjective = kycInfoViewModel.CompanyObjective,
				CompanyOfficersList = kycInfoViewModel.CompanyOfficersList,
				ShareHolders = kycInfoViewModel.ShareHolders,
				CertificateUrl = kycInfoViewModel.CertificateUrl,
			};
		}

		public void SaveModelStateInTempData(ModelStateDictionary modelState)
		{
			var listError = modelState.Where(x => x.Value.Errors.Any())
				.ToDictionary(m => m.Key, m => m.Value.Errors
				.Select(s => s.ErrorMessage)
				.FirstOrDefault(s => s != null));
			TempData["ModelState"] = JsonConvert.SerializeObject(listError);
		}

		public ModelStateDictionary FetchModelStateFromTempData()
		{
			var modelStateString = TempData["ModelState"].ToString();
			var listError = JsonConvert.DeserializeObject<Dictionary<string, string>>(modelStateString);
			var modelState = new ModelStateDictionary();
			foreach (var item in listError)
			{
				modelState.AddModelError(item.Key, item.Value ?? "");
			}
			return modelState;
		}

	}
}