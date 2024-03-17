using System;
using System.Collections.Generic;

namespace Optima.Fais.Dto
{
    public class InventoryAsset
    {
        public int Id { get; set; }

        public string InvNo { get; set; }
        public string Name { get; set; }
        public DateTime? PurchaseDate { get; set; }

        public int? CostCenterIdIni { get; set; }
        public string CostCenterCodeIni { get; set; }
        public string CostCenterNameIni { get; set; }
        public int? CostCenterIdFin { get; set; }
        public string CostCenterCodeFin { get; set; }
        public string CostCenterNameFin { get; set; }

        public int? AdmCenterIdIni { get; set; }
        public string AdmCenterCodeIni { get; set; }
        public string AdmCenterNameIni { get; set; }
        public int? RegionIdIni { get; set; }
        public string RegionCodeIni { get; set; }
        public string RegionNameIni { get; set; }
        public int? LocationIdIni { get; set; }
        public string LocationCodeIni { get; set; }
        public string LocationNameIni { get; set; }
        public int? RoomIdIni { get; set; }
        public string RoomCodeIni { get; set; }
        public string RoomNameIni { get; set; }
        public int? EmployeeIdIni { get; set; }
        public string InternalCodeIni { get; set; }
        public string FirstNameIni { get; set; }
        public string LastNameIni { get; set; }

        public int? AdmCenterIdFin { get; set; }
        public string AdmCenterCodeFin { get; set; }
        public string AdmCenterNameFin { get; set; }
        public int? RegionIdFin { get; set; }
        public string RegionCodeFin { get; set; }
        public string RegionNameFin { get; set; }
        public int? LocationIdFin { get; set; }
        public string LocationCodeFin { get; set; }
        public string LocationNameFin { get; set; }
        public int? RoomIdFin { get; set; }
        public string RoomCodeFin { get; set; }
        public string RoomNameFin { get; set; }
        public int? EmployeeIdFin { get; set; }
        public string InternalCodeFin { get; set; }
        public string FirstNameFin { get; set; }
        public string LastNameFin { get; set; }

        public string SerialNumber { get; set; }
        public string Producer { get; set; }
        public string Model { get; set; }

        public float QIntial { get; set; }
        public float QFinal { get; set; }
        public string Uom { get; set; }

        public int? InvStateIdIni { get; set; }
        public string InvStateIni { get; set; }
        public int? InvStateIdFin { get; set; }
        public string InvStateFin { get; set; }
        public int? InvDetailStateId { get; set; }
        public string InvDetailState { get; set; }

        public decimal ValueInv { get; set; }
        public decimal ValueDep { get; set; }

        public bool? Custody { get; set; }
    }
}
