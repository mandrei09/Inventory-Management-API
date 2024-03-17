using System;

namespace Optima.Fais.Dto
{
    public class ContractOp
    {
        public int Id { get; set; }

        public Contract Contract { get; set; }

        public CodeNameEntity CompanyInitial { get; set; }

        public CodeNameEntity CompanyFinal { get; set; }

        public CodeNameEntity DivisionInitial { get; set; }

        public CodeNameEntity DivisionFinal { get; set; }

        public EmployeeResource EmployeeInitial { get; set; }

        public EmployeeResource EmployeeFinal { get; set; }

        public MonthEntity AccMonth { get; set; }

        public CodePartnerEntity PartnerInitial { get; set; }

        public CodePartnerEntity PartnerFinal { get; set; }

        public decimal ValueIni1 { get; set; }

        public decimal ValueIni2 { get; set; }

        public decimal ValueFin1 { get; set; }

        public decimal ValueFin2 { get; set; }

        public decimal QuantityIni { get; set; }

        public decimal QuantityFin { get; set; }

		public string InfoIni { get; set; }

		public string InfoFin { get; set; }

		//public DateTime? SrcConfAt { get; set; }

		//public EmployeeResource SrcConfUser { get; set; }

        public DateTime? DstConfAt { get; set; }

        public EmployeeResource DstConfUser { get; set; }

        public CodeNameEntity State { get; set; }

		public Document Document { get; set; }

		public CodeNameEntity DocumentType { get; set; }

		public DateTime ModifiedAt { get; set; }


	}
}
