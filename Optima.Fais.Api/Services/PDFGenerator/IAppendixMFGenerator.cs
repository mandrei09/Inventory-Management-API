using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
	public interface IAppendixMFGenerator
	{
		Task<PdfDocumentResult> GenerateDocumentAsync(int assetId, string resourcesPath);
	}
}
