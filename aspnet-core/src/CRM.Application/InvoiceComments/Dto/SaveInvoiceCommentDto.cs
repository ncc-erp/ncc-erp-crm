using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using CRM.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.InvoiceComments.Dto
{
    [AutoMapTo(typeof(InvoiceComment))]
    public class SaveInvoiceCommentDto : EntityDto<long>
    {
        public long UserId { get; set; }
        public long InvoiceId { get; set; }
        public string Comment { get; set; }
    }
}
