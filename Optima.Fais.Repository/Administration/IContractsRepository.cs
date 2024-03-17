using Optima.Fais.Dto;
using Optima.Fais.Model;
using Optima.Fais.Model.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Optima.Fais.Repository
{
    public interface IContractsRepository : IRepository<Model.Contract>
    {

        IEnumerable<Model.ContractDetail> GetContract(ContractFilter budgetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal);
        IEnumerable<Model.ContractDetail> GetContractUI(ContractFilter budgetFilter, string includes, Sorting sorting, Paging paging, out AssetDepTotal depTotal, out AssetCategoryTotal catTotal);
        int CreateOrUpdateContract(ContractSave asset);
        Model.Contract GetDetailsById(int id, string includes);
    }
}
