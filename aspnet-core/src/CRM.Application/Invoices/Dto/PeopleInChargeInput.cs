using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.Invoices.Dto
{
    public class PeopleInChargeInput
    {
        public long InvoiceId { get; set; }
        public List<PeopleInput> People { get; set; }
    }
    public class PeopleInput
    {
        public bool IsAllMonth { get; set; }
        public long InvoiceUserId { get; set; }
        public long UserId { get; set; }
        public string Position { get; set; }
        public double Rate { get; set; }
        public float ManMonth { get; set; }
    }
}
