using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Optima.Fais.Model
{
    public class Error : Entity
    {
      
        public string Code { get; set; }

        public string Name { get; set; }

        public int? AssetId { get; set; }

        public virtual Asset Asset { get; set; }

        public int? ErrorTypeId { get; set; }

        public ErrorType ErrorType { get; set; }

        [MaxLength(450)]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public string BUKRS { get; set; }

        public string BELNR { get; set; }

        public string GJAHR { get; set; }

        public string Request { get; set; }
    }
}
