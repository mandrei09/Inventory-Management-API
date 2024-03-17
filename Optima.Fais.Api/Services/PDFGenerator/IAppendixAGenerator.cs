using Optima.Fais.Model.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
	public interface IAppendixAGenerator
	{
		Task<PdfDocumentResult> GenerateDocumentAsync(int inventoryId, ReportFilter reportFilter, DateTime? inventoryDateEnd);
	}
}
