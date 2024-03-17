using MigraDoc.DocumentObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api.Services
{

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
        public int DocumentNumber { get; set; }
		public List<ParticipantDetail> Participants { get; set; }
		public List<ParticipantDetail> ValidationParticipants { get; set; }
	}

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
        public Model.Employee Employee { get; set; }
        public string Info { get; set; }
        public Model.InvCommitteePosition InvCommitteePosition { get; set; }
    }
}
