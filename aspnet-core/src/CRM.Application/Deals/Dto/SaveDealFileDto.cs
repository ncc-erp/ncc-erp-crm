using Abp.Application.Services.Dto;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.Deals.Dto
{
    public class SaveDealFileDto : EntityDto<long>
    {
        public long DealId { get; set; }
        public IFormFile File { get; set; }
    }
}
