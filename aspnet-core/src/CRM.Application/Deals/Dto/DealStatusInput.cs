using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CRM.Enums.StatusEnum;

namespace CRM.Deals.Dto
{
    public class DealStatusInput : EntityDto<long>
    {
        public DealStatus Status { get; set; }
    }
}
