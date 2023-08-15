using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using CRM.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using static CRM.Enums.StatusEnum;

namespace CRM.Invoices.Dto
{
    public class SaveInvoiceFileDto : EntityDto<long>
    {
        public long InvoiceId { get; set; }
        public InvoiceFileType Type { get; set; }
        public IFormFile File { get; set; }
    }
}
