using Abp.Application.Services.Dto;
using System.Collections.Generic;
using static CRM.Enums.StatusEnum;

namespace CRM.Contacts.Dto
{
    public class ViewContactDto : EntityDto<long>
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; }
        public string Role { get; set; }
        public string Description { get; set; }
        public List<ClientInContact> Clients { get; set; }
        public long? CreatorUserId { get; set; }
    }
    public class ClientInContact : EntityDto<long>
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; }
        public ClientType Type { get; set; }
        public string Country { get; set; }
        public string Website { get; set; }
        public string Description { get; set; }
        public ClientStatus Status { get; set; }
    }
}
