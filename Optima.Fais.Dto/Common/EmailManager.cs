using Optima.Fais.Model;
using System;

namespace Optima.Fais.Dto
{
    public class EmailManager
    {
        public int Id { get; set; }
        public CodeNameEntity EmailType { get; set; }
        public EmployeeResource EmployeeInitial { get; set; }
        public EmployeeResource EmployeeFinal { get; set; }
        public CodeNameEntity RoomInitial { get; set; }
        public CodeNameEntity RoomFinal { get; set; }
        public Order Order { get; set; }
        public Offer Offer { get; set; }
        public CodePartnerEntity Partner { get; set; }
        public CodeNameEntity SubType { get; set; }
        public AssetComponent Asset { get; set; }
        public AssetComponent AssetComponent { get; set; }
        public bool IsAccepted { get; set; }
        // public int? AppStateId { get; set; }

        public virtual AppState State { get; set; }

        public string Info { get; set; }

        public DateTime? CreatedAt { get; set; }
		public DateTime? ModifiedAt { get; set; }
	}
}
