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
    public class Project : FullAuditedEntity<long>, IMayHaveTenant
    {
        [Required]
        public string Name { get; set; }
        public string Code { get; set; }
        [Required]
        public ProjectType Type { get; set; }
        [ForeignKey(nameof(ClientId))]
        public Client Client { get; set; }
        [Required]
        public long ClientId { get; set; }
        [ForeignKey(nameof(DealId))]
        public Deal Deal { get; set; }
        public long? DealId { get; set; }
        [Required]
        public ProjectStatus Status { get; set; }
        public string Description { get; set; }
        public int? TenantId { get; set; }
    }
}
