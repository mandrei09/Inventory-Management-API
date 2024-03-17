using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
    public class DashboardView : AssetBase
    {
        public DocumentMainDetail Document { get; set; }

        public AssetAdmDetail Adm { get; set; }
        public AssetDepDetail Dep { get; set; }
        //public AssetInvDetail Inv { get; set; }
        public AssetInv Inv { get; set; }
        public CodeNameEntity Uom { get; set; }
        public CodeNameEntity Material { get; set; }
        public CodeNameEntity InvState { get; set; }
        public CodeNameEntity Company { get; set; }
        public CodeNameEntity DictionaryItem { get; set; }
        public CodePartnerEntity Partner { get; set; }
        public CodeNameEntity Order { get; set; }
        public Dimension Dimension { get; set; }
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
    }
}
