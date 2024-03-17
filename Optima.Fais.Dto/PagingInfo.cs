using System;

namespace Optima.Fais.Dto
{
    public class PagingInfo
    {
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get { return (int)Math.Ceiling((decimal)TotalItems / PageSize); } }
        public decimal TotalNetAmount { get; set; } = 0;
        public decimal TotalTaxAmount { get; set; } = 0;
        public bool ShowFooter { get; set; } = false;
    }
}
