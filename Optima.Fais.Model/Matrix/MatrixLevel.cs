using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Optima.Fais.Model
{
    public partial class MatrixLevel : Entity
    {
        public MatrixLevel()
        {
        }

        public int MatrixId { get; set; }

        [ForeignKey("MatrixId")]
        public Matrix Matrix { get; set; }

        public int LevelId { get; set; }

        public Level Level { get; set; }

        public int EmployeeId { get; set; }

        public virtual Employee Employee { get; set; }

        public decimal Amount { get; set; }

        public int? UomId { get; set; }

        public virtual Uom Uom { get; set; }

    }
}
