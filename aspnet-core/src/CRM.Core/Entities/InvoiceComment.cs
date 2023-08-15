using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using CRM.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CRM.Entities
{
    public class InvoiceComment : FullAuditedEntity<long>, IMayHaveTenant
    {
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        [Required]
        public long UserId { get; set; }
        [ForeignKey(nameof(InvoiceId))]
        public Invoice Invoice { get; set; }
        [Required]
        public long InvoiceId { get; set; }
        [Required]
        public string Comment { get; set; }
        public int? TenantId { get; set; }
    }
}
