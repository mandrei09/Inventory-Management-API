using System.Collections.Generic;

namespace Optima.Fais.Model
{
    public partial class Material : Entity
    {
        public Material()
        {
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public int? SubTypeId { get; set; }

        public virtual SubType SubType { get; set; }

        public int? AccountId { get; set; }

        public virtual Account Account { get; set; }

        public int? ExpAccountId { get; set; }

        public virtual ExpAccount ExpAccount { get; set; }

        public int? AssetCategoryId { get; set; }

        public virtual AssetCategory AssetCategory { get; set; }

        public int? MaterialTypeId { get; set; }

        public virtual MaterialType MaterialType { get; set; }

		public string EAN { get; set; }

        public string PartNumber { get; set; }

        public decimal Value { get; set; }

        public decimal Price { get; set; }

        public decimal Quantity { get; set; }

        public virtual ICollection<RequestBudgetForecastMaterial> RequestBudgetForecastMaterial { get; set; }

    }
}
