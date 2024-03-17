using System;
using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public class Document : Entity
    {
        public Document()
        {
            ChildDocuments = new HashSet<Document>();
            Operations = new HashSet<AssetOp>();
        }

        public int DocumentTypeId { get; set; }

        public virtual DocumentType DocumentType { get; set; }

        public string DocNo1 { get; set; }

        public string DocNo2 { get; set; }

        public DateTime DocumentDate { get; set; }

        public DateTime RegisterDate { get; set; }

        public DateTime? CreationDate { get; set; }

        //public int? AccMonthId { get; set; }

        //public virtual AccMonth AccMonth { get; set; }

        public int? ParentDocumentId { get; set; }

        public virtual Document ParentDocument { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public int? CostCenterId { get; set; }

        public virtual CostCenter CostCenter { get; set; }

        public int? PartnerId { get; set; }

        public virtual Partner Partner { get; set; }

        public string Details { get; set; }

        public bool? Exported { get; set; }

        public bool? Approved { get; set; }

        public DateTime? ValidationDate { get; set; }

        public virtual ICollection<Document> ChildDocuments { get; set; }

        public virtual ICollection<AssetOp> Operations { get; set; }
    }
}
