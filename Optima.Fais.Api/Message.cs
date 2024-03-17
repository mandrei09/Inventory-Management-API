using Microsoft.AspNetCore.Http;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Optima.Fais.Api
{
    public class Message
    {
        public List<MailboxAddress> To { get; set; }
        public List<MailboxAddress> Cc { get; set; }
        public List<MailboxAddress> Bcc { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public List<(string, string, string)> Files { get; set; }

        //public IFormFileCollection Attachts { get; set; }
        //public Message(IEnumerable<string> to, string subject, string content, IFormFileCollection attachments)
        //{
        //    To = new List<MailboxAddress>();

        //    To.AddRange(to.Select(x => MailboxAddress.Parse(x)));
        //    Subject = subject;
        //    Content = content;
        //    Attachments = attachments;
        //}

        public Message(IEnumerable<string> to, IEnumerable<string> cc, IEnumerable<string> bcc, string subject, string content, List<(string fileName, string filePath, string fileType)> files)
        {
            To = new List<MailboxAddress>();
            Cc = new List<MailboxAddress>();
            Bcc = new List<MailboxAddress>();
            Files = new List<(string, string, string)>();

            To.AddRange(to.Select(x => MailboxAddress.Parse(x)));
            Cc.AddRange(cc.Select(x => MailboxAddress.Parse(x)));
            Bcc.AddRange(bcc.Select(x => MailboxAddress.Parse(x)));
            Subject = subject;
            Content = content;

            if (files != null)
            {
                foreach (var f in files)
                {
                    Files.Add(new (f.fileName, f.filePath, f.fileType));

                }
            }

        }
    }
}
