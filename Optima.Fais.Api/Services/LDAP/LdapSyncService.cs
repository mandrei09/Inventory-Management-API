using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Novell.Directory.Ldap;
using Optima.Fais.Api.Identity;
using Optima.Fais.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services.LDAP
{
	public class LdapSyncService : ILdapSyncService
	{

		//private const string FirstNameAttribute = "sn";
		//private const string LastNameAttribute = "givenName";
		//private const string FullNameAttribute = "displayName";
		//private const string EmailAttribute = "userPrincipalName";
		//private const string DepartmentAttribute = "physicalDeliveryOfficeName";
		//private const string IsActiveAttribute = "countryCode";

		private readonly LdapConfig _config;
		private readonly LdapConnection _connection;

		public LdapSyncService(IOptions<LdapConfig> configAccessor, IServiceProvider services)
		{
			_config = configAccessor.Value;
			_connection = new LdapConnection();
			Services = services;
		}

		public IServiceProvider Services { get; }

		public async Task<int> SyncEmployeesAsync()
		{
			int countChanges = 0;

			// var employees = GetAllEmployees();

			using (var scope = Services.CreateScope())
			{
				var dbContext =
				   scope.ServiceProvider
					   .GetRequiredService<ApplicationDbContext>();

				Model.SyncStatus syncStatus = await dbContext.Set<Model.SyncStatus>().Where(s => s.IsDeleted == false && s.Code == "LDAP").SingleOrDefaultAsync();

				if (syncStatus == null)
				{
					syncStatus = new Model.SyncStatus()
					{
						Code = "LDAP",
						Name = "LDAP angajati",
						SyncEnabled = true,
						SyncInterval = 1440
					};

					dbContext.Add(syncStatus);
					dbContext.SaveChanges();
				}

				if (syncStatus.SyncEnabled)
				{
					var lastSync = syncStatus.SyncLast;

					if (((lastSync == null) || (lastSync.Value.AddMinutes(syncStatus.SyncInterval) < DateTime.Now)))
					{
						var employees = GetAllEmployees();


						//List<LdapEmployee> employees = new List<LdapEmployee>();
						//var path = Path.Combine("upload", DateTime.UtcNow.ToString("yyyyMMdd"));

						//using (StreamReader r = new StreamReader(Directory.GetCurrentDirectory() + @"\" + path + @"\employee.txt"))
						//{
						//	string json = r.ReadToEnd();
						//	employees = JsonConvert.DeserializeObject<List<LdapEmployee>>(json);
						//}


						//using (var errorfile = System.IO.File.CreateText("resultAD" + DateTime.Now.Ticks + ".txt"))
						//{
						//	errorfile.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(employees, Formatting.Indented));

						//};

						if (employees.Count > 0)
						{
							syncStatus.SyncStart = DateTime.Now;

							var IdEmployee = await dbContext.Set<Model.DimensionERP>().FromSql("UpdateAsDeleted {0}", "Employee").SingleAsync();
							var IdManager = await dbContext.Set<Model.DimensionERP>().FromSql("UpdateAsDeleted {0}", "Manager").SingleAsync();
							var pathOutput = Path.Combine("employeeLDAP", DateTime.UtcNow.ToString("yyyyMMdd"));

							if (!Directory.Exists(pathOutput))
							{
								Directory.CreateDirectory(pathOutput);
							}

							for (int i = 0; i < employees.Count; i++)
							{
								try
								{
									var employeeId = dbContext.Set<Model.ERPImportResult>().FromSql("AddOrUpdateLDAPEmployee {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}",
									employees[i].FirstName, employees[i].LastName, employees[i].FullName, employees[i].Email, employees[i].InternalCode,
									employees[i].Department, employees[i].IsActive, employees[i].CostCenter, employees[i].Manager, employees[i].Company);

									using (var errorfile = System.IO.File.CreateText(Directory.GetCurrentDirectory() + @"\" + pathOutput + @"\employeeId_" + employeeId.FirstOrDefault().Id + ".txt"))
									{
										errorfile.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(employeeId.FirstOrDefault().Id, Formatting.Indented));

									};
								}
                                catch (Exception ex)
                                {
                                    string errorFolderPath = "errors";

                                    if (!System.IO.Directory.Exists(errorFolderPath))
                                    {
                                        System.IO.Directory.CreateDirectory(errorFolderPath);
                                    }

                                    string errorFilePath = System.IO.Path.Combine(errorFolderPath, "error-insert-employee" + DateTime.Now.Ticks + ".txt");

                                    using (var errorfile = System.IO.File.CreateText(errorFilePath))
                                    {
                                        errorfile.WriteLine(ex.StackTrace);
                                        errorfile.WriteLine(ex.ToString());
                                        System.Diagnostics.Debug.WriteLine("Log Exception: " + ex.Message);
                                    }
                                }

                                countChanges++;
							}
						}
						syncStatus.SyncEnd = DateTime.Now;
						syncStatus.SyncLast = DateTime.Now;
						dbContext.Update(syncStatus);
						dbContext.SaveChanges();
					}
				}
			}
			return countChanges;
		}

		public List<LdapEmployee> GetAllEmployees()
		{
			List<LdapEmployee> employees = null;

			_connection.SecureSocketLayer = true;

			_connection.Connect(_config.Url, LdapConnection.DefaultSslPort);
			_connection.Bind(_config.Username, _config.Password);


			List<string> typesOnly = new List<string>();
			//if (_config.Attribute.Length > 0) typesOnly.Add(_config.Attribute);
			if (_config.FirstNameAttribute.Length > 0) typesOnly.Add(_config.FirstNameAttribute);
			if (_config.LastNameAttribute.Length > 0) typesOnly.Add(_config.LastNameAttribute);
			if (_config.FullNameAttribute.Length > 0) typesOnly.Add(_config.FullNameAttribute);
			if (_config.EmailAttribute.Length > 0) typesOnly.Add(_config.EmailAttribute);
			if (_config.DepartmentAttribute.Length > 0) typesOnly.Add(_config.DepartmentAttribute);
			if (_config.IsActiveAttribute.Length > 0) typesOnly.Add(_config.IsActiveAttribute);
			if (_config.InternalCodeAttribute.Length > 0) typesOnly.Add(_config.InternalCodeAttribute);
			if (_config.CostCenterAttribute.Length > 0) typesOnly.Add(_config.CostCenterAttribute);
			if (_config.ManagerAttribute.Length > 0) typesOnly.Add(_config.ManagerAttribute);
			if (_config.CompanyAttribute.Length > 0) typesOnly.Add(_config.CompanyAttribute);

			var searchFilter = _config.SearchAllFilter;
			var result = _connection.Search(
				_config.SearchBase,
				LdapConnection.ScopeSub,
				searchFilter,
				typesOnly.ToArray(),
				false,
				new LdapSearchConstraints() { MaxResults = 15000 }
			);

			/*
			using (var errorfile = System.IO.File.CreateText("resultAD" + DateTime.Now.Ticks + ".txt"))
			{
				errorfile.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result, Formatting.Indented));

			};
			*/

			employees = new List<LdapEmployee>();

			try
			{
				var user = result.Next();
				while (user != null)
				{
					LdapAttribute firstNameAttr = null;
					LdapAttribute lastNameAttr = null;
					LdapAttribute fullNameAttr = null;
					LdapAttribute emailAttr = null;
					LdapAttribute departmentAttr = null;
					LdapAttribute isActiveAttr = null;
					LdapAttribute internalCodeAttr = null;
					LdapAttribute costCenterAttr = null;
					LdapAttribute managerAttr = null;
					LdapAttribute companyAttr = null;

					var attributes = user.GetAttributeSet();

					if ((_config.FirstNameAttribute.Length > 0) && (attributes.ContainsKey(_config.FirstNameAttribute))) firstNameAttr = user.GetAttribute(_config.FirstNameAttribute);
					if ((_config.LastNameAttribute.Length > 0) && (attributes.ContainsKey(_config.LastNameAttribute))) lastNameAttr = user.GetAttribute(_config.LastNameAttribute);
					if ((_config.FullNameAttribute.Length > 0) && (attributes.ContainsKey(_config.FullNameAttribute))) fullNameAttr = user.GetAttribute(_config.FullNameAttribute);
					if ((_config.EmailAttribute.Length > 0) && (attributes.ContainsKey(_config.EmailAttribute))) emailAttr = user.GetAttribute(_config.EmailAttribute);
					if ((_config.DepartmentAttribute.Length > 0) && (attributes.ContainsKey(_config.DepartmentAttribute))) departmentAttr = user.GetAttribute(_config.DepartmentAttribute);
					if ((_config.IsActiveAttribute.Length > 0) && (attributes.ContainsKey(_config.IsActiveAttribute))) isActiveAttr = user.GetAttribute(_config.IsActiveAttribute);
					if ((_config.InternalCodeAttribute.Length > 0) && (attributes.ContainsKey(_config.InternalCodeAttribute))) internalCodeAttr = user.GetAttribute(_config.InternalCodeAttribute);
					if ((_config.CostCenterAttribute.Length > 0) && (attributes.ContainsKey(_config.CostCenterAttribute))) costCenterAttr = user.GetAttribute(_config.CostCenterAttribute);
					if ((_config.ManagerAttribute.Length > 0) && (attributes.ContainsKey(_config.ManagerAttribute))) managerAttr = user.GetAttribute(_config.ManagerAttribute);
					if ((_config.CompanyAttribute.Length > 0) && (attributes.ContainsKey(_config.CompanyAttribute))) companyAttr = user.GetAttribute(_config.CompanyAttribute);

					employees.Add(new LdapEmployee
					{
						FirstName = firstNameAttr != null ? firstNameAttr.StringValue : string.Empty,
						LastName = lastNameAttr != null ? lastNameAttr.StringValue : string.Empty,
						FullName = fullNameAttr != null ? fullNameAttr.StringValue : string.Empty,
						Email = emailAttr != null ? emailAttr.StringValue : string.Empty,
						Department = departmentAttr != null ? departmentAttr.StringValue : string.Empty,
						IsActive = isActiveAttr != null ? isActiveAttr.StringValue == "Activ" ? false : true : true,
						InternalCode = internalCodeAttr != null ? internalCodeAttr.StringValue : string.Empty,
						CostCenter = costCenterAttr != null ? costCenterAttr.StringValue : string.Empty,
						Manager = managerAttr != null ? managerAttr.StringValue : string.Empty,
						Company = companyAttr != null ? companyAttr.StringValue : string.Empty,
					});

					user = result.Next();
				}
			}
            catch (Exception ex)
            {
                string errorFolderPath = "errors";

                if (!System.IO.Directory.Exists(errorFolderPath))
                {
                    System.IO.Directory.CreateDirectory(errorFolderPath);
                }

                string errorFilePath = System.IO.Path.Combine(errorFolderPath, "error-sync-LDAP--get-employee" + DateTime.Now.Ticks + ".txt");

                using (var errorfile = System.IO.File.CreateText(errorFilePath))
                {
                    errorfile.WriteLine(ex.StackTrace);
                    errorfile.WriteLine(ex.ToString());
                    System.Diagnostics.Debug.WriteLine("Log Exception: " + ex.Message);
                }
            }
            finally
			{
				_connection.Disconnect();
			}
            return employees;
		}

	}
}
