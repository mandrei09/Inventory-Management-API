using Optima.Fais.Model;
using System;

namespace Optima.Fais.Dto
{
    public class PrintLabel
	{
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public System.DateTime? ModifiedAt { get; set; }
		public Asset Asset { get; set; }
		public DateTime? PrintDate { get; set; }
		public DateTime UploadDate { get; set; }
		public bool Hidden { get; set; }
		public CodeNameEntity Company { get; set; }
	}
}
