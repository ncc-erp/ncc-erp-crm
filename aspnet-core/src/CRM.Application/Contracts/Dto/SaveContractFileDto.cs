using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using CRM.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.Contracts.Dto
{
    public class SaveContractFileDto : EntityDto<long>
    {
        public long ContractId { get; set; }
        public IFormFile File { get; set; }
    }
}
