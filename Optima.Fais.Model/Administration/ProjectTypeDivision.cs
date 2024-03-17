using System;
using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public partial class ProjectTypeDivision : Entity
    {
        public ProjectTypeDivision()
        {
        }

        public int ProjectTypeId { get; set; }

        public ProjectType ProjectType { get; set; }

        public int DivisionId { get; set; }

        public Division Division { get; set; }

	}
}
