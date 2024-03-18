using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Model
{
	public class MatrixImport : Entity
	{
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string CostCenterCode { get; set; }
        public string CostCenterName { get; set; }
        public string AdmCenter { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public string AssetTypeCode { get; set; }
        public string AssetTypeName { get; set; }
        public string L1UserId { get; set; }
        public string L2UserId { get; set; }
        public string L3UserId { get; set; }
        public string L4UserId { get; set; }
        public string S1UserId { get; set; }
        public string S2UserId { get; set; }
        public string S3UserId { get; set; }
        public string UserId { get; set; }
        public decimal L1UserSum { get; set; }
        public decimal L2UserSum { get; set; }
        public decimal L3UserSum { get; set; }
        public decimal L4UserSum { get; set; }
        public decimal S1UserSum { get; set; }
        public decimal S2UserSum { get; set; }
        public decimal S3UserSum { get; set; }
		public bool Used { get; set; }
		public bool Imported { get; set; }
	}
}
