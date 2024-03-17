using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class AssetNiInvDet
    {
        public int InventoryId;

        public int Id { get; set; }

        public string Code1 { get; set; }
        public string Code2 { get; set; }
        public string Name1 { get; set; }
        public string Name2 { get; set; }

        public int? AssetId { get; set; }

        public string SerialNumber { get; set; }
        public string Producer { get; set; }
        public string Model { get; set; }

        public float Quantity { get; set; }

        public int? CostCenterId { get; set; }
        public string CostCenterCode { get; set; }
        public string CostCenterName { get; set; }

        public int? AdmCenterId { get; set; }
        public string AdmCenterCode { get; set; }
        public string AdmCenterName { get; set; }
        public int? RegionId { get; set; }
        public string RegionCode { get; set; }
        public string RegionName { get; set; }
        public int? LocationId { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public int? RoomId { get; set; }
        public string RoomCode { get; set; }
        public string RoomName { get; set; }
        public int? EmployeeId { get; set; }
        public string InternalCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Uom { get; set; }

        public int? InvStateId { get; set; }
        public string InvState { get; set; }

        public decimal ValueInv { get; set; }
        public decimal ValueDep { get; set; }

        public bool? Custody { get; set; }

        public bool AllowLabel { get; set; }
        public string Info { get; set; }

        public bool IsDeleted { get; set; }
        public int? CompanyId { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
