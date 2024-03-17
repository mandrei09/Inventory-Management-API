using System;
using System.ComponentModel.DataAnnotations;

namespace Optima.Fais.Model
{
    public class EmailType : Entity
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string UploadFolder { get; set; }

        public bool NotifyEnabled { get; set; }

        public DateTime? NotifyStart { get; set; }

        public DateTime? NotifyEnd { get; set; }

        public int NotifyInterval { get; set; }

        public DateTime? NotifyLast { get; set; }

        [MaxLength]
        public string HeaderMsg { get; set; }

        [MaxLength]
        public string FooterMsg { get; set; }

        [MaxLength]
        public string CustomMsg { get; set; }

        public string Year { get; set; }

    }
}
