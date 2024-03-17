using System;
using System.ComponentModel.DataAnnotations;

namespace Optima.Fais.Model
{
    public class RequestOp : Entity
    {
        public int RequestId { get; set; }

        public virtual Request Request { get; set; }

        public int? AccSystemId { get; set; }

        public virtual AccSystem AccSystem { get; set; }

        public int DocumentId { get; set; }

        public virtual Document Document { get; set; }

        public int? AccMonthId { get; set; }

        public virtual AccMonth AccMonth { get; set; }

        public int? BudgetManagerId { get; set; }

        public virtual BudgetManager BudgetManager { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public int? CostCenterIdInitial { get; set; }

        public virtual CostCenter CostCenterInitial { get; set; }

        public int? CostCenterIdFinal { get; set; }

        public virtual CostCenter CostCenterFinal { get; set; }

        public int? EmployeeIdInitial { get; set; }

        public virtual Employee EmployeeInitial { get; set; }

        public int? EmployeeIdFinal { get; set; }

        public virtual Employee EmployeeFinal { get; set; }

        public int? ProjectIdInitial { get; set; }

        public virtual Project ProjectInitial { get; set; }

        public int? ProjectIdFinal { get; set; }

        public virtual Project ProjectFinal { get; set; }


        [MaxLength(450)]
        public string InfoIni { get; set; }

        [MaxLength(450)]
        public string InfoFin { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? ReleaseConfAt { get; set; }

        [MaxLength(450)]
        public string ReleaseConfBy { get; set; }

        public ApplicationUser ReleaseConfUser { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? SrcConfAt { get; set; }

        [MaxLength(450)]
        public string SrcConfBy { get; set; }

        public ApplicationUser SrcConfUser { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? DstConfAt { get; set; }

        [MaxLength(450)]
        public string DstConfBy { get; set; }

        public ApplicationUser DstConfUser { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? RegisterConfAt { get; set; }

        [MaxLength(450)]
        public string RegisterConfBy { get; set; }

        public ApplicationUser RegisterConfUser { get; set; }

        public int? RequestStateId { get; set; }

        public virtual AppState RequestState { get; set; }

		public bool Validated { get; set; }

        public bool IsAccepted { get; set; }

        public Guid Guid { get; set; }

        public int? BudgetIdInitial { get; set; }

        public virtual Budget BudgetInitial { get; set; }

        public int? BudgetIdFinal { get; set; }

        public virtual Budget BudgetFinal { get; set; }

    }
}
