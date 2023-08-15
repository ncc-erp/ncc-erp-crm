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
    public class Contract : FullAuditedEntity<long>, IMayHaveTenant
    {
        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; }
        [Required]
        public long ProjectId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime? EndTime { get; set; }
        [Required]
        public ContractType Type { get; set; }
        [Required]
        public ContractStatus Status { get; set; }
        [Required]
        public float  ContractValue { get; set; }
        public CurrencyType Currency{ get; set; }
        public int? TenantId { get; set; }
    }
}
