using Optima.Fais.Model;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Dto
{
    public class EmailReadResponse
    {
		public string Subject { get; set; }
		public List<string> From { get; set; }
		public List<string> To { get; set; }
		public List<string> Cc { get; set; }
		public DateTime SendDate { get; set; }
		public string TextBody { get; set; }
	}
}
