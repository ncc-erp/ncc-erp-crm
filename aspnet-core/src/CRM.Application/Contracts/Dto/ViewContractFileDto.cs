using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.Contracts.Dto
{
    public class ViewContractFileDto : EntityDto<long>
    {
        public string FileUrl { get; set; }
    }
}
