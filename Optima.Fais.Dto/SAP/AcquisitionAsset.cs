using System;
using System.Collections.Generic;
using System.Text;

namespace Optima.Fais.Dto
{
	public class AcquisitionAsset
	{
		public string Sap_function { get; set; }
		public AcquisitionAssetDataOptions Options { get; set; }
		public string Remote_host_name { get; set; }
		public IList<AcquisitionAssetData> Data { get; set; }
	}

	public class AcquisitionAssetData
	{
		public AcquisitionAssetInput I_INPUT { get; set; }
	}

	public class AcquisitionAssetInput
	{
		public string STORNO { get; set; }
		public string COMPANYCODE { get; set; }
		public string DOC_DATE { get; set; }
		public string PSTNG_DATE { get; set; }
		public string REF_DOC_NO { get; set; }
		public string HEADER_TXT { get; set; }
		public string VENDOR_NO { get; set; }
		public string CURRENCY { get; set; }
		public decimal EXCH_RATE { get; set; }
		public decimal TOTAL_AMOUNT { get; set; }
		public IList<AcquisitionAssets> ASSETS { get; set; }
	}

	public class AcquisitionAssets
	{
		public string ASSET { get; set; }
		public string SUBNUMBER { get; set; }
		public string ITEM_TEXT { get; set; }
		public string TAX_CODE { get; set; }
		public decimal NET_AMOUNT { get; set; }
		public decimal TAX_AMOUNT { get; set; }
		public string GL_ACCOUNT { get; set; }
		public string ASVAL_DATE { get; set; }
		public string WBS_ELEMENT { get; set; }
	}

	public class AcquisitionAssetDataOptions
	{
		public int Api_call_timeout { get; set; }
	}

	public class AcquisitionAssetSAP
	{
		public string STORNO { get; set; }
		public string COMPANYCODE { get; set; }
		public string DOC_DATE { get; set; }
		public string PSTNG_DATE { get; set; }
		public string REF_DOC_NO { get; set; }
		public string HEADER_TXT { get; set; }
		public string VENDOR_NO { get; set; }
		public string CURRENCY { get; set; }
		public decimal EXCH_RATE { get; set; }
		public decimal TOTAL_AMOUNT { get; set; }
		public IList<AcquisitionAssets> ASSETS { get; set; }
        public int AssetId { get; set; }
    }

	public class AcquisitionAssetSAPView
	{
		public string STORNO { get; set; }
		public string COMPANYCODE { get; set; }
		public string DOC_DATE { get; set; }
		public string PSTNG_DATE { get; set; }
		public string REF_DOC_NO { get; set; }
		public string HEADER_TXT { get; set; }
		public string VENDOR_NO { get; set; }
		public string CURRENCY { get; set; }
		public string EXCH_RATE { get; set; }
		public string NET_AMOUNT { get; set; }
		public string TAX_AMOUNT { get; set; }
		public string TOTAL_AMOUNT { get; set; }
		public string ASSET { get; set; }
		public int Id { get; set; }
		public int AssetId { get; set; }
		public string GL_ACCOUNT { get; set; }
		public string ITEM_TEXT { get; set; }
		public string SUBNUMBER { get; set; }
		public string TAX_CODE { get; set; }
		public string ASVAL_DATE { get; set; }
		public string WBS_ELEMENT { get; set; }
		public string TOTAL_TAX_AMOUNT { get; set; }
		public string TOTAL_NET_AMOUNT { get; set; }
	}
}
