using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
	public interface IAppendix1Generator
	{
		Task<PdfDocumentResult> GenerateDocumentAsync(int assetId, string resourcesPath);
	}
}
