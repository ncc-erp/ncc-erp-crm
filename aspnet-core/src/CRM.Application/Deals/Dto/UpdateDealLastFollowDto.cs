using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.Deals.Dto
{
    public class UpdateDealLastFollowDto
    {
        public long DealId { get; set; }
        public DateTime? DealLastFollow { get; set; }  
    }
}
