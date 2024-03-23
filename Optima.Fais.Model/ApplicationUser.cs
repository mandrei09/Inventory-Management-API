using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public class ApplicationUser : IdentityUser
    {
        //public ApplicationUser()
        //{
        //    HistoryUsers = new HashSet<HistoryUser>();
        //}

        /// <summary>
        /// Navigation property for the roles this user belongs to.
        /// </summary>
        public virtual ICollection<IdentityUserRole<string>> Roles { get; } = new List<IdentityUserRole<string>>();

        /// <summary>
        /// Navigation property for the claims this user possesses.
        /// </summary>
        public virtual ICollection<IdentityUserClaim<string>> Claims { get; } = new List<IdentityUserClaim<string>>();

        /// <summary>
        /// Navigation property for this users login accounts.
        /// </summary>
        public virtual ICollection<IdentityUserLogin<string>> Logins { get; } = new List<IdentityUserLogin<string>>();

        /// <summary>Given name(s) or first name(s) of the End-User.</summary>
        public virtual string GivenName { get; set; }
        /// <summary>Surname(s) or last name(s) of the End-User.</summary>
        public virtual string FamilyName { get; set; }

        public int? AdmCenterId { get; set; }

        public AdmCenter AdmCenter { get; set; }

        public int? EmployeeId { get; set; }

        public Employee Employee { get; set; }

        public int? SubstituteId { get; set; }

        public Employee Substitute { get; set; }

		public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

		public bool InInventory { get; set; }

		public DateTime? LastLogin { get; set; }

		public int? MobilePhoneId { get; set; }

		public MobilePhone MobilePhone { get; set; }

        //public ICollection<Message> MessagesSent { get; set; }

        //public ICollection<Message> MessagesReceived { get; set; }

        //public DateTime? LastActivity { get; set; }

        //public string LastPage { get; set; }

        //public virtual ICollection<HistoryUser> HistoryUsers { get; set; }

        //public int? AdmCenterArchiveId { get; set; }

        //public AdmCenter AdmCenterArchive { get; set; }
    }
}
