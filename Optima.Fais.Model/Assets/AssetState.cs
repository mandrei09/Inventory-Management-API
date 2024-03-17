namespace Optima.Fais.Model
{
    public partial class AssetState : Entity
    {
        public AssetState()
        {
        }

        public string Code { get; set; }

        public string Name { get; set; }

        //public int AssetTypeId { get; set; }

        //public virtual AssetType AssetType { get; set; }

        public int? CompanyId { get; set; }

        public virtual Company Company { get; set; }

        public bool IsInitial { get; set; }

        public bool IsFinal { get; set; }

        public bool HasDep { get; set; }

        public string BadgeColor { get; set; }

		public string BadgeIcon { get; set; }
	}
}
