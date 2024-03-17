using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class OfferUpload
	{
		public int PartnerId { get; set; }
		public IFormFile[] Files { get; set; }
	}
}
