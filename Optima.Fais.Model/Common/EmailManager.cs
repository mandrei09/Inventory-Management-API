using System;
using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public class EmailManager : Entity
    {
        public Guid Guid { get; set; }

        public Guid GuidAll { get; set; }

        public int EmailTypeId { get; set; }

        public EmailType EmailType { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public int? EmployeeIdInitial { get; set; }

        public virtual Employee EmployeeInitial { get; set; }

        public int? EmployeeIdFinal { get; set; }

        public virtual Employee EmployeeFinal { get; set; }

        public int? RoomIdInitial { get; set; }

        public virtual Room RoomInitial { get; set; }

        public int? RoomIdFinal { get; set; }

        public virtual Room RoomFinal { get; set; }

        public int? AssetId { get; set; }

        public virtual Asset Asset { get; set; }

        public int? AssetComponentId { get; set; }

        public virtual AssetComponent AssetComponent { get; set; }

        public bool IsAccepted { get; set; }

        public int? AppStateId { get; set; }

        public virtual AppState AppState { get; set; }

        public string Info { get; set; }

        public int? SubTypeId { get; set; }

        public virtual SubType SubType { get; set; }

        public int? OfferId { get; set; }

        public virtual Offer Offer { get; set; }

        public int? OrderId { get; set; }

        public virtual Order Order { get; set; }

        public int? BudgetId { get; set; }

        public virtual Budget Budget { get; set; }

        public int? PartnerId { get; set; }

        public virtual Partner Partner { get; set; }


    }
}
