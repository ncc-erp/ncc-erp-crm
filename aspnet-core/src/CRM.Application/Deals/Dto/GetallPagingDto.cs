using CRM.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.Deals.Dto
{
    public class GetallPagingDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public GridParam Param { get; set; }
    }
}
