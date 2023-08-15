using CRM.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.Deals.Dto
{
    public class GetallInput
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string DealStartDateSort { get; set; }
        public string DealLastFollowSort { get;set; }
        public GridParam Param { get; set; }
    }
}
