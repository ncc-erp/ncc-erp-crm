using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Entities
{

    public class ContractFile : FullAuditedEntity<long>, IMayHaveTenant
    {
        [ForeignKey(nameof(ContractId))]
        public Contract Contract { get; set; }
        [Required]
        public long ContractId { get; set; }
        [Required]
        public string FileUrl { get; set; }
        public int? TenantId { get; set; }
    }

}