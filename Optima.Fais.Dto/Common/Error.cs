using System;

namespace Optima.Fais.Dto
{
    public class Error
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Request { get; set; }
        public int? ErrorTypeId { get; set; }
        public ErrorType ErrorType { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public string BUKRS { get; set; }

        public string BELNR { get; set; }

        public string GJAHR { get; set; }

        public AssetEntity Asset { get; set; }
    }
}
