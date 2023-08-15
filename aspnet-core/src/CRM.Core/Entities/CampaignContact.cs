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
    public class CampaignContact : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        [ForeignKey(nameof(ContactId))]
        public Contact Contact { get; set; }
        [Required]
        public long ContactId { get; set; }
        [ForeignKey(nameof(CampaignId))]
        public Campaign Campaign { get; set; }
        [Required]
        public long CampaignId { get; set; }
        public CampaignContactStatus Status { get; set; }
    }
}
