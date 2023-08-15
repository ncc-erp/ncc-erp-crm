using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.ContractMileStones.Dto
{
    public class ViewContractMileStoneDto : EntityDto<long>
    {
        public long ContractId { get; set; }
        public string ContractName { get; set; }
        public string Name { get; set; }
        public DateTime MileStoneDate { get; set; }
        public string Description { get; set; }
        public double Percentage { get; set; }
        public double Value { get; set; }
    }
}
