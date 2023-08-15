using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static CRM.Enums.StatusEnum;

namespace CRM.Entities
{
    public class Assignment : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public AssignmentStatus Status { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime Deadline { get; set; }
        public string Description { get; set; }
        public Priority Priority { get; set; }
    }
}
