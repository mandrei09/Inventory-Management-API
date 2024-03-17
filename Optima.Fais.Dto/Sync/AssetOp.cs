using System;

namespace Optima.Fais.Dto.Sync
{
    public class AssetOp
    {
        public int Id { get; set; }
        public int? InventoryId { get; set; }
        //public int InvCompId { get; set; }

        public int AssetId { get; set; }
        public int? ParentInvCompId { get; set; }

        public int? RoomIdInitial { get; set; }
        public int? EmployeeIdInitial { get; set; }
        public int? RoomIdFinal { get; set; }
        public int? EmployeeIdFinal { get; set; }

        public string SNInitial { get; set; }
        public string ProducerInitial { get; set; }
        public string ModelInitial { get; set; }

        public string SNFinal { get; set; }
        public string ProducerFinal { get; set; }
        public string ModelFinal { get; set; }

        public float QuantityInitial { get; set; }
        public float QuantityFinal { get; set; }

        public int? StateIdInitial { get; set; }
        public int? StateIdFinal { get; set; }

        public DateTime ModifiedAt { get; set; }
        public string Info { get; set; }
        public decimal ValueInv { get; set; }

    }
}

