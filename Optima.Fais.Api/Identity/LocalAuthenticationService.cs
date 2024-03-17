using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using IdentityModel;

namespace Optima.Fais.Api.Identity
{
    public class LocalAuthenticationService : ILocalAuthenticationService
    {
        private readonly UserManager<Model.ApplicationUser> _userManager;

        public LocalAuthenticationService(UserManager<Model.ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IAppUser> LoginAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user != null)
            {
                bool validPassword = await _userManager.CheckPasswordAsync(user, password);

                if (validPassword)
                {
					user.LastLogin = DateTime.Now;
					UserLoginInfo userLoginInfo = new UserLoginInfo("LOCAL", Guid.NewGuid().ToString(), user.Email);
					await _userManager.AddLoginAsync(user, userLoginInfo);
					await _userManager.UpdateAsync(user);
					var claims = await _userManager.GetClaimsAsync(user);

                    var roles = claims.Where(c => c.Type == JwtClaimTypes.Role).Select(c => c.Value).ToArray();

                    return new AppUser {
                        UserName = user.UserName,
                        DisplayName = user.GivenName + " " + user.FamilyName,
                        Email = user.Email,
                        Roles = roles
                    };
                }
            }

            return null;
        }
    }
}
