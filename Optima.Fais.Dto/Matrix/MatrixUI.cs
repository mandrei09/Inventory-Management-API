using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Dto
{
    public class MatrixUI
    {
		public int Id { get; set; }

		public string Code { get; set; }

        public string Name { get; set; }

        public CodeNameEntity Company { get; set; }

        public CodeNameEntity Project { get; set; }

        public CodeNameEntity CostCenter { get; set; }

        public CodeNameEntity AdmCenter { get; set; }
    }
}
