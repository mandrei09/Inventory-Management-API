using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Optima.Fais.Data;
using Optima.Fais.Model;
using Optima.Fais.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Controllers
{
    [Authorize]
    [Route("api/documents")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public partial class DocumentsController : GenericApiController<Model.Document, Dto.Document>
    {
        private readonly IEmailSender _emailSender;
		private readonly UserManager<ApplicationUser> _userManager;

		public DocumentsController(ApplicationDbContext context, IDocumentsRepository itemsRepository, IMapper mapper, IEmailSender emailSender, UserManager<Model.ApplicationUser> userManager)
            : base(context, itemsRepository, mapper)
        {
            this._emailSender = emailSender;
			this._userManager = userManager;
		}

   
        [HttpPost("personelValidate")]
        public async Task<TransferResult> PersonelValidate([FromBody] Dto.PersonelValidate data)
        {

            if (HttpContext.User.Identity.Name != null)
            {
                
                var userName = HttpContext.User.Identity.Name;
                //userName = "ioana.cristea";
                var user = await _userManager.FindByEmailAsync(userName);
                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(userName);
                }
                _context.UserId = user.Id.ToString();

                var createAssetSAPResult = await (_itemsRepository as IDocumentsRepository).PersonelValidate(data);

                // await this._notifyService.NotifyDataCreateAssetAsync(createAssetSAPResult);

                return createAssetSAPResult;
            }
            else
            {
                return new Model.TransferResult { Success = false, Message = $"Va rugam sa va autentificati!" };
            }
        }

        [HttpPost("managerValidate")]
        public async Task<TransferResult> ManagerValidate([FromBody] Dto.PersonelValidate data)
        {

            if (HttpContext.User.Identity.Name != null)
            {

                var userName = HttpContext.User.Identity.Name;
                //userName = "ioana.cristea";
                var user = await _userManager.FindByEmailAsync(userName);
                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(userName);
                }
                _context.UserId = user.Id.ToString();

                var createAssetSAPResult = await (_itemsRepository as IDocumentsRepository).ManagerValidate(data);

                // await this._notifyService.NotifyDataCreateAssetAsync(createAssetSAPResult);

                return createAssetSAPResult;
            }
            else
            {
                return new Model.TransferResult { Success = false, Message = $"Va rugam sa va autentificati!" };
            }
        }

        [HttpPost("validate")]
        public async Task<TransferResult> Validate([FromBody] Dto.DocumentUpload documentUpload)
        {

            if (HttpContext.User.Identity.Name != null)
            {

                var userName = HttpContext.User.Identity.Name;
                //userName = "ioana.cristea";
                var user = await _userManager.FindByEmailAsync(userName);
                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(userName);
                }
                _context.UserId = user.Id.ToString();

                var createAssetSAPResult = await (_itemsRepository as IDocumentsRepository).Validate(documentUpload);

                // await this._notifyService.NotifyDataCreateAssetAsync(createAssetSAPResult);

                return createAssetSAPResult;
            }
            else
            {
                return new Model.TransferResult { Success = false, Message = $"Va rugam sa va autentificati!" };
            }
        }

		[HttpPost("reject")]
		public async Task<TransferResult> Reject([FromBody] Dto.DocumentUpload documentUpload)
		{

			if (HttpContext.User.Identity.Name != null)
			{

				var userName = HttpContext.User.Identity.Name;
				var user = await _userManager.FindByEmailAsync(userName);
				if (user == null)
				{
					user = await _userManager.FindByNameAsync(userName);
				}
				_context.UserId = user.Id.ToString();

				var createAssetSAPResult = await (_itemsRepository as IDocumentsRepository).Reject(documentUpload);

				// await this._notifyService.NotifyDataCreateAssetAsync(createAssetSAPResult);

				return createAssetSAPResult;
			}
			else
			{
				return new Model.TransferResult { Success = false, Message = $"Va rugam sa va autentificati!" };
			}
		}

		[HttpPost("rejectFromStock")]
		public async Task<TransferResult> RejectFromStock([FromBody] Dto.DocumentUpload documentUpload)
		{

			if (HttpContext.User.Identity.Name != null)
			{

				var userName = HttpContext.User.Identity.Name;
				var user = await _userManager.FindByEmailAsync(userName);
				if (user == null)
				{
					user = await _userManager.FindByNameAsync(userName);
				}
				_context.UserId = user.Id.ToString();
                documentUpload.OperationEmpId = user.EmployeeId;

				var createAssetSAPResult = await (_itemsRepository as IDocumentsRepository).RejectFromStock(documentUpload);

				// await this._notifyService.NotifyDataCreateAssetAsync(createAssetSAPResult);

				return createAssetSAPResult;
			}
			else
			{
				return new Model.TransferResult { Success = false, Message = $"Va rugam sa va autentificati!" };
			}
		}

		[HttpPost("full")]
        public async Task<TransferResult> PostTransfer([FromBody] Dto.DocumentUpload documentUpload)
        {

            if (HttpContext.User.Identity.Name != null)
            {
                var userName = HttpContext.User.Identity.Name;
                var user = await _userManager.FindByEmailAsync(userName);
                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(userName);
                }
                _context.UserId = user.Id.ToString();
                documentUpload.OperationEmpId = user.EmployeeId;

                var createAssetSAPResult = await (_itemsRepository as IDocumentsRepository).Save(documentUpload);

                //await this._notifyService.NotifyDataCreateAssetAsync(createAssetSAPResult);

                return createAssetSAPResult;
            }
            else
            {
                return new Model.TransferResult { Success = false,Message = $"Va rugam sa va autentificati!" };
            }

        }

        [HttpPost("wfhfull")]
        public async Task<TransferResult> PostWFHTransfer([FromBody] Dto.DocumentUpload documentUpload)
        {

            if (HttpContext.User.Identity.Name != null)
            {
                var userName = HttpContext.User.Identity.Name;
                var user = await _userManager.FindByEmailAsync(userName);
                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(userName);
                }
                _context.UserId = user.Id.ToString();
                documentUpload.OperationEmpId = user.EmployeeId;

                var createAssetSAPResult = await (_itemsRepository as IDocumentsRepository).WFHSave(documentUpload);

                //await this._notifyService.NotifyDataCreateAssetAsync(createAssetSAPResult);

                return createAssetSAPResult;
            }
            else
            {
                return new Model.TransferResult { Success = false, Message = $"Va rugam sa va autentificati!" };
            }

        }

        [HttpPost("stateChange")]
        public async Task<TransferResult> PostStateChange([FromBody] Dto.DocumentUpload documentUpload)
        {

            if (HttpContext.User.Identity.Name != null)
            {
                var userName = HttpContext.User.Identity.Name;
                var user = await _userManager.FindByEmailAsync(userName);
                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(userName);
                }
                _context.UserId = user.Id.ToString();

                var createAssetSAPResult = (_itemsRepository as IDocumentsRepository).SaveStateChange(documentUpload);

                // await this._notifyService.NotifyDataCreateAssetAsync(createAssetSAPResult);

                return createAssetSAPResult;
            }
            else
            {
                return new Model.TransferResult { Success = false, Message = $"Va rugam sa va autentificati!" };
            }

        }

        [AllowAnonymous]
        [Route("manual/serie")]
        public ActionResult DownloadSerie()
        {
            string filePath = @"manuals\SERIE.pdf";
            string fileName = "Instructiuni identificare serie.pdf";

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/force-download", fileName);

        }

        [AllowAnonymous]
        [Route("manual/imei")]
        public ActionResult DownloadImei()
        {
            string filePath = @"manuals\IMEI.pdf";
            string fileName = "Instructiuni identificare IMEI.pdf";

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/force-download", fileName);

        }

        [AllowAnonymous]
        [Route("manual/book")]
        public ActionResult DownloadBook()
        {
            string filePath = @"manuals\MANUAL.pdf";
            string fileName = "Manual OPTIMA.pdf";

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/force-download", fileName);

        }

		[AllowAnonymous]
		[Route("manual/wfhbook")]
		public ActionResult DownloadWFHBook()
		{
			string filePath = @"manuals\MANUAL_WFH.pdf";
			string fileName = "Manual WFH.pdf";

			byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

			return File(fileBytes, "application/force-download", fileName);

		}
	}
}
