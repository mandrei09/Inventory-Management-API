//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Optima.Fais.Model
//{
//	public class AcquisitionAssets : Entity
//	{
//		public string ASSET { get; set; }
//		public string SUBNUMBER { get; set; }
//		public string ITEM_TEXT { get; set; }
//		public string TAX_CODE { get; set; }
//		public decimal NET_AMOUNT { get; set; }
//		public decimal TAX_AMOUNT { get; set; }
//		public string GL_ACCOUNT { get; set; }

//		public int BudgetManagerId { get; set; }
//		public virtual BudgetManager BudgetManager { get; set; }
//		public int AccMonthId { get; set; }
//		public virtual AccMonth AccMonth { get; set; }

//		public int AssetId { get; set; }
//		public virtual Asset Asset { get; set; }

//		public int AcquisitionAssetSAPId { get; set; }
//		public virtual AcquisitionAssetSAP AcquisitionAssetSAP { get; set; }

//		public bool NotSync { get; set; }
//		public Guid Guid { get; set; }
//		public int SyncErrorCount { get; set; }
//	}
//}
