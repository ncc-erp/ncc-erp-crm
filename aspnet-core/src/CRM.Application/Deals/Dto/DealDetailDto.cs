using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.Deals.Dto
{
    public class DealDetailDto
    {
        public long SkillId { get; set; }
        public string SkillName { get; set; }
        public long LevelId { get; set; }
        public string LevelName { get; set; }
        public int Quantity { get; set; }
    }
}
