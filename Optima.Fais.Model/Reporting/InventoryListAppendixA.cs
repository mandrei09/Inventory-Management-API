using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Model.Reporting
{
	public class InventoryListAppendixA
	{
		public InventoryListAppendixA()
		{

		}

        public string AssetName { get; set; }
        public string Company { get; set; }
        public string InvNo { get; set; }
        public string ERPCode { get; set; }
        public string SubNo { get; set; }
        public string Uom { get; set; }
        public decimal QInitial { get; set; }
        public decimal QFinal { get; set; }
        public decimal CurrentAPC { get; set; }
        public decimal AccumulDep { get; set; }
		public decimal CurrBkValue { get; set; }
		public string Info { get; set; }
        public int CostCenterIdInitial { get; set; }
        public int CostCenterIdFinal { get; set; }
		public int DivisionIdInitial { get; set; }
		public int DivisionIdFinal { get; set; }
		public int DepartmentIdInitial { get; set; }
		public int DepartmentIdFinal { get; set; }
		public int AdministrationIdInitial { get; set; }
		public int AdministrationIdFinal { get; set; }
		public bool IsTemp { get; set; }
        public DateTime? FirstScan { get; set; }
        public DateTime? LastScan { get; set; }
    }
}
