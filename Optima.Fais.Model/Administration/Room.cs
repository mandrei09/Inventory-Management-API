using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public partial class Room : Entity
    {
        public Room()
        {
            ChildRooms = new HashSet<Room>();
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public string ERPCode { get; set; }

        public int LocationId { get; set; }

        public virtual Location Location { get; set; }

       

        //public int? CostCenterId { get; set; }

        //public virtual CostCenter CostCenter { get; set; }

        public int? ParentRoomId { get; set; }

        public Room ParentRoom { get; set; }

        public virtual ICollection<Room> ChildRooms { get; set; }

        public bool IsFinished { get; set; }

		public int AssetCount { get; set; }
	}
}
