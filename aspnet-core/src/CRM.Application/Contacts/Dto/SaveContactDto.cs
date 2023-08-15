using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.Contacts.Dto
{
    public class SaveContactDto : EntityDto<long>
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; }
        public string Role { get; set; }
        public string Description { get; set; }
        public string FirstName { get; set; }
    }
}
