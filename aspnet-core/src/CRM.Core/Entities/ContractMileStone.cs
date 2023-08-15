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
    public class ContractMileStone : FullAuditedEntity<long>, IMayHaveTenant
    {
        [ForeignKey(nameof(ContractId))]
        public Contract Contract { get; set; }
        public long ContractId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime MileStoneDate { get; set; }
        public string Description { get; set; }
        public double Percentage { get; set; }
        public float Value { get; set; }
        public CurrencyType Currency { get; set; }
        public int? TenantId { get; set; }
    }
}
