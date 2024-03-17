using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services.BNR
{
	public interface IBNRRatesImportService
	{
		Task<int> BNRRatesImportAsync();
	}
}
