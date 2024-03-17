using System;

namespace Optima.Fais.Dto
{
     public class BudgetForecastImport
    {
        public string BudgetCode { get; set; }
        public string Employee { get; set; }
        public string Project { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public string Activity { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentCode { get; set; }
        public string AdmCenter { get; set; }
        public string Region { get; set; }
        public string DivisionName { get; set; }
        public string DivisionCode { get; set; }
        public string ProjectTypeName { get; set; }
        public string ProjectTypeCode { get; set; }
        public string Info { get; set; }
        public string AssetTypeName { get; set; }
        public string AssetTypeCode { get; set; }
        public string AppState { get; set; }
        public string StartMonth { get; set; }
        public int DepPeriod { get; set; }
        public int DepPeriodRem { get; set; }
        public decimal ValueRem { get; set; }
        public string UserId { get; set; }
        public decimal ValueMonth1 { get; set; }
        public decimal ValueMonth2 { get; set; }
        public decimal ValueMonth3 { get; set; }
        public decimal ValueMonth4 { get; set; }
        public decimal ValueMonth5 { get; set; }
        public decimal ValueMonth6 { get; set; }
        public decimal ValueMonth7 { get; set; }
        public decimal ValueMonth8 { get; set; }
        public decimal ValueMonth9 { get; set; }
        public decimal ValueMonth10 { get; set; }
        public decimal ValueMonth11 { get; set; }
        public decimal ValueMonth12 { get; set; }
        public decimal ValueOrder { get; set; }
        public int CountLines { get; set; }
        public int CurrentIndex { get; set; }
        public string UniqueCode { get; set; }
    }
}
