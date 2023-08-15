using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CRM.Entities
{
    public class Contact : FullAuditedEntity<long>, IMayHaveTenant
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Phone { get; set; }
        public string Mail { get; set; }
        public string Role { get; set; }
        public string Description { get; set; }
        public int? TenantId { get; set; }
        public string FirstName { get; set; }
    }
}
