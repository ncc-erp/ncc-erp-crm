using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.CampaignContacts.Dto
{
    public class CreateCampaignContactDto : EntityDto<long>
    {
        public string Name { get; set; }
        public long[] ContactId { get; set; }
    }
}
