using System;
using System.Collections.Generic;

namespace Stephs_Shop.Services
{
    public interface IEmailSender
    {
         void SendEmail(EmailMessage _emailContent);
    }


    public class EmailSender : IEmailSender
    {

        public void SendEmail(EmailMessage _emailContent)
        {

            var emailMessage = new EmailMessage();
            emailMessage.subject = _emailContent.subject;
            emailMessage.from = _emailContent.from;
            emailMessage.To.AddRange(_emailContent.To);
            emailMessage.message = _emailContent.message;


            try
            {
                
            }
            catch(Exception e)
            {

            }
        }
       
    }


    public class EmailMessage
    {
        public List<string> To { get; set; }
        public string subject { get; set; }
        public string from { get; set; }
        public string message { get; set; }

    }
}
