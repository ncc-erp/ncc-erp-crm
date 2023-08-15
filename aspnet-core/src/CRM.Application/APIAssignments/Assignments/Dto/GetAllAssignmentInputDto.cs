using CRM.Paging;
using System;
using System.Collections.Generic;
using System.Text;
using static CRM.Enums.StatusEnum;

namespace CRM.APIAssignments.Assignments.Dto
{
    public class GetAllAssignmentInputDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string SearchUser { get; set; }
        public string SearchEntity { get; set; }
        public Priority? Priority { get; set; }
        public EntityDefault? EntityType { get; set; }
        public AssignmentStatus? Status { get; set; }
        public GridParam Param { get; set; }

    }
}
