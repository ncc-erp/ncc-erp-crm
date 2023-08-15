using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.InvoiceComments.Dto
{
    public class ViewInvoiceCommentDto : EntityDto<long>
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public long InvoiceId { get; set; }
        public string InvoiceName { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
