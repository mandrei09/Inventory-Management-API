using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Optima.Fais.Model
{
    public class EmailStatus : Entity
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

        public int? CostCenterIdInitial { get; set; }

        public virtual CostCenter CostCenterInitial { get; set; }

        public int? CostCenterIdFinal { get; set; }

        public virtual CostCenter CostCenterFinal { get; set; }

        public int? AssetId { get; set; }

        public virtual Asset Asset { get; set; }

        public bool IsAccepted { get; set; }

        public int? AppStateId { get; set; }

        public virtual AppState AppState { get; set; }

        public string Info { get; set; }

        public int? OfferId { get; set; }

        public virtual Offer Offer { get; set; }

        public int? OrderId { get; set; }

        public virtual Order Order { get; set; }

        public int? BudgetBaseId { get; set; }

        public virtual BudgetBase BudgetBase { get; set; }

        public int? PartnerId { get; set; }

        public virtual Partner Partner { get; set; }

        public int? RequestId { get; set; }

        public virtual Request Request { get; set; }

        public int? StockId { get; set; }

        public virtual Stock Stock { get; set; }

        public int DocumentNumber { get; set; }

        public bool NotSync { get; set; }

        public int SyncErrorCount { get; set; }

        public int? ErrorId { get; set; }

        public virtual Error Error { get; set; }

        public int? AssetOpId { get; set; }

        public virtual AssetOp AssetOp { get; set; }

		[DataType(DataType.DateTime)]
		public DateTime? SrcEmployeeValidateAt { get; set; }

		[MaxLength(450)]
		public string SrcEmployeeValidateBy { get; set; }

		public ApplicationUser SrcEmployeeValidateUser { get; set; }


		[DataType(DataType.DateTime)]
		public DateTime? SrcManagerValidateAt { get; set; }

		[MaxLength(450)]
		public string SrcManagerValidateBy { get; set; }

		public ApplicationUser SrcManagerValidateUser { get; set; }

		[DataType(DataType.DateTime)]
		public DateTime? DstEmployeeValidateAt { get; set; }

		[MaxLength(450)]
		public string DstEmployeeValidateBy { get; set; }

		public ApplicationUser DstEmployeeValidateUser { get; set; }


		[DataType(DataType.DateTime)]
		public DateTime? DstManagerValidateAt { get; set; }

		[MaxLength(450)]
		public string DstManagerValidateBy { get; set; }

		public ApplicationUser DstManagerValidateUser { get; set; }


		[DataType(DataType.DateTime)]
		public DateTime? FinalValidateAt { get; set; }

		[MaxLength(450)]
		public string FinalValidateBy { get; set; }

		public ApplicationUser FinalValidateUser { get; set; }

		public bool NotSrcEmployeeSync { get; set; }

		public int SyncSrcEmployeeErrorCount { get; set; }

		public bool NotSrcManagerSync { get; set; }

		public int SyncSrcManagerErrorCount { get; set; }

		public bool NotDstEmployeeSync { get; set; }

		public int SyncDstEmployeeErrorCount { get; set; }

		public bool NotDstManagerSync { get; set; }

		public int SyncDstManagerErrorCount { get; set; }

		public bool SameEmployee { get; set; }

		public bool SameManager { get; set; }

		public bool NotCompletedSync { get; set; }

		public int SyncCompletedErrorCount { get; set; }

		public bool Completed { get; set; }

		public bool Exported { get; set; }

		public bool EmailSend { get; set; }

		public bool SrcEmployeeEmailSend { get; set; }

		public bool SrcManagerEmailSend { get; set; }

		public bool DstEmployeeEmailSend { get; set; }

		public bool DstManagerEmailSend { get; set; }

		public bool SkipSrcEmployee { get; set; }

		public bool SkipSrcManager { get; set; }

		public bool SkipDstEmployee { get; set; }

		public bool SkipDstManager { get; set; }

		public bool Skip { get; set; }

		[DataType(DataType.DateTime)]
		public DateTime? DstEmployeeReminder1At { get; set; }

		[DataType(DataType.DateTime)]
		public DateTime? DstEmployeeReminder2At { get; set; }

		[DataType(DataType.DateTime)]
		public DateTime? DstEmployeeReminder3At { get; set; }

		[DataType(DataType.DateTime)]
		public DateTime? DstEmployeeReminder4At { get; set; }

		public bool NotDstEmployeeReminder1Sync { get; set; }

		public bool NotDstEmployeeReminder2Sync { get; set; }

		public bool NotDstEmployeeReminder3Sync { get; set; }

		public bool NotDstEmployeeReminder4Sync { get; set; }

		public bool DstEmployeeReminder1EmailSend { get; set; }

		public bool DstEmployeeReminder2EmailSend { get; set; }

		public bool DstEmployeeReminder3EmailSend { get; set; }

		public bool DstEmployeeReminder4EmailSend { get; set; }

		public int SyncDstEmployeeReminder1ErrorCount { get; set; }

		public int SyncDstEmployeeReminder2ErrorCount { get; set; }

		public int SyncDstEmployeeReminder3ErrorCount { get; set; }

		public int SyncDstEmployeeReminder4ErrorCount { get; set; }

		public int ReminderDays { get; set; }

		public int GenerateBookErrorCount { get; set; }
	}
}
