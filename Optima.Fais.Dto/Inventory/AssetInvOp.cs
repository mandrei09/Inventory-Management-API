using System;

namespace Optima.Fais.Dto
{
    public class AssetInvOp
    {
        public int? Id { get; set; }
        public int? InventoryId { get; set; }
        public int AssetId { get; set; }

        public int? RoomIdInitial { get; set; }
        public int? RoomIdFinal { get; set; }
        public int? EmployeeIdInitial { get; set; }
        public int? EmployeeIdFinal { get; set; }

        public string SNFinal { get; set; }
        public string ProducerFinal { get; set; }
        public string ModelFinal { get; set; }

        public float QInitial { get; set; }
        public float QFinal { get; set; }

        public int? StateIdInitial { get; set; }
        public int? StateIdFinal { get; set; }

        public bool? AllowLabel { get; set; }
        public bool IsDeleted { get; set; }

        public int? DetailStateId { get; set; }

        public string Info { get; set; }

        public DateTime ModifiedAt { get; set; }
    }
}
