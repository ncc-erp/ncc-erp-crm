using Abp.Application.Services.Dto;
using CRM.Anotations;
using System;
using System.Collections.Generic;
using System.Text;
using static CRM.Enums.StatusEnum;

namespace CRM.APIAssignments.Assignments.Dto
{
    public class GetEntityDto : EntityDto<long>
    {
        [ApplySearchAttribute]
        public string Name { get; set; }
        public EntityDefault EntityType { get; set; }
    }
}
