using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class Asset : AssetBase
    {
        public DocumentMainDetail Document { get; set; }

        public AssetAdmDetail Adm { get; set; }
        public AssetDepDetail Dep { get; set; }
        //public AssetInvDetail Inv { get; set; }
        public AssetInv Inv { get; set; }
        public CodeNameEntity Uom { get; set; }
        public CodeNameEntity Material { get; set; }
        public CodeNameEntity SubCategory { get; set; }
        public CodeNameEntity State { get; set; }
        public InvState InvState { get; set; }
        public CodeNameEntity Company { get; set; }
        public CodeNameEntity DictionaryItem { get; set; }
        public CodePartnerEntity Partner { get; set; }
        public Order Order { get; set; }
        public Dimension Dimension { get; set; }
        public Tax Tax { get; set; }
        public Rate Rate { get; set; }
        public Request Request { get; set; }
        public BudgetForecast BudgetForecast { get; set; }
        public bool Validated { get; set; }

        public bool Custody { get; set; }

        public bool IsInTransfer { get; set; }

        public bool IsReconcile { get; set; }

        public bool IsAccepted { get; set; }

        public string SAPCode { get; set; }

        public DateTime ReceptionDate { get; set; }

        public DateTime PODate { get; set; }

        public DateTime InvoiceDate { get; set; }

        public DateTime RemovalDate { get; set; }

        public bool IsPrinted { get; set; }

        public DateTime PrintDate { get; set; }

        public bool IsDuplicate { get; set; }

		public string HeaderText { get; set; }

        public IEnumerable<Error> ErrorRetire { get; set; }

        public IEnumerable<Error> LastAcquisition { get; set; }

        public EmployeeResource EmployeeTransfer { get; set; }

		public decimal ReceptionPrice { get; set; }

        public bool IsLocked { get; set; }

		public List<InvState> InvStates { get; set; }

        public decimal RateValue { get; set; }

		public string PhoneNumber { get; set; }

		public string Imei { get; set; }

        public ApplicationUser TempUser { get; set; }

		public bool Storno { get; set; }

		public decimal StornoValue { get; set; }

		public int StornoQuantity { get; set; }

		public decimal StornoValueRon { get; set; }

		public ApplicationUser CreatedByUser { get; set; }

		public bool Cassation { get; set; }

		public decimal CassationValue { get; set; }

		public float CassationQuantity { get; set; }

		public decimal CassationValueRon { get; set; }

        public AppState WFHState { get; set; }

        public string InSapValidation { get; set; }

    }
}
