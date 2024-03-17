using MigraDoc.DocumentObjectModel;
using System.Collections.Generic;

namespace Optima.Faia.Api.Services
{
    public class InvDecisionValidationResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class DocumentGeneratorResult
    {
        public int DocumentTypeId { get; set; }
        public string DocumentNumber { get; set; }
        public string StoredAs { get; set; }
        public int PageCount { get; set; }
        public int LastElementBottom { get; set; }
        public SigningArea SigningArea { get; set; }
        public List<ParticipantDetail> Participants { get; set; }
        public SigningArea SigningValidationArea { get; set; }
        public List<ParticipantDetail> ValidationParticipants { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public bool Landscape { get; set; }
    }

    public class DocumentDetail
    {
        public int AppDocumentId { get; set; }
        //public Model.AppDocument AppDocument { get; set; }
        public int PageCount { get; set; }
        public int LastElementBottom { get; set; }
        public SigningArea SigningArea { get; set; }
        public bool Justifying { get; set; }
        public bool Landscape { get; set; }
    }

    public class ParticipantDetail
    {
        public string Email { get; set; }
        public SigningArea SigningArea { get; set; }
        public int ParticipantIndex { get; set; }
    }

    public class PdfDocumentResult
    {
        public Document Document { get; set; }
        public int DocumentTypeId { get; set; }
        public string DocumentNumber { get; set; }
        public List<ParticipantDetail> Participants { get; set; }
        public List<ParticipantDetail> ValidationParticipants { get; set; }
    }

    //public class ParticipantArea
    //{
    //    public string Email { get; set; }
    //    public SigningArea SigningArea { get; set; }
    //}

    public class SigningArea
    {
        public double Top { get; set; }
        public double Left { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public double LTop { get; set; }
        public double LLeft { get; set; }
        public double LHeight { get; set; }
        public double LWidth { get; set; }
    }

    public class SigningEmployeeDetail
    {
        public Optima.Fais.Model.Employee Employee { get; set; }
        public string Info { get; set; }
        public string Position { get; set; }

    }
}
