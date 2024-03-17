using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto.Reporting
{
    public class InventoryReconciliationDetail
    {
        public string TempInvNo { get; set; }
        public string TempName { get; set; }
        public string InvNo { get; set; }
        public string Description { get; set; }
        public string SerialNumberInitial { get; set; }
        public string SerialNumberFinal { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public decimal ValueInv { get; set; }
        public decimal ValueDep { get; set; }
        public string AdmCenterCodeInitial { get; set; }
        public string AdmCenterNameInitial { get; set; }
        public string CostCenterCodeInitial { get; set; }
        public string CostCenterNameInitial { get; set; }
        public string EmployeeInternalCodeInitial { get; set; }
        public string EmployeeFirstNameInitial { get; set; }
        public string EmployeeLastNameInitial { get; set; }
        public string RoomCodeInitial { get; set; }
        public string RoomNameInitial { get; set; }
        public string LocationCodeInitial { get; set; }
        public string LocationNameInitial { get; set; }
        public string AdmCenterCodeFinal { get; set; }
        public string AdmCenterNameFinal { get; set; }
        public string CostCenterCodeFinal { get; set; }
        public string CostCenterNameFinal { get; set; }
        public string EmployeeInternalCodeFinal { get; set; }
        public string EmployeeFirstNameFinal { get; set; }
        public string EmployeeLastNameFinal { get; set; }
        public string RoomCodeFinal { get; set; }
        public string RoomNameFinal { get; set; }
        public string LocationCodeFinal { get; set; }
        public string LocationNameFinal { get; set; }
    }
}
