using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Entities
{
    public class WorkflowTransition : FullAuditedEntity<long>, IMayHaveTenant
    {
        [ForeignKey(nameof(WorkflowId))]
        public Workflow Workflow { get; set; }
        public long WorkflowId { get; set; }
        public int FromStatus { get; set; }
        public int ToStatus { get; set; }
        public int? TenantId { get; set; }
    }
}