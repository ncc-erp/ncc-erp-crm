using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;

namespace CRM.Entities
{
    public class Workflow : FullAuditedEntity<long>, IMayHaveTenant
    {
        [Required]
        public string EntityName { get; set; }
        [Required]
        public string WorkflowName { get; set; }
        public bool IsActive { get; set; }
        public int DefaultStatus { get; set; }
        public int? TenantId { get; set; }
    }
}