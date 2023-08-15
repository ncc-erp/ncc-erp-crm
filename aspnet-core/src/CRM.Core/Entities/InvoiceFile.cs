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
    public class InvoiceFile : FullAuditedEntity<long>, IMayHaveTenant
    {
        [ForeignKey(nameof(InvoiceId))]
        public Invoice Invoice { get; set; }
        [Required]
        public long InvoiceId { get; set; }
        [Required]
        public InvoiceFileType Type { get; set; }
        [Required]
        public string FileUrl { get; set; }
        public int? TenantId { get; set; }
    }
}
