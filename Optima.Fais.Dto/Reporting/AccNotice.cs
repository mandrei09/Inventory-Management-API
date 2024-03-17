using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Dto.Reporting
{
    public class AccNotice
    {
        public string ProviderAdministration { get; set; }
        public string ReceiverAdministration { get; set; }
        public string SourceDocumentNumber { get; set; }
        public string DocumentDate { get; set; }

        public List<AccNoticeAsset> Assets { get; set; }

        public List<AccNoticeAsset> ListAccNotice()
        {
            return Assets;
        }
    }
}
