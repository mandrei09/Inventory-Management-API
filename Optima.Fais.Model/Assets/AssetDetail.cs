namespace Optima.Fais.Model
{
    public class AssetDetail
    {
        public Asset Asset { get; set; }
        //public AssetAdmMD AdmMD { get; set; }
        //public AssetDepMD DepMD { get; set; }
        public AssetDep Dep { get; set; }
        public AssetAC AC { get; set; }
    }
}
