using System;
using System.ComponentModel.DataAnnotations;

namespace Optima.Fais.Model
{
    public abstract class Entity : IEntity
    {
        //[Key]
        public int Id { get; set; }

        //private DateTime? createdAt;

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }
        //public DateTime CreatedAt
        //{
        //    get { return createdAt ?? DateTime.UtcNow; }
        //    set { createdAt = value; }
        //}

        [DataType(DataType.DateTime)]
        public DateTime? ModifiedAt { get; set; }

        [MaxLength(450)]
        public string CreatedBy { get; set; }

        //public ApplicationUser CreatedByUser { get; set; }

        [MaxLength(450)]
        public string ModifiedBy { get; set; }

        //public ApplicationUser ModifiedByUser { get; set; }

        public bool IsDeleted { get; set; }
    }
}
