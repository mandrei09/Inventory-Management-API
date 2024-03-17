using System;

namespace Optima.Fais.Dto
{
    public class AssetOp
    {
        public int Id { get; set; }

        public AssetBase Asset { get; set; }

        public CodeNameEntity AccSystem { get; set; }

        public Document Document { get; set; }

        public CodeNameEntity DocumentType { get; set; }

        public CodeNameEntity State { get; set; }

        public CodeNameEntity AssetType { get; set; }

        public CodeNameEntity AdministrationInitial { get; set; }

        public CodeNameEntity AdministrationFinal { get; set; }

        public CodeNameEntity CostCenterInitial { get; set; }

        public CodeNameEntity CostCenterFinal { get; set; }

        public CodeNameEntity AssetStateInitial { get; set; }

        public CodeNameEntity AssetStateFinal { get; set; }

        public CodeNameEntity DepartmentInitial { get; set; }

        public CodeNameEntity DepartmentFinal { get; set; }

        public decimal? ValueAdd { get; set; }

        public bool? DepUpdate { get; set; }

        public EmployeeResource EmployeeInitial { get; set; }

        public EmployeeResource EmployeeFinal { get; set; }

        public CodeNameEntity RoomInitial { get; set; }

        public CodeNameEntity RoomFinal { get; set; }

        public CodeNameEntity AssetCategoryInitial { get; set; }

        public CodeNameEntity AssetCategoryFinal { get; set; }

        public CodeNameEntity InvStateInitial { get; set; }
        public CodeNameEntity InvStateFinal { get; set; }

        public CodeNameEntity BudgetManagerInitial { get; set; }
        public CodeNameEntity BudgetManagerFinal { get; set; }

        public CodeNameEntity ProjectInitial { get; set; }
        public CodeNameEntity ProjectFinal { get; set; }

        public Dimension DimensionInitial { get; set; }
        public Dimension DimensionFinal { get; set; }

        public CodeNameEntity AssetNatureInitial { get; set; }
        public CodeNameEntity AssetNatureFinal { get; set; }

        public DateTime? ReleaseConfAt { get; set; }
        public EmployeeResource ReleaseConfUser { get; set; }
        public CodeNameEntity ReleaseConfDepartment { get; set; }

        public DateTime? SrcConfAt { get; set; }
        public EmployeeResource SrcConfUser { get; set; }
        public CodeNameEntity SrcConfDepartment { get; set; }

        public DateTime? DstConfAt { get; set; }

        public DateTime? RegisterConfAt { get; set; }
        public EmployeeResource RegisterConfUser { get; set; }
        public CodeNameEntity RegisterConfDepartment { get; set; }

        public CodeNameEntity AssetOpState { get; set; }
        public CodeNameEntity LocationInitial { get; set; }
        public CodeNameEntity LocationFinal { get; set; }
        public CodeNameEntity RegionInitial { get; set; }
        public CodeNameEntity RegionFinal { get; set; }
        public DateTime ModifiedAt { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }


    }
}
