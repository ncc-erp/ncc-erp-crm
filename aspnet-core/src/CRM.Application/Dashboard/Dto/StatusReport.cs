using System;
using System.Collections.Generic;

namespace CRM.Dashboard.Dto
{
    public class StatusReport
    {
        public string Value { get; set; }
        public string Name { get; set; }
        public int Total { get; set; }
    }
    public class AccountReport
    {
        public bool Value { get; set; }
        public string Name { get; set; }
        public int Total { get; set; }
    }
    public class DashboardDto
    {
        public List<StatusReport> Clients { get; internal set; }
        public List<StatusReport> Projects { get; internal set; }
        public List<StatusReport> Invoices { get; internal set; }
        public List<AccountReport> AccountsCount { get; internal set; }
    }
}
