using System;

namespace CRM.Users.Dto
{
    internal class LoginInfor
    {
        public long? Id { get; set; }
        public DateTime LastLoginTime { get; set; }
    }
}