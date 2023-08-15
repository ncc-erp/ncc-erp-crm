using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.WorkFlows.Dto
{
    public class WorkflowTransitionDto
    {
        //public string WorkflowName { get; set; }
        public int FromStatus { get; set; }
        public int ToStatus { get; set; }
        public bool CanChange { get; set; }
    }
}
