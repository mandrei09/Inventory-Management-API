using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using IdentityModel;
using Optima.Fais.Model;
using Optima.Fais.Data;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace Optima.Fais.Api.Identity
{
    public class LdapAuthenticationService : ILdapAuthenticationService
    {
        //private const string MemberOfAttribute = "memberOf";
        //private const string DisplayNameAttribute = "displayName";
        //private const string SAMAccountNameAttribute = "sAMAccountName";
        //private const string MailAttribute = "userPrincipalName";

        private readonly LdapConfig _config;
        private readonly LdapConnection _connection;
        private readonly UserManager<Model.ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
		private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configAD;

        public LdapAuthenticationService(IOptions<LdapConfig> configAccessor, 
            UserManager<Model.ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ApplicationDbContext context, IConfiguration configAD)
        {
            _config = configAccessor.Value;
            _configAD = configAD;

			//LdapConnectionOptions options = new LdapConnectionOptions()
			//.ConfigureRemoteCertificateValidationCallback(((a, b, c, d) => true))
			//.UseSsl();

			//_connection = new LdapConnection(options);
			_connection = new LdapConnection();

            _userManager = userManager;
            _roleManager = roleManager;
			_context = context;
		}

		public async Task<IAppUser> LoginAsync(string username, string password)
		{
			try
			{
				return await DoLoginAsync(username, password);
			}
			catch (Exception e)
			{
				Console.Write("Meldingen kunne ikke sendes til en eller flere mottakere.", ConsoleColor.Red);
				Console.Write(e.Message, ConsoleColor.DarkRed);

				// for other error types just write the info without the FailedRecipient
				using (var errorfile = System.IO.File.CreateText("MOBILE-" + DateTime.Now.Ticks + ".txt"))
				{
					errorfile.WriteLine(e.StackTrace);
					errorfile.WriteLine(e.ToString());

				};

				return null;
			}
		}

		private async Task<IAppUser> DoLoginAsync(string username, string password)
        {
            // _connection.SecureSocketLayer = true;

            //var urlAD = _configAD.GetSection("ldapLogin").GetValue<string>("Url");
            //var usernameAD = _configAD.GetSection("ldapLogin").GetValue<string>("Username");
            //var passwordAD = _configAD.GetSection("ldapLogin").GetValue<string>("Password");

            //var MemberOfAttributeAD = _configAD.GetSection("ldapLogin").GetValue<string>("MemberOfAttribute");
            //var DisplayNameAttributeAD = _configAD.GetSection("ldapLogin").GetValue<string>("DisplayNameAttribute");
            //var SAMAccountNameAttributeAD = _configAD.GetSection("ldapLogin").GetValue<string>("SAMAccountNameAttribute");
            //var EmailAttributeAD = _configAD.GetSection("ldapLogin").GetValue<string>("EmailAttribute");

            //var SearchFilterAD = _configAD.GetSection("ldapLogin").GetValue<string>("SearchFilter");
            //var SearchBaseAD = _configAD.GetSection("ldapLogin").GetValue<string>("SearchBase");

            _connection.Connect(_config.Url, LdapConnection.DefaultPort);
            _connection.Bind(_config.Username, _config.Password);
            //_connection.SecureSocketLayer = true;

            List<string> typesOnly = new List<string>();
            if (_config.MemberOfAttribute.Length > 0) typesOnly.Add(_config.MemberOfAttribute);
            if (_config.DisplayNameAttribute.Length > 0) typesOnly.Add(_config.DisplayNameAttribute);
            if (_config.SAMAccountNameAttribute.Length > 0) typesOnly.Add(_config.SAMAccountNameAttribute);
            if (_config.EmailAttribute.Length > 0) typesOnly.Add(_config.EmailAttribute);

            var searchFilter = String.Format(_config.SearchFilter, username);
            var result = _connection.Search(
                _config.SearchBase,
                LdapConnection.ScopeSub,
                searchFilter,
                typesOnly.ToArray(),
                false
            );

            try
            {
                var user = result.Next();
                if (user != null)
                {
                    _connection.Bind(user.Dn, password);
                    if (_connection.Bound)
                    {
                        LdapAttribute memberOfAttr = null;
                        LdapAttribute displayNameAttr = null;
                        LdapAttribute sAMAccountNameAttr = null;
                        LdapAttribute emailAttr = null;

                        var attributes = user.GetAttributeSet();

                        if ((_config.MemberOfAttribute.Length > 0) && (attributes.ContainsKey(_config.MemberOfAttribute))) memberOfAttr = user.GetAttribute(_config.MemberOfAttribute);
                        if ((_config.DisplayNameAttribute.Length > 0) && (attributes.ContainsKey(_config.DisplayNameAttribute))) displayNameAttr = user.GetAttribute(_config.DisplayNameAttribute);
                        if ((_config.SAMAccountNameAttribute.Length > 0) && (attributes.ContainsKey(_config.SAMAccountNameAttribute))) sAMAccountNameAttr = user.GetAttribute(_config.SAMAccountNameAttribute);
                        if ((_config.EmailAttribute.Length > 0) && (attributes.ContainsKey(_config.EmailAttribute))) emailAttr = user.GetAttribute(_config.EmailAttribute);

                        string userName = sAMAccountNameAttr != null ? sAMAccountNameAttr.StringValue : string.Empty;
                        string email = emailAttr != null ? emailAttr.StringValue : string.Empty;
                        string displayName = displayNameAttr != null ? displayNameAttr.StringValue : string.Empty;
                        string[] roles = null;

                        var localUser = await _userManager.FindByNameAsync(username);

                        if (localUser == null)
						{
                            var employee = _context.Employees.Where(e => e.IsDeleted == false && e.Email == email).FirstOrDefault();

                            localUser = new Model.ApplicationUser
                            {
                                UserName = userName,
                                Email = email,
                                GivenName = displayName,
                                FamilyName = displayName,
                                LockoutEnabled = false,
                                Employee = employee
                            };

                            // Claims.
                            localUser.Claims.Add(new IdentityUserClaim<string> { ClaimType = JwtClaimTypes.GivenName, ClaimValue = displayName });
                            localUser.Claims.Add(new IdentityUserClaim<string> { ClaimType = JwtClaimTypes.FamilyName, ClaimValue = displayName });

                            if (employee != null)
                            {
                                localUser.Claims.Add(new IdentityUserClaim<string> { ClaimType = "employeeId", ClaimValue = employee.Id.ToString()});
                            }

                            string roleNames = "user";
                            roles = roleNames.Split(";".ToCharArray());

                            foreach (string role in roles)
                            {
                                localUser.Claims.Add(new IdentityUserClaim<string> { ClaimType = JwtClaimTypes.Role, ClaimValue = role });

                                // Roles //
                                var localRole = await _roleManager.FindByNameAsync(role);


                                if (localRole == null)
                                {
                                    //create the roles and seed them to the database
                                    localRole = new ApplicationRole();
                                    localRole.Name = role;
                                    await _roleManager.CreateAsync(localRole);
                                }

                                localUser.Roles.Add(new IdentityUserRole<string> { UserId = localUser.Id, RoleId = localRole.Id });
                            }

                            await _userManager.CreateAsync(localUser);
                        }
                        else
						{
                            var claims = await _userManager.GetClaimsAsync(localUser);
                            roles = claims.Where(c => c.Type == JwtClaimTypes.Role).Select(c => c.Value).ToArray();
                        }

                        localUser.LastLogin = DateTime.Now;
						UserLoginInfo userLoginInfo = new UserLoginInfo("CLIENT", Guid.NewGuid().ToString(), localUser.Email);
						await _userManager.AddLoginAsync(localUser, userLoginInfo);
                        await _userManager.UpdateAsync(localUser);

						return new AppUser
                        {
                            DisplayName = displayName,
                            UserName = userName,
                            Email = email,
                            Roles = roles
                            //                    Roles =  memberAttr.StringValueArray
                            //.Select(x => GetGroup(x))
                            //.Where(x => x != null)
                            //.Distinct()
                            //.ToArray()
                        };
                    }
                }
            }
			catch (Exception e)
			{
                Console.Write("Error", ConsoleColor.Red);
                Console.Write(e.Message, ConsoleColor.DarkRed);

                // for other error types just write the info without the FailedRecipient
                //using (var errorfile = System.IO.File.CreateText("MOBILE1-" + DateTime.Now.Ticks + ".txt"))
                //{
                //    errorfile.WriteLine(e.StackTrace);
                //    errorfile.WriteLine(e.ToString());

                //};
            }
			finally
            {
                _connection.Disconnect();
            }

            return null;
        }

		//public Task<IAppUser> LoginAsync(string username, string password)
		//{
  //          //throw new NotImplementedException();

  //          return Task.Run(() => Login(username, password));
  //      }

		private string GetGroup(string value)
        {
            Match match = Regex.Match(value, "^CN=([^,]*)");
            if (!match.Success)
            {
                return null;
            }

            return match.Groups[1].Value;
        }
    }
}
