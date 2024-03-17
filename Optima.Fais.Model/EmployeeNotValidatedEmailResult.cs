using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Model
{
    public class EmployeeNotValidatedEmailResult
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime? NotifyLast { get; set; }
        public int NotifyInterval { get; set; }

    }
}
