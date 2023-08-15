using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static CRM.Enums.StatusEnum;

namespace CRM.Entities
{
    public class Invoice : FullAuditedEntity<long>, IMayHaveTenant
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public ContractType Type { get; set; }
        [ForeignKey(nameof(ContractId))]
        public Contract Contract { get; set; }
        [Required]
        public long ContractId { get; set; }
        [Required]
        public DateTime InvoiceDate { get; set; }
        [Required]
        public InvoiceStatus Status { get; set; }
        public long Assignee { get; set; }
        [Required]
        public long Order { get; set; }
        public long? ContractMileStoneId { get; set; }
        public float Value { get; set; }
        public CurrencyType Currency { get; set; }
        public int? TenantId { get; set; }
    }
}
