using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.Contacts.Dto
{
    public class SaveClientContactDto
    {
        public long ClientId { get; set; }
        public List<long> Contacts { get; set; }
    }
}
