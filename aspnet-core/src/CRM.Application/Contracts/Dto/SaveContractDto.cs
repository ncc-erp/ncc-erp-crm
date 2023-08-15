using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using CRM.ContractMileStones.Dto;
using CRM.Entities;
using CRM.Projects.Dto;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using static CRM.Enums.StatusEnum;

namespace CRM.Contracts.Dto
{
    public class SaveContractDto : EntityDto<long>
    {
        public long ProjectId { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public ContractType Type { get; set; }
        public ContractStatus Status { get; set; }
        public long ContractValue { get; set; }
        public List<ContractMileStonesDto> MileStones { get; set; }
        public CurrencyType ContractCurrency { get; set; }
    }

    public class ContractMileStonesDto : EntityDto<long?>
    {
        public string Name { get; set; }
        public DateTime? MileStoneDate { get; set; }
        public string Description { get; set; }
        public double? Percentage { get; set; }
        public float? MileStoneValue { get; set; }
    }

}
