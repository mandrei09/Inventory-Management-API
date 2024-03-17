using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class AddAssetUpload
	{
		public IFormFile[] Files { get; set; }
	}
}
