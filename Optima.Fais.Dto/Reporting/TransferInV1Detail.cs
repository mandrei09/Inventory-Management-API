using System;

namespace Optima.Fais.Dto.Reporting
{
    public class TransferInV1Detail
    {
        public string InvNo { get; set; }
        public string Description { get; set; }
        public string SerialNumber { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public string RegionInitial { get; set; }
        public string LocationNameInitial { get; set; }
        public string CostCenterInitial { get; set; }
        public string RoomInitial { get; set; }
        public string InternalCodeInitial { get; set; }
        public string FullNameInitial { get; set; }
        public decimal Value { get; set; }
        public decimal ValueDepTotal { get; set; }
        public float Initial { get; set; }
        public float Actual { get; set; }
        public string Uom { get; set; }
        public string Custody { get; set; }
        public string RegionFinal { get; set; }
        public string LocationNameFinal { get; set; }
        public string CostCenterFinal { get; set; }
        public string RoomFinal { get; set; }
        public string InternalCodeFinal { get; set; }
        public string FullNameFinal { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string LocationHeader { get; set; }
        public string LocationHeaderSecond { get; set; }
        public bool IsDeleted { get; set; }

    }
}