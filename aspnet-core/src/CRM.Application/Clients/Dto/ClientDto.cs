using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using AutoMapper;
using CRM.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static CRM.Enums.StatusEnum;

namespace CRM.Clients.Dto
{
    [AutoMapTo(typeof(Client))]
    public class ClientDetailDto : ClientDto
    {
        public string Phone { get; set; }
        public string Mail { get; set; }
        public ClientType Type { get; set; }
        public string Country { get; set; }
        public string Website { get; set; }
        public string Description { get; set; }
        public ClientStatus Status { get; set; }
        public long? RegionId { get; set; }
        public string RegionName { get; set; }
        public ClientWorkflowTransitionDto ClientWorkflowTransition { get; set; }
        public List<ProjectClientDto> Projects { get; set; }
        public List<DealClientDto> Deals { get; set; }
    }

    public class ClientDto : EntityDto<long>
    {
        public string Name { get; set; }
    }

    public class ClientStatusDto : EntityDto<long>
    {
        public ClientStatus Status { get; set; }

    }

    public class ClientWorkflowTransitionDto
    {
        public bool CanChangeToNew { get; set; }
        public bool CanChangeToRegularContact { get; set; }
        public bool CanChangeToInactiveContact { get; set; }
    }
}
