using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using CRM.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static CRM.Enums.StatusEnum;

namespace CRM.Entities
{
    public class Deal : FullAuditedEntity<long>, IMayHaveTenant
    {
        public string Name { get; set; }
        [ForeignKey(nameof(OwnerId))]
        public User Owner { get; set; }
        [Required]
        public long OwnerId { get; set; }
        [ForeignKey(nameof(ClientId))]
        public Client Client { get; set; }
        public long? ClientId { get; set; }
        public long? ContactID { get; set; }
        [ForeignKey(nameof(ContactID))]
        public Contact Contact { get; set; }
        [Required]
        public double Amount { get; set; }
        public DealStatus Status { get; set; }
        public string Description { get; set; }
        public DateTime? ClosingDate { get; set; }
        public string WinReason { get; set; }
        public string LoseReason { get; set; }
        public int? TenantId { get; set; }
        public DateTime? DealStartDate { get; set; }
        public DateTime? DealLastFollow { get; set; }
        public string Note { get; set; }
        public Priority? Priority { get; set; }
        public PotentialType? PotentialType { get; set; }
    }
}
