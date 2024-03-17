using Microsoft.Extensions.Configuration;
using MigraDoc.Rendering;
using Optima.Fais.Repository;
using System.IO;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{
	public class AssetService : IAssetService
	{
		private IAppendixMFGenerator _appendixMFGenerator = null;
		private readonly string _basePath = string.Empty;

		public AssetService(IConfiguration configuration, IAppendixMFGenerator appendixMFGenerator)
		{
			this._appendixMFGenerator = appendixMFGenerator;
			this._basePath = configuration.GetSection("BasePath").GetValue<string>("Base");
		}

		public async Task<MemoryStream> PreviewAppendixAAsync(int assetId)
		{
			var document = await this._appendixMFGenerator.GenerateDocumentAsync(assetId, string.Empty);

			PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);
			pdfRenderer.Document = document.Document;
			pdfRenderer.RenderDocument();

			MemoryStream ms = new MemoryStream();
			pdfRenderer.PdfDocument.Save(ms, false);
			byte[] buffer = new byte[ms.Length];
			ms.Seek(0, SeekOrigin.Begin);
			ms.Flush();
			ms.Read(buffer, 0, (int)ms.Length);
			return ms;
		}
	}
}
