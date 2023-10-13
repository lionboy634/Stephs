using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using static Stephs_Shop.Services.EmailSender;

namespace Stephs_Shop.Services
{
    public interface IEmailSender
    {
        void SendEmail(EmailMessage _emailContent);
    }


    public class EmailSender : IEmailSender
    {

        public EmailSender()
        {

        }
        public void SendEmail(EmailMessage emailContent)
        {
            /*try
            {
                _logger.LogInformation("Processing Email");
				var emailMessage = new EmailMessage()
				{
					subject = emailContent.subject,
					from = "",
					message = emailContent.message,
				};
				emailMessage.To.AddRange(emailContent.To);
			}
            catch(Exception e)
            {
                _logger.LogError("Sending Email With Errors : {}", e.Message);
            }
        }*/

        }


		public class EmailMessage
        {
            public List<string> To { get; set; }
            public string subject { get; set; }
            public string from { get; set; }
            public string message { get; set; }

        }
    }
}
