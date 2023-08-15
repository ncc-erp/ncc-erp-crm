using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CRM.WorkFlows.Dto
{
    public class WorkflowStatusDto
    {
        public long? Id { get; set; }
        public long WorkflowId { get; set; }
        public int Status { get; set; }
        [Required]
        public string StatusName { get; set; }
    }
}
