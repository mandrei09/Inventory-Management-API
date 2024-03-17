using MimeKit;
using Optima.Fais.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Optima.Fais.Api
{
    public interface IEmailSender
    {
        void SendEmail(Message message);
        Task<bool> SendEmailAsync(Message messages);
        Task<List<EmailReadResponse>> ReadEmails(int maxCount);
    }
}
