using System.Collections.Generic;

namespace Optima.Fais.Model
{
	public partial class InterCompany : Entity
	{
		public InterCompany()
		{

		}

		public string Code { get; set; }

		public string Name { get; set; }

		public int? CompanyId { get; set; }

		public virtual Company Company { get; set; }

		public int? InterCompanyENId { get; set; }

		public virtual InterCompanyEN InterCompanyEN { get; set; }

		public int? ERPId { get; set; }

		public int? AccountId { get; set; }

		public virtual Account Account { get; set; }

		public int? ExpAccountId { get; set; }

		public virtual ExpAccount ExpAccount { get; set; }

		public int? AssetCategoryId { get; set; }

		public virtual AssetCategory AssetCategory { get; set; }

		public int? AssetTypeId { get; set; }

		public virtual AssetType AssetType { get; set; }

		public virtual ICollection<Accountancy> Accountancies { get; set; }

	}
}
