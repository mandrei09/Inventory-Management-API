using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Optima.Fais.Model
{
    public class EmailOfferStatus : Entity
    {
        public Guid Guid { get; set; }

        public Guid GuidAll { get; set; }

        public int EmailTypeId { get; set; }

        public EmailType EmailType { get; set; }

        public bool IsAccepted { get; set; }

        public int? AppStateId { get; set; }

        public virtual AppState AppState { get; set; }

        public string Message { get; set; }

        public int DocumentNumber { get; set; }

        public bool NotSync { get; set; }

        public int SyncErrorCount { get; set; }

        public int? ErrorId { get; set; }

        public virtual Error Error { get; set; }

		public bool EmailSend { get; set; }

		public bool EmailSkip { get; set; }

		public int? OfferId { get; set; }

		public virtual Offer Offer { get; set; }

		public int? PartnerId { get; set; }

		public virtual Partner Partner { get; set; }

		public int? EmployeeId { get; set; }

		public virtual Employee Employee { get; set; }

		public int? RequestId { get; set; }

		public virtual Request Request { get; set; }

		public int? OwnerId { get; set; }

		public virtual Owner Owner { get; set; }

		public int? CompanyId { get; set; }

		public virtual Company Company { get; set; }

	}
}
