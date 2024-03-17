using MigraDoc.DocumentObjectModel;
using Optima.Faia.Api.Services;
using System.Collections.Generic;

namespace Optima.Fais.Api.Services
{
    public interface IDocumentHelperService
    {
        List<ParticipantDetail> AddSignatureArea(Section section, string tag, List<SigningEmployeeDetail> employees, bool landscape);
        List<ParticipantDetail> AddSignatureAreaCommitteeMember(Section section, string tag, List<SigningEmployeeDetail> members, bool landscape = false);
        //Row AddSignatureItem(Table table, string firstInfo, string secondInfo);
        //Row AddSignatureTableHeader(Table table);
    }
}
