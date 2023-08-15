using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CRM.Entities
{
    public class Team : FullAuditedEntity<long>, IMayHaveTenant
    {
        [Required]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int? TenantId { get; set; }
    }
}
