using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using CRM.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.ContractMileStones.Dto
{
    [AutoMapTo(typeof(ContractMileStone))]
    public class SaveContractMileStoneDto : EntityDto<long>
    {
        public long ContractId { get; set; }
        public string Name { get; set; }
        public DateTime MileStoneDate { get; set; }
        public string Description { get; set; }
        public double Percentage { get; set; }
        public double Value { get; set; }
    }
}
