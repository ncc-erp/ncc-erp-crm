using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.Deals.Dto
{
    public class DealContact
    {
        public long dealId { get; set; }
        public long? jointId { get; set; }
        public string jointName { get; set; }
    }
}
