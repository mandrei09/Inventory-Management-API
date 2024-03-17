using System;

namespace Optima.Fais.Model
{

    public interface IBaseEntity
    {
        int Id { get; set; }
    }

    public interface IEntity: IBaseEntity
    {
        //int Id { get; set; }

        DateTime CreatedAt { get; set; }
        DateTime? ModifiedAt { get; set; }
        string CreatedBy { get; set; }
        string ModifiedBy { get; set; }
        bool IsDeleted { get; set; }
    }
}
