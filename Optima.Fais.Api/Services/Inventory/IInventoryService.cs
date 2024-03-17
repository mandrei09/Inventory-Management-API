using Optima.Fais.Model.Utils;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
	public interface IInventoryService
	{
		Task<MemoryStream> PreviewAppendixAAsync(int inventoryId, ReportFilter reportFilter, DateTime? inventoryDateEnd);
		Task<MemoryStream> PreviewAppendixAMinusAsync(int inventoryId, ReportFilter reportFilter, DateTime? inventoryDateEnd);
		Task<MemoryStream> PreviewAppendixAPlusAsync(int inventoryId, ReportFilter reportFilter, DateTime? inventoryDateEnd);
		Task<MemoryStream> AllowLabelAsync(int inventoryId, ReportFilter reportFilter, DateTime? inventoryDateEnd);
		Task<MemoryStream> BookBeforeAsync(int inventoryId, ReportFilter reportFilter, DateTime? inventoryDateStart);
		Task<MemoryStream> BookAfterAsync(int inventoryId, ReportFilter reportFilter, DateTime? inventoryDateEnd);
		Task<MemoryStream> BookPVAsync(int inventoryId, ReportFilter reportFilter, DateTime? inventoryDateStart, DateTime? inventoryDateEnd);
		Task<MemoryStream> BookPVFinalAsync(int inventoryId, ReportFilter reportFilter, DateTime? inventoryDateStart, DateTime? inventoryDateEnd);

	}
}
