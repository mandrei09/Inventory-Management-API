using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Optima.Fais.Model
{
    public class EmailRequestStatus : Entity
    {
        public Guid Guid { get; set; }

        public Guid GuidAll { get; set; }

        public int EmailTypeId { get; set; }

        public EmailType EmailType { get; set; }

        public int RequestId { get; set; }

        public virtual Request Request { get; set; }

        public bool IsAccepted { get; set; }

        public int? AppStateId { get; set; }

        public virtual AppState AppState { get; set; }

        public string Info { get; set; }

        public int DocumentNumber { get; set; }

        public bool NotSync { get; set; }

        public int SyncErrorCount { get; set; }

        public int? ErrorId { get; set; }

        public virtual Error Error { get; set; }

		[DataType(DataType.DateTime)]
		public DateTime? EmployeeL1ValidateAt { get; set; }

		[MaxLength(450)]
		public string EmployeeL1ValidateBy { get; set; }

		public ApplicationUser EmployeeL1ValidateUser { get; set; }


		[DataType(DataType.DateTime)]
		public DateTime? EmployeeL2ValidateAt { get; set; }

		[MaxLength(450)]
		public string EmployeeL2ValidateBy { get; set; }

		public ApplicationUser EmployeeL2ValidateUser { get; set; }

		[DataType(DataType.DateTime)]
		public DateTime? EmployeeL3ValidateAt { get; set; }

		[MaxLength(450)]
		public string EmployeeL3ValidateBy { get; set; }

		public ApplicationUser EmployeeL3ValidateUser { get; set; }


		[DataType(DataType.DateTime)]
		public DateTime? EmployeeL4ValidateAt { get; set; }

		[MaxLength(450)]
		public string EmployeeL4ValidateBy { get; set; }

		public ApplicationUser EmployeeL4ValidateUser { get; set; }

		[DataType(DataType.DateTime)]
		public DateTime? EmployeeS1ValidateAt { get; set; }

		[MaxLength(450)]
		public string EmployeeS1ValidateBy { get; set; }

		public ApplicationUser EmployeeS1ValidateUser { get; set; }

		[DataType(DataType.DateTime)]
		public DateTime? EmployeeS2ValidateAt { get; set; }

		[MaxLength(450)]
		public string EmployeeS2ValidateBy { get; set; }

		public ApplicationUser EmployeeS2ValidateUser { get; set; }

		[DataType(DataType.DateTime)]
		public DateTime? EmployeeS3ValidateAt { get; set; }

		[MaxLength(450)]
		public string EmployeeS3ValidateBy { get; set; }

		public ApplicationUser EmployeeS3ValidateUser { get; set; }


		[DataType(DataType.DateTime)]
		public DateTime? FinalValidateAt { get; set; }

		[MaxLength(450)]
		public string FinalValidateBy { get; set; }

		public ApplicationUser FinalValidateUser { get; set; }

		public bool NotEmployeeL1Sync { get; set; }

		public int SyncEmployeeL1ErrorCount { get; set; }

		public bool NotEmployeeL2Sync { get; set; }

		public int SyncEmployeeL2ErrorCount { get; set; }

		public bool NotEmployeeL3Sync { get; set; }

		public int SyncEmployeeL3ErrorCount { get; set; }

		public bool NotEmployeeL4Sync { get; set; }

		public int SyncEmployeeL4ErrorCount { get; set; }

		public bool NotEmployeeS1Sync { get; set; }

		public int SyncEmployeeS1ErrorCount { get; set; }

		public bool NotEmployeeS2Sync { get; set; }

		public int SyncEmployeeS2ErrorCount { get; set; }

		public bool NotEmployeeS3Sync { get; set; }

		public int SyncEmployeeS3ErrorCount { get; set; }

		public bool NotCompletedSync { get; set; }

		public int SyncCompletedErrorCount { get; set; }

		public bool Completed { get; set; }

		public bool Exported { get; set; }

		public bool EmailSend { get; set; }

		public bool EmployeeL1EmailSend { get; set; }

		public bool EmployeeL2EmailSend { get; set; }

		public bool EmployeeL3EmailSend { get; set; }

		public bool EmployeeL4EmailSend { get; set; }

		public bool EmployeeS1EmailSend { get; set; }

		public bool EmployeeS2EmailSend { get; set; }

		public bool EmployeeS3EmailSend { get; set; }

		public bool EmployeeL1EmailSkip { get; set; }

		public bool EmployeeL2EmailSkip { get; set; }

		public bool EmployeeL3EmailSkip { get; set; }

		public bool EmployeeL4EmailSkip { get; set; }

		public bool EmployeeS1EmailSkip { get; set; }

		public bool EmployeeS2EmailSkip { get; set; }

		public bool EmployeeS3EmailSkip { get; set; }

		public int? RequestBudgetForecastId { get; set; }

		public virtual RequestBudgetForecast RequestBudgetForecast { get; set; }

		public bool NotNeedBudgetSync { get; set; }

		public int SyncNeedBudgetErrorCount { get; set; }

		public bool NeedBudgetEmailSend { get; set; }
	}
}
