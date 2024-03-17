using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services.SAPAriba
{
	public interface IContractImportService
	{
		Task<int> ContractImportAsync();
	}
}
