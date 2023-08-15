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
    public class Client : FullAuditedEntity<long>, IMayHaveTenant
    {
        [Required]
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; }
        [Required]
        public ClientType Type { get; set; }
        public string Country { get; set; }
        public string Website { get; set; }
        public string Description { get; set; }
        public ClientStatus Status { get; set; }
        public int? TenantId { get; set; }
        public long? RegionId { get; set; }
        [ForeignKey(nameof(RegionId))]
        public Region Region { get; set; }
    }
}
