using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Optima.Fais.Model
{
    public class AssetComponent : Entity
    {

        public AssetComponent()
        {
            entityFiles = new List<string>();
        }
        public string Code { get; set; }

        public string Name { get; set; }

        public string Prefix { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public int? EmployeeId { get; set; }

        public virtual Employee Employee { get; set; }

        public int? AssetId { get; set; }

        public virtual Asset Asset { get; set; }

        public bool IsAccepted { get; set; }

        public int? SubTypeId { get; set; }

        public virtual SubType SubType { get; set; }

        public int? AppStateId { get; set; }

        public virtual AppState AppState { get; set; }

        public string Info { get; set; }

        public int? InvStateId { get; set; }

        public virtual InvState InvState { get; set; }

        [NotMapped]
        public List<string> entityFiles { get; set; }

    }
}
