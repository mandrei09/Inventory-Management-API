using System;

namespace Optima.Fais.Dto
{
    public class AssetImportSocgen
    {
        public string Info { get; set; }
        public string InvNo { get; set; }
        public string ERPCode { get; set; }
        public string SerialNumber { get; set; }
        public string AssetType { get; set; }
        public string MasterType { get; set; }
        public string Type { get; set; }
        public string SubType { get; set; }
        public string InsuranceCategory { get; set; }
        public string Description { get; set; }
        public string Model { get; set; }
        public string Brand { get; set; }
        public string AssetState { get; set; }
        public string InvState { get; set; }
        public string Partner { get; set; }
        public DateTime? ReceptionDate { get; set; }
        public DateTime? PODate { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? RemovalDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string PONumber { get; set; }
        public decimal ValueInv { get; set; }
        public string AccSystem { get; set; }
        public decimal ValueDep { get; set; }
        public decimal ValueDepPU { get; set; }
        public int DepPeriodMonth { get; set; }
        public string InternalCode { get; set; }
        public string EmployeeLastName { get; set; }
        public string EmployeeFirstName { get; set; }
        public string Region { get; set; }
        public string Location { get; set; }
        public string Room { get; set; }
        public string EmployeeTeamLeader { get; set; }
        public string EmployeeStatus { get; set; }
        public string CostCenter { get; set; }
        public string AssetNature { get; set; }
        public string BudgetManager { get; set; }
        public string RunChange { get; set; }
        public string Project { get; set; }
        public string Client { get; set; }
        public string Uom { get; set; }
        public string AssetCategory { get; set; }
    }
}
