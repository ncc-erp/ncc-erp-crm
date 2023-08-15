using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CRM.Enums.StatusEnum;

namespace CRM.Deals.Dto
{
    public class SaveDealDto : EntityDto<long>
    {
        public string Name { get; set; }
        public long OwnerId { get; set; }
        public long? ClientId { get; set; }
        public long? ContactId { get; set; }
        public double? Amount { get; set; }
        public DealStatus Status { get; set; }
        public string Description { get; set; }
        public string WinReason { get; set; }
        public string LoseReason { get; set; }
        public Priority Priority { get; set; }
        public DateTime? DealStartDate { get; set; }
        public DateTime? DealLastFollow { get; set; }
        public List<CreateDealDetailDto> DealDetail { get; set; }
    }

    public class CreateDealDetailDto
    {
        public long SkillId { get; set; }
        public long LevelId { get; set; }
        public int Quantity { get; set; }
    }
}
