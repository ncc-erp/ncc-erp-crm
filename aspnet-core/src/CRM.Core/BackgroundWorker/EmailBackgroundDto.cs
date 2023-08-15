using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.BackgroundWorker
{
    public class EmailBackgroundDto
    {
        public List<string> ReceiverEmails { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
    }
    public class SendMailBGJ
    {
        public string ReceiverEmail { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
        public long CampaignContactId { get; set; }
    }
}
