using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Dto
{
    public class AssetBase
    {
        public int Id { get; set; }

        public string InvNo { get; set; }

        public string Name { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public string ERPCode { get; set; }

        public string DocNo2 { get; set; }

        public string DocNo1 { get; set; }

        public string Details { get; set; }

        public string InvName { get; set; }

        public float Quantity { get; set; }

        public string SerialNumber { get; set; }

        public decimal ValueInv { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime ModifiedAt { get; set; }

        public string CreatedBy { get; set; }

        public string SubNo { get; set; }

        public string InfoIni { get; set; }

		public string Info { get; set; }

		public string Manufacturer { get; set; }

        public string AgreementNo { get; set; }

        public string ModelIni { get; set; }

		public bool NotSync { get; set; }

        public string TempReco { get; set; }

        public string TempName { get; set; }

        public string TempSerialNumber { get; set; }

		public decimal ValueInvRon { get; set; }

		public decimal ValueRemRon { get; set; }

		public decimal ValueRem { get; set; }
    }
}
