using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Optima.Fais.Data;
using System.IO;

namespace Optima.Fais.Api.Controllers.Common
{
	[Authorize]
	[Route("api/Version")]
	[Produces("application/xml", "application/vnd.android.package-archive")]
	public class VersionController : Controller
	{
		private readonly ApplicationDbContext context = null;

		public VersionController(ApplicationDbContext context)
		{
			this.context = context;
		}

		[AllowAnonymous]
		[HttpGet]
		[Route("", Order = -1)]
		public Stream Get()
		{
			return new FileStream(@"version\update.xml", FileMode.Open);
		}

		[AllowAnonymous]
		[HttpGet("apk")]
		[Produces("application/vnd.android.package-archive")]
		[Route("", Order = -1)]
		public Stream GetApk()
		{
			var version = "10300";
			return new FileStream(@"apk\emag_" + version + ".apk", FileMode.Open);
		}

	}
}
