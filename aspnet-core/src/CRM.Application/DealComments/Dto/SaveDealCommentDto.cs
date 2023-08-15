using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.DealComments.Dto
{
    public class SaveDealCommentDto : EntityDto<long>
    {
        public long DealId { get; set; }
        public string Content { get; set; }
    }
}
