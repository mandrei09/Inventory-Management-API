using System;

namespace Optima.Fais.Dto.Sync
{
    public class Asset
    {
        public int Id { get; set; }
        public string InvNo { get; set; }
        public string Name { get; set; }

        public int? RoomId { get; set; }
        public int? EmployeeId { get; set; }

        public string SerialNumber { get; set; }
        public string Producer { get; set; }
        public string Model { get; set; }

        public float Quantity { get; set; }
        public float ValueInv { get; set; }

        public int? InvStateId { get; set; }
        public int? CompanyId { get; set; }
        public bool? IsDeleted { get; set; }

        public bool? AllowLabel { get; set; }
        public string Info { get; set; }
        public string InvName { get; set; }
        public string Barcode { get; set; }
        public DateTime? PurchaseDate { get; set; }

        public DateTime ModifiedAt { get; set; }
    }
}
