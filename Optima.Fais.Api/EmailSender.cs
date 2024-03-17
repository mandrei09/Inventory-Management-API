using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit.Security;
using Microsoft.Extensions.DependencyInjection;
using MimeKit;
using MimeKit.Utils;
using Optima.Fais.Data;
using Optima.Fais.Dto;
using Optima.Fais.Model.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Optima.Fais.Api
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;
        public IServiceProvider _services { get; }

        public EmailSender(EmailConfiguration emailConfig, IServiceProvider services)
        {
            _emailConfig = emailConfig;
            _services = services;
        }

        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

        public async Task<bool> SendEmailAsync(Message message)
        {
            var mailMessage = CreateEmailMessage(message);


            try
            {
				bool IsSend = await SendAsync(mailMessage);
				return IsSend;

			}
            catch
            {
                //log an error message or throw an exception, or both.
                return false;
                throw;
            }
            finally
            {
               

            }

        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(MailboxAddress.Parse(_emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Cc.AddRange(message.Cc);
            emailMessage.Bcc.AddRange(message.Bcc);
            emailMessage.Subject = message.Subject;
            emailMessage.Priority = MessagePriority.Normal;


            //emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };
            // emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = string.Format("<h2 style='color:red;'>{0}</h2>", message.Content) };

            var bodyBuilder = new BodyBuilder { HtmlBody = string.Format("{0}", message.Content) };
            //string HtmlFormat = string.Empty;

            //string FullFormatPath = Path.Combine(Environment.CurrentDirectory, "", "");
            //string[] ImgPaths = Directory.GetFiles(Path.Combine(FullFormatPath, "upload"));

            var multipart = new Multipart("mixed");
            multipart.Add(bodyBuilder.ToMessageBody());

            if (message.Files != null && message.Files.Any())
			{
                foreach (var item in message.Files)
				{
					var attachment = new MimePart(item.Item3)
					{
						Content = new MimeContent(File.OpenRead(item.Item1)),
						ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
						ContentTransferEncoding = ContentEncoding.Base64,
						FileName = item.Item2
					};

                    multipart.Add(attachment);
                }

                
            }

            emailMessage.Body = multipart;
            return emailMessage;
        }

        private void Send(MimeMessage mailMessage)
        {;
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.Auto);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_emailConfig.UserName, _emailConfig.Password);

                    client.Send(mailMessage);
                   
                }
                catch
                {
                    //log an error message or throw an exception or both.
                  
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                  
                }
            }
        }

        //private async Task<bool> SendAsync(MimeMessage mailMessage)
        //{
        //    using (var client = new SmtpClient())
        //    {
        //        try
        //        {
        //            client.CheckCertificateRevocation = false;
        //            await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.Auto);
        //            client.AuthenticationMechanisms.Remove("XOAUTH2");
        //            // await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);

        //            await client.SendAsync(mailMessage);
        //            return true;

        //        }
        //        catch (Exception e)
        //        {
        //            Console.Write("Meldingen kunne ikke sendes til en eller flere mottakere.", ConsoleColor.Red);
        //            Console.Write(e.Message, ConsoleColor.DarkRed);

        //            // for other error types just write the info without the FailedRecipient
        //            using (var errorfile = System.IO.File.CreateText("error-" + DateTime.Now.Ticks + ".txt"))
        //            {
        //                errorfile.WriteLine(e.StackTrace);
        //                errorfile.WriteLine(e.ToString());

        //                using (var scope = _services.CreateScope())
        //                {
        //                    var dbContext =
        //                       scope.ServiceProvider
        //                           .GetRequiredService<ApplicationDbContext>();


        //                    Model.EmailManager emailManager = new Model.EmailManager()
        //                    {
        //                        Info = e.ToString(),
        //                        EmailTypeId = 11
        //                    };

        //                    dbContext.Add(emailManager);
        //                    dbContext.SaveChanges();



        //                }





        //            }

        //            return false;

        //        }
        //        finally
        //        {
        //            await client.DisconnectAsync(true);
        //            client.Dispose();

        //        }
        //    }
        //}

        private async Task<bool> SendAsync(MimeMessage mailMessage)
        {
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    //client.CheckCertificateRevocation = false;
                    await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.Auto);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
#if DEBUG
                    await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);  // Trebuie comentat 
#endif
					await client.SendAsync(mailMessage);
                    return true;

                }
                catch (SmtpFailedRecipientException se)
                {
                    using (var errorfile = System.IO.File.CreateText(System.IO.Path.Combine("errors", "error-" + DateTime.Now.Ticks + ".txt")))
                    {
                        errorfile.WriteLine(se.StackTrace);
                        // variable se is already the right type, so no need to cast it      
                        errorfile.WriteLine(se.FailedRecipient);
                        errorfile.WriteLine(se.ToString());
                    }

                    return false;
                }
                catch (Exception e)
                {
                    Console.Write("Meldingen kunne ikke sendes til en eller flere mottakere.", ConsoleColor.Red);
                    Console.Write(e.Message, ConsoleColor.DarkRed);

                    // for other error types just write the info without the FailedRecipient
                    using (var errorfile = System.IO.File.CreateText(System.IO.Path.Combine("errors", "error-" + DateTime.Now.Ticks + ".txt")))
                    {
                        errorfile.WriteLine(e.StackTrace);
                        errorfile.WriteLine(e.ToString());

                        //using (var scope = _services.CreateScope())
                        //{
                        //    var dbContext =
                        //       scope.ServiceProvider
                        //           .GetRequiredService<ApplicationDbContext>();


                        //    Model.EmailManager emailManager = new Model.EmailManager()
                        //    {
                        //        Info = e.ToString(),
                        //        EmailTypeId = 11
                        //    };

                        //    dbContext.Add(emailManager);
                        //    dbContext.SaveChanges();



                        //}





                    }

                    return false;

                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();

                }
            }
        }

        public async Task<List<EmailReadResponse>> ReadEmails(int maxCount = 1000)
        {
            //       using (var emailClient = new Pop3Client())
            //       {
            //           emailClient.Connect(_emailConfig.SmtpServer, 995, SecureSocketOptions.Auto);

            //           emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

            //           emailClient.Authenticate(_emailConfig.UserName, _emailConfig.Password);


            //           List<EmailReadResponse> emails = await Task.Run(() => new List<EmailReadResponse>());

            //           for (int i = 0; i < emailClient.Count && i < maxCount; i++)
            //           {
            //               var message = emailClient.GetMessage(i);

            //               var emailMessage = new EmailReadResponse();
            //               emailMessage.Subject = message.Subject;
            //               emailMessage.TextBody = message.GetTextBody(MimeKit.Text.TextFormat.Html);


            //               emailMessage.From = new List<string>();

            //               for (int f = 0; f < message.From.Count; f++)
            //{
            //                   emailMessage.From.Add(message.From.OfType<MailboxAddress>().ToList()[f].Address);
            //               }

            //               emailMessage.To = new List<string>();

            //               for (int f = 0; f < message.To.Count; f++)
            //               {
            //                   emailMessage.To.Add(message.To.OfType<MailboxAddress>().ToList()[f].Address);
            //               }

            //               emailMessage.Cc = new List<string>();

            //               for (int f = 0; f < message.Cc.Count; f++)
            //               {
            //                   emailMessage.From.Add(message.Cc.OfType<MailboxAddress>().ToList()[f].Address);
            //               }

            //               emailMessage.SendDate = message.Date.DateTime.ToLocalTime();
            //               emails.Add(emailMessage);
            //           }

            //           return emails;
            //       }


            using (var emailClient = new ImapClient())
            {
                emailClient.Connect(_emailConfig.SmtpServer, 993, SecureSocketOptions.Auto);

                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                emailClient.Authenticate(_emailConfig.UserName, _emailConfig.Password);

                var inbox = emailClient.Inbox;
                inbox.Open(FolderAccess.ReadOnly);


                if (inbox.Count > 0)
                {
                    // fetch the UIDs of the newest 100 messages
                    int index = Math.Max(inbox.Count - 10, 0);
                    var items = inbox.Fetch(index, -1, MessageSummaryItems.UniqueId | MessageSummaryItems.BodyStructure);

                    foreach (var item in items)
                    {
                        if (item.TextBody != null)
                        {
                            var mime = (TextPart)inbox.GetBodyPart(item.UniqueId, item.TextBody);
                            var text = mime.Text;

                            Console.WriteLine("This is the text/plain content:");
                            Console.WriteLine("{0}", text);
                        }
                    }

                } 



                // var Ids = inbox.Search(SearchQuery.SentSince(DateTime.Now.AddDays(-3)));
                var Ids = inbox.Search(SearchQuery.DeliveredAfter(DateTime.Parse(DateTime.Now.AddDays(-2).ToString("dd-MMM-yyyy"))));
				// var Ids = inbox.Search(SearchQuery.All);

				//var items = inbox.Fetch(0, -1, MessageSummaryItems.UniqueId | MessageSummaryItems.BodyStructure);

				//foreach (var item in items)
				//{
				//	if (item.TextBody != null)
				//	{
				//		var mime = (TextPart)inbox.GetBodyPart(item.UniqueId, item.TextBody);
				//		var text = mime.Text;

				//		Console.WriteLine("This is the text/plain content:");
				//		Console.WriteLine("{0}", text);
				//	}
				//}

				List<EmailReadResponse> emails = await Task.Run(() => new List<EmailReadResponse>());

				for (int i = 0; i < Ids.Count && i < maxCount; i++)
				{
					var message = inbox.GetMessage(i);

					var emailMessage = new EmailReadResponse();
					emailMessage.Subject = message.Subject;
					emailMessage.TextBody = message.GetTextBody(MimeKit.Text.TextFormat.Html);


					emailMessage.From = new List<string>();

					for (int f = 0; f < message.From.Count; f++)
					{
						emailMessage.From.Add(message.From.OfType<MailboxAddress>().ToList()[f].Address);
					}

					emailMessage.To = new List<string>();

					for (int f = 0; f < message.To.Count; f++)
					{
						emailMessage.To.Add(message.To.OfType<MailboxAddress>().ToList()[f].Address);
					}

					emailMessage.Cc = new List<string>();

					for (int f = 0; f < message.Cc.Count; f++)
					{
						emailMessage.From.Add(message.Cc.OfType<MailboxAddress>().ToList()[f].Address);
					}

					emailMessage.SendDate = message.Date.DateTime.ToLocalTime();
					emails.Add(emailMessage);
				}

				emailClient.Disconnect(true);

				return emails;
            }
        }
	}
}
