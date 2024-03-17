using System;

namespace Optima.Fais.Dto
{
    public class CostCenterImport
    {
        public string CompanyCode { get; set; }
        public string CostCenterCode { get; set; }
        public string CostCenterName { get; set; }
        public string AdmCenterCode { get; set; }
        public string UserId { get; set; }
		public int CountLines { get; set; }
        public int CurrentIndex { get; set; }
    }
}
