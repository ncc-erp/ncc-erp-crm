using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CRM.Enums.StatusEnum;

namespace CRM.Projects.Dto
{
    public class ViewProjectDto : EntityDto<long>
    {
        public string Name { get; set; }
        public ProjectType Type { get; set; }
        public long ClientId { get; set; }
        public string ClientName { get; set; }
        public ProjectStatus Status { get; set; }
    }

    public class ProjectDetailDto
    {
        public long Id { get; set; }
        public  long ClientId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public ProjectType ProjectType { get; set; }
        public List<string> UserName { get; set; }
        public string Description { get; set; }
        public string Client { get; set; }
        public ProjectStatus ProjectStatus { get; set; }
        public IEnumerable<ProjectContractDetailDto> ProjectContractDetails { get; set; }
        public List<long> AssigneeId { get; set; }
    }

    public class ProjectContractDetailDto
    {
        public long ProjectId { get; set; }
        public long ContractId { get; set; }
        public CurrencyType ContractCurrency { get; set; }
        public string ContractName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public ContractStatus ContractStatus { get; set; }
        public float ContractValue { get; set; }
        public ContractType ContractType { get; set; }
        public IEnumerable<ContractMileStoneDto> ContractMileStones { get; set; }
        public IEnumerable<ContractInVoiceDto> ContractInVoices { get; set; }
    }

    public class ContractMileStoneDto
    {
        public long MileStoneId { get; set; }
        public double Percentage { get; set; }
        public string MileStone { get; set; }
        public string Description { get; set; }
        public DateTime MileStoneDate { get; set; }
        public float MileStoneValue { get; set; }
        public InvoiceStatus InvoiceStatus { get; set; }
    }
    public class ContractInVoiceDto
    {
        public long InvoiceId { get; set; }
        public DateTime Time { get; set; }
        public InvoiceStatus InvoiceStatus { get; set; }
        public List<PeopleInCharge> PeopleInCharges { get; set; }

    }

    public class PeopleInCharge
    {
        public long UserId { get; set; }
        public long InvoiceUserId { get; set; }
        public string UserName { get; set; }
        public string Position { get; set; }
        public double Rate { get; set; }
        public float ManMonth { get; set; }
    }
}
