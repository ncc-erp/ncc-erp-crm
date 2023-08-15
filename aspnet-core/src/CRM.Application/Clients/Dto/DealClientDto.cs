using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CRM.Enums.StatusEnum;

namespace CRM.Clients.Dto
{
    public class DealClientDto : EntityDto<long>
    {
        public string Name { get; set; }
        public long OwnerId { get; set; }
        public string OwnerName { get; set; }
        public double Amount { get; set; }
        public DealStatus Status { get; set; }
        public string Description { get; set; }
        public DateTime? ClosingDate { get; set; }
        public string WinReason { get; set; }
        public string LoseReason { get; set; }
    }
}
