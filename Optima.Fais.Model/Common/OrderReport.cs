using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Optima.Fais.Model.Common
{
    public class OrderReport
    {
        [Key]
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public string InvNo { get; set; }
        public string POTypeName { get; set; }
        public string InvoiceNumber { get; set; }
        public string Supplier { get; set; }
        public string AssetStateName { get; set; }
        public string L4 { get; set; }
        public string L3 { get; set; }
        public string L2 { get; set; }
        public string L1 { get; set; }
        public string S3 { get; set; }
        public string S2 { get; set; }
        public string S1 { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string OwnerEmail { get; set; }
        public string OfferCode { get; set; }
        public string CompanyCode { get; set; }
    }
}
