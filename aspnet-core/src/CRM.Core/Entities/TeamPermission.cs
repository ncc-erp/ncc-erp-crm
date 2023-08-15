using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.Entities
{
    public class TeamPermission : FullAuditedEntity<long>, IMayHaveTenant
    {
        public string Name { get; set; }
        public int? TenantId { get; set; }
    }
}
