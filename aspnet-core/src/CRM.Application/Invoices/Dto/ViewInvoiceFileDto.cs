using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static CRM.Enums.StatusEnum;

namespace CRM.Invoices.Dto
{
    public class ViewInvoiceFileDto : EntityDto<long>
    {
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public InvoiceFileType Type { get; set; }
        public string TypeString { get; set; }
    }
}
