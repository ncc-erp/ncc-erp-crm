using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using CRM.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using static CRM.Enums.StatusEnum;

namespace CRM.Invoices.Dto
{
    public class InvoiceOutPut : EntityDto<long>
    {
        public string ProjectName { get; set; }
        public string InvoiceName { get; set; }
        public long ProjectId { get; set; }
        public long ClientId { get; set; }
        public string ClientName { get; set; }
        public DateTime Time { get; set; }
        public ContractType Type { get; set; }

        public string TypeName { get; set; }
        public InvoiceStatus Status { get; set; }
        public long Assigne { get; set; }

        public String AssigneeName { get; set; }
        public bool ChangeFromPendingToChasing { get; set; }
        public bool ChangeFromChasing { get; set; }
        public bool ChangeFromFailToChasing { get; set; }
        public bool ChangeFromPaidToChasing { get; set; }
        public bool ChangeFromChasingToPending { get; set; }
        public bool ChangeFromChasingToFail { get; set; }
        public bool ChangeFromChasingToPaid { get; set; }
    }
    public class InvoiceInput
    {
        public long? ProjectId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public ContractType? Type { get; set; }
        public InvoiceStatus? Status { get; set; }
        public long? Assigne { get; set; }
    }

    public class InvoiceStatusInput: EntityDto<long>
    {
        public InvoiceStatus Status { get; set; }
    }

    [AutoMapTo(typeof(Invoice))]
    public class InvoiceInfomationInput : InvoiceStatusInput
    {
        public string InvoiceName { get; set; }
        public InvoiceStatus Status { get; set; }
        public long Assignee { get; set; }
        public long ProjectId { get; set; }
        //public long ClientId { get; set; }
        public DateTime Time { get; set; }
        public long ContractId { get; set; }
        public CurrencyType Currency { get; set; }
        public long Value { get; set; }
        public ContractType InvoiceType { get; set; }
    }
}
