using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CRM.Entities
{
    public class ClientContact : FullAuditedEntity<long>, IMayHaveTenant
    {
        [ForeignKey(nameof(ClientId))]
        public Client Client { get; set; }
        [Required]
        public long ClientId { get; set; }
        [ForeignKey(nameof(ContactId))]
        public Contact Contact { get; set; }
        [Required]
        public long ContactId { get; set; }
        public int? TenantId { get; set; }
    }
}
