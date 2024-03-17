using System;

namespace Optima.Fais.Dto
{
    public class DimensionSync
    {
        public int Id { get; set; }
        public string Length { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime ModifiedAt { get; set; }
        public int? AssetCategoryId { get; set; }
    }
}
