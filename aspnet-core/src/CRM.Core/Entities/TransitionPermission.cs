using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using CRM.Authorization.Roles;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Entities
{
    public class TransitionPermission : FullAuditedEntity<long>, IMayHaveTenant
    {
        [ForeignKey(nameof(RoleId))]
        public Role Role { get; set; }
        public int RoleId { get; set; }
        [ForeignKey(nameof(WorkflowTransitionId))]
        public WorkflowTransition WorkflowTransition { get; set; }
        public long WorkflowTransitionId { get; set; }
        public int? TenantId { get; set; }
    }
}