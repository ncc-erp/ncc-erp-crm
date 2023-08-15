using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.Contracts.Dto
{
    public class MileStoneDto : EntityDto<long>
    {
        public string Name { get; set; }
        public DateTime MileStoneDate { get; set; }
    }
}
