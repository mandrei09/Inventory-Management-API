using Optima.Fais.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface IDocumentTypesRepository : IRepository<DocumentType>
    {
        Task<DocumentType> GetByCodeAsync(string code);
        IEnumerable<DocumentType> GetDocumentTypesByFilters(string parentCode);
        IEnumerable<DocumentType> GetDocumentTypesByPrefix(string parentCode);
    }
}
