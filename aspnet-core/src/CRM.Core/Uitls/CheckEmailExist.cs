using CRM.BackgroundWorker;
using EASendMail;
using System;
namespace CRM.Uitls
{
    public static class CheckEmailExist
    {
        public static bool IsExistEmailAddress(string email)
        {
            try
            {
                var check = new SmtpMail("TryIt") { To = email };
                SmtpServer oServer = new SmtpServer("");
                SmtpClient oSmtp = new SmtpClient();
                oSmtp.TestRecipients(oServer, check);
                return true;
            }
            catch (Exception ex)
            {
               return false;
            }
        }
    }
}
