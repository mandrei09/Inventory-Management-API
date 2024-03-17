using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Optima.Fais.Api.Identity;
using Optima.Fais.Dto;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Controllers.Identity
{
	public class CustomUserValidator : IResourceOwnerPasswordValidator
	{
        //private readonly IAuthenticationService ldapService = null;
        private readonly ILdapAuthenticationService _ldapService = null;
        private readonly ILocalAuthenticationService _localService = null;
        private readonly UserManager<Model.ApplicationUser> _userManager;

        public CustomUserValidator(ILdapAuthenticationService ldapService, ILocalAuthenticationService localService,
            UserManager<Model.ApplicationUser> userManager)
        {
            this._ldapService = ldapService;
            this._localService = localService;
            this._userManager = userManager;
        }

        public UserManager<User> UserManager { get; }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            // var user = await UserManager.FindByNameAsync(context.UserName);

            // validate username/password against Ldap

            //IAppUser user = null;
            IAuthenticationService authService = null;

            //if (context.UserName.ToLower().StartsWith("t0") || context.UserName.ToLower().StartsWith("s0")) authService = this._ldapService;
            //else authService = this._localService;

            // for other error types just write the info without the FailedRecipient
            //using (var errorfile = System.IO.File.CreateText("MOBILE-AUTH-STEP1 " + DateTime.Now.Ticks + ".txt"))
            //{
            //    errorfile.WriteLine(context.UserName);
            //    errorfile.WriteLine(context.Password);

            //};

            if (context.UserName.Contains("optima")) authService = this._localService;
            else authService = this._ldapService;

            var user = await authService.LoginAsync(context.UserName, context.Password);

            //using (var errorfile = System.IO.File.CreateText("MOBILE-AUTH-STEP2 " + DateTime.Now.Ticks + ".txt"))
            //{
            //    errorfile.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(user, Formatting.Indented));

            //};


            if (user == null)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Username or password is incorrect");
                return;
            }

			//var passwordValid = await UserManager.CheckPasswordAsync(user, context.Password);
			//if (!passwordValid)
			//{
			//    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Username or password is incorrect");
			//    return;

			//}

			//var claims = new List<Claim>
			//		 {
			//			 new Claim(JwtClaimTypes.Name, user.UserName),
			//			 new Claim(JwtClaimTypes.Email, user.Email),

			//		 };

			//         claims.Add(new Claim("employeeId", "4"));
			//         claims.Add(new Claim("role", "administrator"));

			//foreach (var role in user.Roles)
			//{
			//	claims.Add(new Claim(JwtClaimTypes.Role, role));
			//}

			var localUser = await _userManager.FindByNameAsync(user.UserName);

            //using (var errorfile = System.IO.File.CreateText("MOBILE-AUTH-localuser " + DateTime.Now.Ticks + ".txt"))
            //{
            //    errorfile.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(localUser, Formatting.Indented));

            //};

            var claims = await _userManager.GetClaimsAsync(localUser);

            //using (var errorfile = System.IO.File.CreateText("MOBILE-AUTH-claims " + DateTime.Now.Ticks + ".txt"))
            //{
            //    errorfile.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(localUser, Formatting.Indented));

            //};
            claims.Add(new Claim(JwtClaimTypes.Name, user.UserName));
            claims.Add(new Claim(JwtClaimTypes.Email, user.Email));
            claims.Add(new Claim(JwtClaimTypes.Id, localUser.Id));

			try
			{
                context.Result = new GrantValidationResult(user.UserName, "password", claims);

                //using (var errorfile = System.IO.File.CreateText("MOBILE-AUTH-claims " + DateTime.Now.Ticks + ".txt"))
                //{
                //    errorfile.WriteLine(context.Result.ToString());

                //};
            }
			catch (Exception ex)
			{

                //using (var errorfile = System.IO.File.CreateText("MOBILE-AUTH-STEP_final " + DateTime.Now.Ticks + ".txt"))
                //{
                //    errorfile.WriteLine(ex.StackTrace);

                //};
            }
            

           
        }
    }
}
