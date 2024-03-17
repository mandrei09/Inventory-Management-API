namespace Optima.Fais.Model
{
    public class AssetNi : Entity
    {
        public string Code1 { get; set; }
        public string Code2 { get; set; }
        public string Name1 { get; set; }
        public string Name2 { get; set; }

        public int? AssetId { get; set; }
        public virtual Asset Asset { get; set; }

        public int? RoomId { get; set; }
        public virtual Room Room { get; set; }

        public int? EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        public string SerialNumber { get; set; }
        public string Producer { get; set; }
        public string Model { get; set; }

        public float Quantity { get; set; }

        public int? InvStateId { get; set; }
        public virtual InvState InvState { get; set; }

        public int? CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public bool AllowLabel { get; set; }
        public string Info { get; set; }

        public int InventoryId { get; set; }
        public virtual Inventory Inventory { get; set; }

        public int? CostCenterId { get; set; }
        public virtual CostCenter CostCenter { get; set; }
    }
}
