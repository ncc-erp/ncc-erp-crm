using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Entities
{
    public class WorkflowStatus : FullAuditedEntity<long>, IMayHaveTenant
    {
        [ForeignKey(nameof(WorkflowId))]
        public Workflow Workflow { get; set; }
        public long WorkflowId { get; set; }
        public int Status { get; set; }
        [Required]
        public string StatusName { get; set; }
        public int? TenantId { get; set; }
    }
}