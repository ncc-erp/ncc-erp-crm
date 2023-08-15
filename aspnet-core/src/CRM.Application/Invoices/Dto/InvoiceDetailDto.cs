using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CRM.Enums.StatusEnum;

namespace CRM.Invoices.Dto
{
    public class InvoiceDetailDto : EntityDto<long>
    {
        public string Name { get; set; }
        public string ContractName { get; set; }
        public ContractType Type { get; set; }
        public long ContractId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public InvoiceStatus Status { get; set; }
        public long Assignee { get; set; }
        public long Order { get; set; }
        public float Value { get; set; }
        public CurrencyType Currency { get; set; }
        public string ProjectName { get; set; }
        public long ProjectId { get; set; }
        public long CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string AssigneeName { get; set; }
        public bool ChangeFromPendingToChasing { get; set; }
        public bool ChangeFromChasing { get; set; }
        public bool ChangeFromFailToChasing { get; set; }
        public bool ChangeFromPaidToChasing { get; set; }
        public InvoiceMileStoneDetailDto MileStoneDetail { get; set; }
        public List<InvoiceUserDetailDto> InvoiceUserDetail { get; set; }
    }

    public class InvoiceMileStoneDetailDto
    {
        public long? MileStoneId { get; set; }
        public string MileStoneName { get; set; }
        public string MileStoneDescription { get; set; }
        public DateTime? MilestoneTime { get; set; }
        public double? Percentage { get; set; }
    }
    public class InvoiceUserDetailDto : EntityDto<long>
    {
        public long UserId { get; set; }
        public long InvoiceId { get; set; }
        public double Rate { get; set; }
        public string Position { get; set; }
        public float ManMonth { get; set; }
    }
}
