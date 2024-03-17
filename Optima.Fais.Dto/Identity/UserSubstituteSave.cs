using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class UserSubstituteSave
    {
        public string userId { get; set; }

        public int employeeId { get; set; }

		public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }
}
