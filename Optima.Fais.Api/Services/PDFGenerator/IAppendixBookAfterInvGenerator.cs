using Optima.Fais.Model.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
	public interface IAppendixBookAfterInvGenerator
    {
		Task<PdfDocumentResult> GenerateDocumentAsync(int inventoryId, ReportFilter reportFilter, DateTime? inventoryDateEnd);
	}
}
