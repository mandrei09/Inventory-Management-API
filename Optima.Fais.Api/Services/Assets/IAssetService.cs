using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
	public interface IAssetService
	{
		Task<MemoryStream> PreviewAppendixAAsync(int assetId);
	}
}
