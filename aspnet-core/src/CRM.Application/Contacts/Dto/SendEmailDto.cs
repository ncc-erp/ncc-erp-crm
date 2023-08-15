using CRM.EmailTemplates.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.Contacts.Dto
{
    public class SendEmailDto
    {
        public long CampaignId { get; set; }
        public EmailTemplateInputDto EmailTemplateDto { get; set; }
    }
}
