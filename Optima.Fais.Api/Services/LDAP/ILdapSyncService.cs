using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services.LDAP
{
	public interface ILdapSyncService
	{
		Task<int> SyncEmployeesAsync();
	}
}
