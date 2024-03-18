using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Optima.Fais.Model
{
    public partial class Matrix : Entity
    {
        public Matrix()
        {
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public int? CostCenterId { get; set; }

        public virtual CostCenter CostCenter { get; set; }

        public int AppStateId { get; set; }

        public virtual AppState AppState { get; set; }

        public int? CountryId { get; set; }

        public virtual Country Country { get; set; }

        public int? AssetTypeId { get; set; }

        public virtual AssetType AssetType { get; set; }

        public int? ProjectId { get; set; }

        public virtual Project Project { get; set; }

        public virtual ICollection<MatrixLevel> MatrixLevels { get; set; }

        public int? EmployeeL1Id { get; set; }

        public virtual Employee EmployeeL1 { get; set; }

        public int? EmployeeL2Id { get; set; }

        public virtual Employee EmployeeL2 { get; set; }

        public int? EmployeeL3Id { get; set; }

        public virtual Employee EmployeeL3 { get; set; }

        public int? EmployeeL4Id { get; set; }

        public virtual Employee EmployeeL4 { get; set; }

        public int? EmployeeS1Id { get; set; }

        public virtual Employee EmployeeS1 { get; set; }

        public int? EmployeeS2Id { get; set; }

        public virtual Employee EmployeeS2 { get; set; }

        public int? EmployeeS3Id { get; set; }

        public virtual Employee EmployeeS3 { get; set; }

        public decimal AmountL1 { get; set; }

        public decimal AmountL2 { get; set; }

        public decimal AmountL3 { get; set; }

        public decimal AmountL4 { get; set; }

        public decimal AmountS1 { get; set; }

        public decimal AmountS2 { get; set; }

        public decimal AmountS3 { get; set; }

        public int? ProjectTypeId { get; set; }

        public virtual ProjectType ProjectType { get; set; }

        public int? DivisionId { get; set; }

        public virtual Division Division { get; set; }

		public int? EmployeeB1Id { get; set; }

		public virtual Employee EmployeeB1 { get; set; }

        public int PriorityL4 { get; set; }

		public int PriorityL3 { get; set; }

		public int PriorityL2 { get; set; }

		public int PriorityL1 { get; set; }

		public int PriorityS3 { get; set; }

		public int PriorityS2 { get; set; }

		public int PriorityS1 { get; set; }
	}
}
