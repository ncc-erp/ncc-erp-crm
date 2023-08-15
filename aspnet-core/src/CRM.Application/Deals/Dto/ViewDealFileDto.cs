using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.Deals.Dto
{
    public class ViewDealFileDto : EntityDto<long>
    {
        public string FileUrl { get; set; }
    }
}
