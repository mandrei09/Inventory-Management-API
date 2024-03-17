using System;

namespace Optima.Fais.Dto
{
    public class AssetImportV11
    {
        public string InvNo { get; set; }
        public string SerialNumber { get; set; }
        public string Name { get; set; }
        public float Quantity { get; set; }
        public string AssetType { get; set; }
        public DateTime RetiredDate { get; set; }
        public string TransactionTypeCode { get; set; }
        public string TransactionType { get; set; }
        public float  RetiredQuantity { get; set; }
        public decimal RetiredValue { get; set; }
    }
}
