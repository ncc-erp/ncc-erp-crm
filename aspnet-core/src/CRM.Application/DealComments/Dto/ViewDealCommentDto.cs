using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;

namespace CRM.DealComments.Dto
{
    public class ViewDealCommentDto : EntityDto<long>
    {
        public string Content { get; set; }
        public string UserName { get; set; }
        public DateTime CommentTime { get; set; }
        public long? ParentId { get; set; }
    }
}
