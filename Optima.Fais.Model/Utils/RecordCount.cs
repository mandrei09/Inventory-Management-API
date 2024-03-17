using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Model
{
    public class RecordCount
    {
        [Key]
        public int Count { get; set; }
    }
}
