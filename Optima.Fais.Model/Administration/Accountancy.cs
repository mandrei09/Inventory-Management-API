using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Optima.Fais.Model
{
    public partial class Accountancy : Entity
    {
        public Accountancy()
        {
        }

        //public int? InterCompanyId { get; set; }

        //[ForeignKey("InterCompanyId")]
        //public InterCompany InterCompany { get; set; }

        public int AccountId { get; set; }

        public Account Account { get; set; }

        public int ExpAccountId { get; set; }

        public ExpAccount ExpAccount { get; set; }

        public int AssetTypeId { get; set; }

        public AssetType AssetType { get; set; }

        public int AssetCategoryId { get; set; }

        public AssetCategory AssetCategory { get; set; }

        public int? SubCategoryId { get; set; }

        public SubCategory SubCategory { get; set; }

        public decimal Value { get; set; }
    }
}
