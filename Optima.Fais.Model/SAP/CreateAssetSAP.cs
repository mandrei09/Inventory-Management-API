using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Optima.Fais.Model
{
	public class CreateAssetSAP : Entity
	{
		public int AssetId { get; set; }
		[ForeignKey("AssetId")]
		public virtual Asset Asset { get; set; }

		public string XSUBNO { get; set; }
		public string COMPANYCODE { get; set; }
		public string ASSET { get; set; }
		public string SUBNUMBER { get; set; }
		public string ASSETCLASS { get; set; }
		public string POSTCAP { get; set; }
		public string DESCRIPT { get; set; }
		public string DESCRIPT2 { get; set; }
		public string INVENT_NO { get; set; }
		public string SERIAL_NO { get; set; }
		public int QUANTITY { get; set; }
		public string BASE_UOM { get; set; }
		public string LAST_INVENTORY_DATE { get; set; }
		public string LAST_INVENTORY_DOCNO { get; set; }
		public string CAP_DATE { get; set; }
		public string COSTCENTER { get; set; }
		public string RESP_CCTR { get; set; }
		public string INTERN_ORD { get; set; }
		public string PLANT { get; set; }
		public string LOCATION { get; set; }
		public string ROOM { get; set; }
		public string PERSON_NO { get; set; }
		public string PLATE_NO { get; set; }
		public string ZZCLAS { get; set; }
		public string IN_CONSERVATION { get; set; }
		public string PROP_IND { get; set; }
		public string OPTIMA_ASSET_NO { get; set; }
		public string OPTIMA_ASSET_PARENT_NO { get; set; }
		public string TESTRUN { get; set; }
		public string VENDOR_NO { get; set; }

		public int BudgetManagerId { get; set; }
		public virtual BudgetManager BudgetManager { get; set; }
		public int AccMonthId { get; set; }
		public virtual AccMonth AccMonth { get; set; }

		public bool NotSync { get; set; }
		public Guid Guid { get; set; }
		public int SyncErrorCount { get; set; }
		public bool FromStock { get; set; }
		public bool FromClone { get; set; }
		public bool InvPlus { get; set; }

		public int? ErrorId { get; set; }
		public virtual Error Error { get; set; }

		public string INVOICE { get; set; }

		public bool IsTesting { get; set; }
	}
}
