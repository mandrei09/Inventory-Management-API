using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto.Reporting
{
    public class InventoryListTotal
    {
        public string InvNo { get; set; }
        public string Description { get; set; }
        public string SerialNumber { get; set; }
        public string CostCenterCodeInitial { get; set; }
        public string CostCenterNameInitial { get; set; }
        public string AdmCenterCodeInitial { get; set; }
        public string AdmCenterNameInitial { get; set; }
        public string LocationCodeInitial { get; set; }
        public string LocationNameInitial { get; set; }
        public string RoomCodeInitial { get; set; }
        public string RoomNameInitial { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public decimal ValueInv { get; set; }
        public decimal ValueDep { get; set; }
        public string UomName { get; set; }
        public decimal QInitial { get; set; }
        public bool Custody { get; set; }
        public string InternalCodeInitial { get; set; }
        public string FirstNameInitial { get; set; }
        public string LastNameInitial { get; set; }
        public string CostCenterCodeFinal { get; set; }
        public string CostCenterNameFinal { get; set; }
        public string AdmCenterCodeFinal { get; set; }
        public string AdmCenterNameFinal { get; set; }
        public string LocationCodeFinal { get; set; }
        public string LocationNameFinal { get; set; }
        public string RoomCodeFinal { get; set; }
        public string RoomNameFinal { get; set; }
        public string InternalCodeFinal { get; set; }
        public string FirstNameFinal { get; set; }
        public string LastNameFinal { get; set; }
        public decimal QFinal { get; set; }
        public string InventoryName { get; set; }

        public List<InventoryListTotalDetail> InventoryListTotalDetail;

        public List<InventoryListTotalDetail> ListInventoryTotal()
        {
            return InventoryListTotalDetail;
        }
    }
}
