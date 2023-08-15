using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using CRM.Entities;
using System;
using System.Collections.Generic;
using System.Text;


namespace CRM.EmailTemplates.Dto
{
    [AutoMapTo(typeof(EmailTemplate))]
    public class EmailTemplateDto : EntityDto<long>
    {
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
    }
}
