using Optima.Fais.Dto.Common;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Dto
{
    public class RequestDelete
    {
        public int Id { get; set; }
        public string Reason { get; set; }
		public string UserId { get; set; }
	}
}
