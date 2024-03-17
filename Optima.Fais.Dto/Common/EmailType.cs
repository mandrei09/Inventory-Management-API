using System;

namespace Optima.Fais.Dto
{
    public class EmailType
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string UploadFolder { get; set; }

        public bool NotifyEnabled { get; set; }

        public DateTime? NotifyStart { get; set; }

        public DateTime? NotifyEnd { get; set; }

        public int NotifyInterval { get; set; }

        public DateTime? NotifyLast { get; set; }
        public string HeaderMsg { get; set; }

        public string FooterMsg { get; set; }

        public string CustomMsg { get; set; }
    }
}
