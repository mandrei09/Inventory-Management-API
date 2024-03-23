using AutoMapper;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.EfRepository;
using Optima.Fais.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Controllers
{
    /// <summary>
    /// Identity Web API controller.
    /// </summary>
    [Route("api/identity")]
    [Authorize(Policy = "Manage Accounts")] // Authorization policy for this API.
    public class IdentityController : Controller
    {
        private readonly UserManager<Model.ApplicationUser> _userManager;
        private readonly SignInManager<Model.ApplicationUser> _signInManager;
		private readonly RoleManager<ApplicationRole> _roleManager;
		private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper mapper;

        public IdentityController(
            UserManager<Model.ApplicationUser> userManager,
            SignInManager<Model.ApplicationUser> signInManager,
			RoleManager<ApplicationRole> roleManager,
			ILoggerFactory loggerFactory,
            ApplicationDbContext context,
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
			_roleManager = roleManager;
			_logger = loggerFactory.CreateLogger<IdentityController>();
            _context = context;
            this.mapper = mapper;
        }

        /// <summary>
        /// Gets all the users (user role).
        /// </summary>
        /// <returns>Returns all the users</returns>
        // GET api/identity/users
        [HttpGet("users")]
        //[AllowAnonymous]
        public async Task<IActionResult> GetAllUsers(string sortColumn, string sortDirection, int? page, int? pageSize, string filter, string role)
        {

            var users = _userManager.Users;

            // var claim = new Claim("role", role);
            if (role == "all")
            {
                users = _userManager.Users
                    .Include(c => c.Claims)
                    .Include(e => e.Employee).ThenInclude(c => c.Company)
                    .Include(e => e.Substitute)
                    .Include("MobilePhone")
                    .Where(e => e.Employee.IsDeleted == false);

            }
            else
            {
                users = _userManager.Users
                    .Include(c => c.Claims)
                    .Include(e => e.Employee).ThenInclude(c => c.Company)
                    .Include(e => e.Substitute)
                    .Include("MobilePhone")
                    .Where(u => u.Claims.Any(c => c.ClaimValue == role))
                    .Where(e => e.Employee.IsDeleted == false);
            }

            var usersMap = mapper.Map<IEnumerable<Model.ApplicationUser>, IEnumerable<Dto.ApplicationUser>>(users);

            if (filter != null && filter.Length > 0)
            {
                usersMap = usersMap.Where(u =>
                u.UserName.Contains(filter, StringComparison.InvariantCultureIgnoreCase)
                || (u.GivenName?.Contains(filter, StringComparison.InvariantCultureIgnoreCase) ?? false)
                || (u.FamilyName?.Contains(filter, StringComparison.InvariantCultureIgnoreCase) ?? false)
                || (u.Role?.Contains(filter, StringComparison.InvariantCultureIgnoreCase) ?? false)
                || (u.Employee?.InternalCode?.Contains(filter, StringComparison.InvariantCultureIgnoreCase) ?? false)
                || (u.Employee?.FirstName?.Contains(filter, StringComparison.InvariantCultureIgnoreCase) ?? false)
                || (u.Employee?.LastName?.Contains(filter, StringComparison.InvariantCultureIgnoreCase) ?? false)
                || (u.Employee?.Company?.Code?.Contains(filter, StringComparison.InvariantCultureIgnoreCase) ?? false));
            }

            int totalItems = usersMap.Count();

            usersMap = sortDirection.ToLower() == "asc"
                ? usersMap.AsQueryable().OrderBy(ExpressionHelper.GenericEvaluateOrderBy<Dto.ApplicationUser>(sortColumn))
                : usersMap.AsQueryable().OrderByDescending(ExpressionHelper.GenericEvaluateOrderBy<Dto.ApplicationUser>(sortColumn));

            if (page.HasValue && pageSize.HasValue)
            {
                usersMap = usersMap.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }

            var pagedResult = new Dto.PagedResult<Dto.ApplicationUser>(usersMap, new Dto.PagingInfo()
            {
                TotalItems = totalItems,
                CurrentPage = page.Value,
                PageSize = pageSize.Value
            });
            return Ok(pagedResult);

            //return new JsonResult(usersMap);
        }


		/// <summary>
		/// Registers a new user.
		/// </summary>
		/// <returns>IdentityResult</returns>
		//POST: api/identity/create
		[HttpPost("create")]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody]User model)
        {
			IdentityResult roleResult;

			var user = new Model.ApplicationUser
            {
                UserName = model.userName,
                Email = model.userName,
                GivenName = model.givenName,
                FamilyName = model.familyName,
            };

			

			// Claims.
			user.Claims.Add(new IdentityUserClaim<string> { ClaimType = JwtClaimTypes.GivenName, ClaimValue = model.givenName });
            user.Claims.Add(new IdentityUserClaim<string> { ClaimType = JwtClaimTypes.FamilyName, ClaimValue = model.familyName });
            string[] roleNames = model.roles.Split(";".ToCharArray());


			foreach (string roleName in roleNames)
            {
                user.Claims.Add(new IdentityUserClaim<string> { ClaimType = JwtClaimTypes.Role, ClaimValue = roleName });

				// Roles //
				var role = await _roleManager.FindByNameAsync(roleName);
				

				if (role == null)
				{
					//create the roles and seed them to the database
					role = new ApplicationRole();
					role.Name = roleName;
					roleResult = await _roleManager.CreateAsync(role);
				}

				user.Roles.Add(new IdentityUserRole<string> { UserId = user.Id, RoleId = role.Id });

				////user.Roles.Add(new IdentityUserRole<string>({ }));
				//var userRole = new IdentityUserRole<string>();
				//userRole.RoleId = role.Id;
				//userRole.UserId = user.Id;
				//user.Roles.Add(userRole);
			}

			var result = await _userManager.CreateAsync(user, model.password);

            return new JsonResult(result);
        }


		//[HttpPost("create")]
		//[AllowAnonymous]
		//public async Task<IActionResult> Create([FromBody] List<User> model)
		//{
		//	foreach (var item in model)
		//	{
  //              IdentityResult roleResult;
  //              var user = new Model.ApplicationUser
		//		{
		//			UserName = item.userName,
		//			Email = item.userName,
		//			GivenName = item.givenName,
		//			FamilyName = item.familyName,
		//			PhoneNumber = item.phonenumber
		//		};

		//		// Claims.
		//		user.Claims.Add(new IdentityUserClaim<string> { ClaimType = JwtClaimTypes.GivenName, ClaimValue = item.givenName });
		//		user.Claims.Add(new IdentityUserClaim<string> { ClaimType = JwtClaimTypes.FamilyName, ClaimValue = item.familyName });
		//		string[] roles = item.roles.Split(";".ToCharArray());
				
  //              foreach (string roleName in roles)
		//		{
		//			user.Claims.Add(new IdentityUserClaim<string> { ClaimType = JwtClaimTypes.Role, ClaimValue = roleName });

  //                  // Roles //
  //                  var role = await _roleManager.FindByNameAsync(roleName);


  //                  if (role == null)
  //                  {
  //                      //create the roles and seed them to the database
  //                      role = new ApplicationRole();
  //                      role.Name = roleName;
  //                      roleResult = await _roleManager.CreateAsync(role);
  //                  }

  //                  user.Roles.Add(new IdentityUserRole<string> { UserId = user.Id, RoleId = role.Id });
  //              }

		//		var result = await _userManager.CreateAsync(user, item.password);

		//		// Option: enable account confirmation and password reset.

		//		//  return new JsonResult(result);
		//	}

		//	return new JsonResult(Ok(200));
		//}

		[HttpPost("password")]
        [AllowAnonymous]
        public async Task<IActionResult> ChangePassword([FromBody]PasswordChange model)
        {
            var user = await _userManager.FindByIdAsync(model.id);

            var check = await _userManager.CheckPasswordAsync(user, model.oldPassword);

            var result = await _userManager.ChangePasswordAsync(user, model.oldPassword, model.password);

            return new JsonResult(result);
        }

        [HttpPost("resetpassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody]PasswordReset model)
        {
            var user = await _userManager.FindByNameAsync(model.userName);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, model.password);

            return new JsonResult(result);
        }

        [HttpPost("admcenter")]
        [AllowAnonymous]
        public async Task<IActionResult> SetAdmCenter([FromBody]Dto.UserAdmCenterSave model)
        {
            //var user = _context.Set<ApplicationUser>().Where(u => u.Id == model.id).Single();
            var user = _context.Set<Model.ApplicationUser>().Where(u => u.Id == model.userId).Single();
            var admCenter = _context.Set<Model.AdmCenter>().Where(u => u.Id == model.admCenterId).Single();
            user.AdmCenterId = model.admCenterId;

            var claims = await _userManager.GetClaimsAsync(user);
            var claim = claims.FirstOrDefault(c => c.Type == "admCenter");



            if (claim != null)
            {
                var newClaim = new Claim("admCenter", admCenter.Name);
                var result = await _userManager.ReplaceClaimAsync(user, claim, newClaim);
            }
            else
            {
                var newClaim = new Claim("admCenter", admCenter.Name);
                var result = await _userManager.AddClaimAsync(user, newClaim);
            }

            _context.Update(user);
            _context.SaveChanges();

            return Ok(user);
        }

        [HttpPost("employee")]
        [AllowAnonymous]
        public async Task<IActionResult> SetEmployee([FromBody]Dto.UserEmployeeSave model)
        {
            //var user = _context.Set<ApplicationUser>().Where(u => u.Id == model.id).Single();
            var user = _context.Set<Model.ApplicationUser>().Where(u => u.Id == model.userId).Single();
            user.EmployeeId = model.employeeId;


            // CLAIM //
            var claims = await _userManager.GetClaimsAsync(user);
            var claim = claims.FirstOrDefault(c => c.Type == "employeeId");



            if (claim != null)
            {
                var newClaim = new Claim("employeeId", model.employeeId.ToString());
                var result = await _userManager.ReplaceClaimAsync(user, claim, newClaim);
            }
            else
            {
                var newClaim = new Claim("employeeId", model.employeeId.ToString());
                var result = await _userManager.AddClaimAsync(user, newClaim);
            }


            // CLAIM //

            _context.Update(user);
            _context.SaveChanges();

            return Ok(user);
        }

        [HttpPost("substitute")]
        [AllowAnonymous]
        public async Task<IActionResult> SetSubstitute([FromBody] Dto.UserSubstituteSave model)
        {
            var user = _context.Set<Model.ApplicationUser>().Where(u => u.Id == model.userId).Single();
            user.SubstituteId = model.employeeId;
            user.FromDate = model.FromDate;
            user.ToDate = model.ToDate;

            // CLAIM //
            var claims = await _userManager.GetClaimsAsync(user);
            var claim = claims.FirstOrDefault(c => c.Type == "substituteId");



            if (claim != null)
            {
                var newClaim = new Claim("substituteId", model.employeeId.ToString());
                var result = await _userManager.ReplaceClaimAsync(user, claim, newClaim);
            }
            else
            {
                var newClaim = new Claim("substituteId", model.employeeId.ToString());
                var result = await _userManager.AddClaimAsync(user, newClaim);
            }


            // CLAIM //

            _context.Update(user);
            _context.SaveChanges();

            return Ok(user);
        }

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <returns>IdentityResult</returns>
        // POST: api/identity/Delete
        [HttpPost("delete")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete([FromBody]string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            var result = await _userManager.DeleteAsync(user);

            return new JsonResult(result);
        }

        [HttpPut("updateUser/{userEmail}/{claimsValue}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateUser(string userEmail, string claimsValue)
        {

            IdentityResult roleResult;
            List<string> roles = new List<string>();
            string userRole = string.Empty;

            var user = _context.Set<Model.ApplicationUser>().Where(u => u.UserName == userEmail).Include(a => a.Claims).Single();

            userRole = user.Claims.Where(a => a.ClaimType == "role").FirstOrDefault().ClaimValue;
            
            var previousRole = await _roleManager.FindByNameAsync(userRole);
            roles.Add(previousRole.Name);

            await DeleteRolesAsync(roles, user);

            user.Claims.Where(a => a.ClaimType == "role").FirstOrDefault().ClaimValue = claimsValue;

            // Roles //

            var role = await _roleManager.FindByNameAsync(claimsValue);

            if (role == null)
            {
                //create the roles and seed them to the database
                role = new ApplicationRole();
                role.Name = claimsValue;
                roleResult = await _roleManager.CreateAsync(role);
            }


            user.Roles.Add(new IdentityUserRole<string> { UserId = user.Id, RoleId = role.Id });

            // Role // 
            _context.SaveChanges();

            return Ok();
        }


        [HttpPost("updateuserroles")]
        [AllowAnonymous]
        public async Task<RequestResult> UpdateUserRoles([FromBody] RoleSave updatedRoles)
        {
            if (HttpContext.User.Identity.Name != null)
            {
                var userName = HttpContext.User.Identity.Name;
                var connectedUser = await _userManager.FindByEmailAsync(userName);

                if (connectedUser == null)
                {
                    connectedUser = await _userManager.FindByNameAsync(userName);
                }

                if (connectedUser != null)
                {
                    _context.UserId = connectedUser.Id.ToString();

                    IdentityResult roleResult;
                    //List<string> roles = new List<string>();
                    //string userRole = string.Empty;

                    if(updatedRoles != null)
					{
                        var user = _context.Set<Model.ApplicationUser>().Where(u => u.Id == updatedRoles.UserId).Include(a => a.Claims).Single();

                        //userRole = user.Claims.Where(a => a.ClaimType == "role").FirstOrDefault().ClaimValue;

                        var oldRoles = await _userManager.GetRolesAsync(user);

                        for (int r = 0; r < oldRoles.Count; r++)
                        {
                            // var previousRole = await _roleManager.FindByNameAsync(userRole);
                            var rol = await _roleManager.FindByNameAsync(oldRoles[r]);
                            await _userManager.RemoveFromRoleAsync(user, rol.Name);
                            //roles.Add(rol.Name);

                            //await DeleteRolesAsync(roles, user);

                            //  user.Claims.Where(a => a.ClaimType == "role").FirstOrDefault().ClaimValue = claimsValue;
                        }


                        for (int i = 0; i < updatedRoles.RoleIds.Length; i++)
						{
                            var newRole = await _roleManager.FindByIdAsync(updatedRoles.RoleIds[i]);

                            // Roles //

                            // var role = await _roleManager.FindByNameAsync(newRole.Name);

                            if (newRole == null)
                            {
                                //create the roles and seed them to the database
                                newRole = new ApplicationRole();
                                newRole.Name = newRole.Name;
                                roleResult = await _roleManager.CreateAsync(newRole);
                            }


                            user.Roles.Add(new IdentityUserRole<string> { UserId = user.Id, RoleId = newRole.Id });

                            // Role // 
                            _context.SaveChanges();
                        }
                       
                    }

                    return new Model.RequestResult { Success = true, Message = "OK", RequestId = 0 };
                }
                else
                {
                    return new Model.RequestResult { Success = false, Message = $"Userul nu exista!", RequestId = 0 };
                }


            }
            else
            {
                return new Model.RequestResult { Success = false, Message = $"Va rugam sa va autentificati!", RequestId = 0 };
            }
            
        }

		[HttpPost("updateMobilePhone")]
		public async Task<UserMobilePhoneResult> UpdateMobilePhone([FromBody] MobilePhoneSave mobilephone)
		{
            Model.MobilePhone mPhone = null;
                 
			if (HttpContext.User.Identity.Name != null)
			{
				var userName = HttpContext.User.Identity.Name;
				var connectedUser = await _userManager.FindByEmailAsync(userName);

				if (connectedUser == null)
				{
					connectedUser = await _userManager.FindByNameAsync(userName);
				}

				if (connectedUser != null)
				{
                    mPhone = await _context.Set<Model.MobilePhone>().Where(a => a.Id == mobilephone.MobilePhoneId).FirstOrDefaultAsync();
					_context.UserId = connectedUser.Id.ToString();
                    connectedUser.PhoneNumber = mPhone != null ? mPhone.Name : string.Empty;
                    connectedUser.MobilePhoneId = mPhone != null ? mPhone.Id : null;

					// CLAIM //
					var claims = await _userManager.GetClaimsAsync(connectedUser);
					var claim = claims.FirstOrDefault(c => c.Type == "mobilePhoneId");



					if (claim != null)
					{
						var newClaim = new Claim("mobilePhoneId", connectedUser.MobilePhoneId.ToString());
						var result = await _userManager.ReplaceClaimAsync(connectedUser, claim, newClaim);
					}
					else
					{
						var newClaim = new Claim("mobilePhoneId", connectedUser.MobilePhoneId.ToString());
						var result = await _userManager.AddClaimAsync(connectedUser, newClaim);
					}


					// CLAIM //


					_context.Update(connectedUser);
                    _context.SaveChanges();

					return new Model.UserMobilePhoneResult { Success = true, Message = "Datele au fost salvate cu success"};
				}
				else
				{
					return new Model.UserMobilePhoneResult { Success = false, Message = $"Userul nu exista!"};
				}


			}
			else
			{
				return new Model.UserMobilePhoneResult { Success = false, Message = $"Va rugam sa va autentificati!" };
			}

		}

		public async Task DeleteRolesAsync(List<string> deleteList, Model.ApplicationUser user)
        {
            if (user != null)
            {
                foreach (var roleName in deleteList)
                {
                    IdentityResult deletionResult = await _userManager.RemoveFromRoleAsync(user, roleName);
                }
            }
        }


        //[HttpPost("device")]
        //[AllowAnonymous]
        //public async Task<IActionResult> SetDevice([FromBody] Dto.UserDeviceSave model)
        //{

        //    var user = _context.Set<Model.ApplicationUser>().Where(u => u.Id == model.userId).Single();
        //    var employee = _context.Set<Model.Employee>().Where(u => u.Id == user.EmployeeId).Single();

        //    user.DeviceId = model.deviceId;
        //    device.EmployeeId = user.EmployeeId;

        //    var claims = await _userManager.GetClaimsAsync(user);
        //    var claim = claims.FirstOrDefault(c => c.Type == "uui");
        //    var claimPrinterAddress = claims.FirstOrDefault(c => c.Type == "printerAddress");



        //    if (claim != null)
        //    {
        //        var newClaim = new Claim("uui", device.UUI);
        //        var result = await _userManager.ReplaceClaimAsync(user, claim, newClaim);
        //    }
        //    else
        //    {
        //        var newClaim = new Claim("uui", device.UUI);
        //        var result = await _userManager.AddClaimAsync(user, newClaim);
        //    }


        //    if (claimPrinterAddress != null)
        //    {
        //        var newClaimPrinterAddress = new Claim("printerAddress", employee.ERPCode);
        //        var result = await _userManager.ReplaceClaimAsync(user, claimPrinterAddress, newClaimPrinterAddress);
        //    }
        //    else
        //    {
        //        var newClaimPrinterAddress = new Claim("printerAddress", employee.ERPCode);
        //        var result = await _userManager.AddClaimAsync(user, newClaimPrinterAddress);
        //    }

        //    _context.Update(user);
        //   await _context.SaveChangesAsync();

        //    return Ok(user);
        //}

   //     [HttpPut("updateDevice/{userId}")]
   //     [AllowAnonymous]
   //     public async Task<IActionResult> UpdateDevice(string userId)
   //     {

   //         var user = _context.Set<Model.ApplicationUser>().Where(u => u.Id == userId).FirstOrDefault();
   //         var device = _context.Set<Model.Device>().Where(u => u.Id == user.DeviceId).FirstOrDefault();
   //         var claims = await _userManager.GetClaimsAsync(user);
   //         var claim = claims.FirstOrDefault(c => c.Type == "uui");
   //         var claimPrinterAddress = claims.FirstOrDefault(c => c.Type == "printerAddress");



   //         if (claim != null)
   //         {
   //             var result = await _userManager.RemoveClaimAsync(user, claim);
   //         }


   //         if (claimPrinterAddress != null)
   //         {
   //             var result = await _userManager.RemoveClaimAsync(user, claimPrinterAddress);
   //         }

   //         if(device != null)
			//{
   //             device.EmployeeId = null;
   //         }

            
   //         user.DeviceId = null;

   //         await _context.SaveChangesAsync();

   //         return Ok();
   //     }

		// Add other methods.

		[HttpGet("export")]
		public IActionResult Export(string sortColumn, string sortDirection, int? page, int? pageSize, string filter, string role)
		{
			var users = _userManager.Users;
			int rowNumber = 0;
            if (role == "all")
			{
                users = _userManager.Users
                    .Include(c => c.Claims)
                    .Include(c => c.Employee)
                        .ThenInclude(c => c.Company)
                    .Include(c => c.Employee)
                        .ThenInclude(c => c.CostCenter)
                            .ThenInclude(c => c.Division)
                                .ThenInclude(c => c.Department)
                    .Include(c => c.Substitute);

            }
			else
			{
                users = _userManager.Users
                   .Include(c => c.Claims)
					.Include(c => c.Employee)
						.ThenInclude(c => c.Company)
					.Include(c => c.Employee)
						.ThenInclude(c => c.CostCenter)
							.ThenInclude(c => c.Division)
								.ThenInclude(c => c.Department)
					.Include(c => c.Substitute)
					.Where(u => u.Claims.Any(c => c.ClaimValue == role));
            }



            if (filter != null && filter.Length > 0)
            {
				users = users.Where(u => u.UserName.Contains(filter) || u.GivenName.Contains(filter) || u.FamilyName.Contains(filter));
            }

			var usersMap = mapper.Map<List<Model.ApplicationUser>, List<Dto.ApplicationUser>>(users.ToList());

            using (ExcelPackage package = new ExcelPackage())
			{

				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Emag");
				//First add the headers
				worksheet.Cells[1, 1].Value = "Ultima logare";
				worksheet.Cells[1, 2].Value = "Username";
				worksheet.Cells[1, 3].Value = "Rol";
				worksheet.Cells[1, 4].Value = "Angajat marca";
				worksheet.Cells[1, 5].Value = "Angajat nume";
				worksheet.Cells[1, 6].Value = "Anagajt prenume";
				worksheet.Cells[1, 7].Value = "Angajat email";
				worksheet.Cells[1, 8].Value = "Angajat companie";
				worksheet.Cells[1, 9].Value = "Angajat centru de cost";
				worksheet.Cells[1, 10].Value = "Angajat departament";
				worksheet.Cells[1, 11].Value = "Angajat B.U.";
				worksheet.Cells[1, 12].Value = "Angajat Activ(DA/NU)";
				worksheet.Cells[1, 13].Value = "Angajat WFH confirmare(DA/NU)";

				int recordIndex = 2;
				int count = users.Count();

				foreach (var item in users)
				{
					rowNumber++;

					int diff = recordIndex - count;

					if (diff > 0)
					{
						diff = 0;
					}

					worksheet.Cells[recordIndex, 1].Value = item.LastLogin;
					worksheet.Cells[recordIndex, 1].Style.Numberformat.Format = "yyyy-mm-dd HH:mm:ss";
					worksheet.Cells[recordIndex, 2].Value = item.UserName;
					worksheet.Cells[recordIndex, 3].Value = item.Claims.Count() > 0 ? item.Claims.Where(c => c.ClaimType == "role").ToList().FirstOrDefault().ClaimValue : "";
					worksheet.Cells[recordIndex, 4].Value = item.Employee != null ? item.Employee.InternalCode : "";
					worksheet.Cells[recordIndex, 5].Value = item.Employee != null ? item.Employee.FirstName : "";
					worksheet.Cells[recordIndex, 6].Value = item.Employee != null ? item.Employee.LastName : "";
					worksheet.Cells[recordIndex, 7].Value = item.Employee != null ? item.Employee.Email : "";
					worksheet.Cells[recordIndex, 8].Value = item.Employee != null && item.Employee.Company != null ? item.Employee.Company.Code : "";
					worksheet.Cells[recordIndex, 9].Value = item.Employee != null && item.Employee.CostCenter != null ? item.Employee.CostCenter.Code : "";
					worksheet.Cells[recordIndex, 10].Value = item.Employee != null && item.Employee.CostCenter != null && item.Employee.CostCenter.Division != null ? item.Employee.CostCenter.Division.Name : "";
					worksheet.Cells[recordIndex, 11].Value = item.Employee != null && item.Employee.CostCenter != null && item.Employee.CostCenter.Division != null && item.Employee.CostCenter.Division.Department != null ? item.Employee.CostCenter.Division.Department.Name : "";
					worksheet.Cells[recordIndex, 12].Value = item.Employee != null && item.Employee.IsDeleted ? "NU" : "DA";
					worksheet.Cells[recordIndex, 13].Value = item.Employee != null && item.Employee.IsConfirmed ? "DA" : "NU";

					if (diff == 0)
					{

						for (int i = 1; i < 14; i++)
						{
							worksheet.Cells[1, i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[1, i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[1, i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[1, i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[1, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[1, i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[1, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
							worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 104, 174));
							worksheet.Cells[1, i].Style.Font.SetFromFont(new Font("Times New Roman", 10));

						}

						worksheet.Row(1).Height = 35.00;
						worksheet.Row(1).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
						worksheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.View.FreezePanes(2, 1);

						using (var cells = worksheet.Cells[1, 1, count + 1, 13])
						{
							cells.Style.Font.Bold = false;
							cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
							cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 104, 174));
							cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
							cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
							cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
							cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
							cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
							cells.Style.Font.SetFromFont(new Font("Times New Roman", 10));

						}


						using (var cells = worksheet.Cells[1, 1, 1, 13])
						{
							cells.Style.Font.Bold = true;
							cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
							cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(235, 29, 34));
							cells.Style.Font.Color.SetColor(Color.Black);
						}

						using (var cells = worksheet.Cells[2, 1, count + 3, 13])
						{
							cells.Style.Font.Bold = false;
							cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
							cells.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255));
						}

						using (var cells = worksheet.Cells[2, 1, count + 3, 13])
						{
							for (int i = 2; i < count + 2; i++)
							{
								worksheet.Row(i).Height = 15.00;
								worksheet.Row(i).Style.VerticalAlignment = ExcelVerticalAlignment.Center;
								worksheet.Row(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

								worksheet.Cells[$"A{i}"].Style.WrapText = true;
								worksheet.Cells[$"B{i}"].Style.WrapText = true;
								worksheet.Cells[$"C{i}"].Style.WrapText = true;
								worksheet.Cells[$"D{i}"].Style.WrapText = true;
								worksheet.Cells[$"E{i}"].Style.WrapText = true;
								worksheet.Cells[$"F{i}"].Style.WrapText = true;
								worksheet.Cells[$"G{i}"].Style.WrapText = true;
								worksheet.Cells[$"H{i}"].Style.WrapText = true;
								worksheet.Cells[$"I{i}"].Style.WrapText = true;
								worksheet.Cells[$"J{i}"].Style.WrapText = true;
								worksheet.Cells[$"K{i}"].Style.WrapText = true;
								worksheet.Cells[$"L{i}"].Style.WrapText = true;
								worksheet.Cells[$"M{i}"].Style.WrapText = true;

							}
						}


						worksheet.View.ShowGridLines = false;
						worksheet.View.ZoomScale = 100;

						for (int i = 1; i < 14; i++)
						{
							worksheet.Column(i).AutoFit();
						}

						worksheet.Column(1).Width = 12.00;
						worksheet.Column(2).Width = 10.00;
						worksheet.Column(3).Width = 14.00;
						worksheet.Column(4).Width = 20.00;
						worksheet.Column(5).Width = 15.00;
						worksheet.Column(6).Width = 15.00;
						worksheet.Column(7).Width = 30.00;
						worksheet.Column(8).Width = 30.00;
						worksheet.Column(9).Width = 30.00;
						worksheet.Column(10).Width = 30.00;
						worksheet.Column(11).Width = 30.00;
						worksheet.Column(12).Width = 30.00;
						worksheet.Column(13).Width = 30.00;

						worksheet.Cells["A1:M1"].AutoFilter = true;

					}

					recordIndex++;
				}


				using (var cells = worksheet.Cells[1, 1, 1, 13])
				{
					cells.Style.Font.Bold = true;
					cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
					cells.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

				}

				string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
				//HttpContext.Response.ContentType = entityFile.FileType;
				HttpContext.Response.ContentType = contentType;
				FileContentResult result = new FileContentResult(package.GetAsByteArray(), contentType)
				{
					FileDownloadName = "Export.xlsx"
				};

				return result;

			}
		}

		[HttpPost("adminUpdateMobilePhone")]
		public async Task<UserMobilePhoneResult> AdminUpdateMobilePhone([FromBody] MobilePhoneSave mobilephone)
		{
			Model.MobilePhone mPhone = null;

			if (HttpContext.User.Identity.Name != null)
			{
				var userName = HttpContext.User.Identity.Name;
				var connectedUser = await _userManager.FindByEmailAsync(mobilephone.Email);

				if (connectedUser == null)
				{
					connectedUser = await _userManager.FindByNameAsync(userName);
				}

				if (connectedUser != null)
				{
					mPhone = await _context.Set<Model.MobilePhone>().Where(a => a.Id == mobilephone.MobilePhoneId).FirstOrDefaultAsync();
					_context.UserId = connectedUser.Id.ToString();
					connectedUser.PhoneNumber = mPhone != null ? mPhone.Name : string.Empty;
					connectedUser.MobilePhoneId = mPhone != null ? mPhone.Id : null;

					// CLAIM //
					var claims = await _userManager.GetClaimsAsync(connectedUser);
					var claim = claims.FirstOrDefault(c => c.Type == "mobilePhoneId");



					if (claim != null)
					{
						var newClaim = new Claim("mobilePhoneId", connectedUser.MobilePhoneId.ToString());
						var result = await _userManager.ReplaceClaimAsync(connectedUser, claim, newClaim);
					}
					else
					{
						var newClaim = new Claim("mobilePhoneId", connectedUser.MobilePhoneId.ToString());
						var result = await _userManager.AddClaimAsync(connectedUser, newClaim);
					}


					// CLAIM //


					_context.Update(connectedUser);
					_context.SaveChanges();

					return new Model.UserMobilePhoneResult { Success = true, Message = "Datele au fost salvate cu success" };
				}
				else
				{
					return new Model.UserMobilePhoneResult { Success = false, Message = $"Userul nu exista!" };
				}


			}
			else
			{
				return new Model.UserMobilePhoneResult { Success = false, Message = $"Va rugam sa va autentificati!" };
			}

		}

		[HttpPut("removeMobilePhone/{userId}")]
		[AllowAnonymous]
		public async Task<IActionResult> RemoveMobilePhone(string userId)
		{

			var user = _context.Set<Model.ApplicationUser>().Where(u => u.Id == userId).FirstOrDefault();
			var claims = await _userManager.GetClaimsAsync(user);
			var claim = claims.FirstOrDefault(c => c.Type == "mobilePhoneId");



			if (claim != null)
			{
				var result = await _userManager.RemoveClaimAsync(user, claim);
			}


			user.MobilePhoneId = null;
			user.PhoneNumber = string.Empty;

			await _context.SaveChangesAsync();

			return Ok();
		}

	}
}