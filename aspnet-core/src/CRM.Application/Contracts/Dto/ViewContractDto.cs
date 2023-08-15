using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CRM.Enums.StatusEnum;

namespace CRM.Contracts.Dto
{
    public class ViewContractDto : EntityDto<long>
    {
        public long ProjectId { get; set; }
        public string ProjectName { get; set; }
        public long ClientId { get; set; }
        public String ClientName { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public ContractType Type { get; set; }
        public ContractStatus Status { get; set; }
        public float ContractValue { get; set; }
        public List<MileStoneDto> MileStones { get; set; }
    }
}
