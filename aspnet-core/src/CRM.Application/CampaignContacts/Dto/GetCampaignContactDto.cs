using Abp.Application.Services.Dto;
using CRM.EmailTemplates.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CRM.Enums.StatusEnum;

namespace CRM.CampaignContacts.Dto
{
    public class GetCampaignContactDto : EntityDto<long>
    {
        public string CampaignName { get; set; }
        public IEnumerable<GetContactDto> Contacts { get; set; }
        public GetEmailTemplateDto MailTemplate { get; set; }
        public List<GetEmailTemplatesDto> MailTemplates { get; set; }
    }
    public class GetContactDto : EntityDto<long>
    {
        public string ContactEmail { get; set; }
        public string ContactName { get; set; }
        public CampaignContactStatus Status { get; set; }
    }
    public class GetEmailTemplateDto
    {
        public string Subject { get; set; }
        public string Content { get; set; }
    }
    public class GetEmailTemplatesDto
    {
        public long? Id { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
    }


}
