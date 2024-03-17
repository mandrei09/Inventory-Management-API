using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public class ApplicationUserLogin : IdentityUserLogin<string>
	{
		public DateTime LoginDate { get; set; } = DateTime.Now;

		//public ApplicationUser ApplicationUser { get; set; }
	}
}
