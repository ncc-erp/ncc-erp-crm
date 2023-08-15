using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CRM.Enums.StatusEnum;

namespace CRM.Deals.Dto
{
    public class ViewDealDto : EntityDto<long>
    {
        public string Name { get; set; }
        public long OwnerId { get; set; }
        public string OwnerName { get; set; }
        public long? ClientId { get; set; }
        public string ClientName { get; set; }
        public long? ContactId { get; set; }
        public string ContactName { get; set; }
        public double Amount { get; set; }
        public DealStatus Status { get; set; }
        public string Description { get; set; }
        public DateTime? ClosingDate { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string WinReason { get; set; }
        public string LoseReason { get; set; }
        public ProjectInDeal Project { get; set; }
        public bool ChangeFromProcessingToInProgress { get; set; }
        public bool ChangeFromProcessingToLost { get; set; }
        public bool ChangeFromInProgressToWin { get; set; }
        public bool ChangeFromInProgressToFail { get; set; }
        public bool ChangeToProcessing { get; set; }
        public DateTime? CreationTime { get; set; }
        public Priority? Priority { get; set; }
        public List<DealDetailDto> DealDetails { get; set;}
        public DateTime? DealStartDate { get; set; }
        public DateTime? DealLastFollow { get; set; }
    }
    public class ProjectInDeal : EntityDto<long>
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public ProjectStatus Status { get; set; }
    }
}
