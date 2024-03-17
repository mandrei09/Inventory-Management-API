using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Model
{
    public class EmployeeEmailResult
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime? NotifyLast { get; set; }
        public int NotifyInterval { get; set; }
        public DateTime? NotifyStart { get; set; }
        public DateTime? NotifyEnd { get; set; }
		public int NotifyEnabled { get; set; }

	}
}
