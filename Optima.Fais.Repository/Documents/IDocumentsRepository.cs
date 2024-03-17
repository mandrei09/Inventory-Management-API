using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Optima.Fais.Repository
{
    public interface IDocumentsRepository : IRepository<Model.Document>
    {
        Task<Model.TransferResult> Save(Dto.DocumentUpload document);
        Task<Model.TransferResult> WFHSave(Dto.DocumentUpload document);
        Model.TransferResult SaveStateChange(Dto.DocumentUpload document);
        Task<Model.TransferResult> Validate(Dto.DocumentUpload document);
        Task<Model.TransferResult> PersonelValidate(Dto.PersonelValidate document);
        Task<Model.TransferResult> ManagerValidate(Dto.PersonelValidate document);
		Task<Model.TransferResult> Reject(Dto.DocumentUpload document);
		Task<Model.TransferResult> RejectFromStock(Dto.DocumentUpload document);
	}
}
