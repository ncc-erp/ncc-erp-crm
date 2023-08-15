using Abp.Application.Services.Dto;
using Abp.Timing;
using System;

namespace CRM.Users.Dto
{
    //custom PagedResultRequestDto
    public class PagedUserResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
