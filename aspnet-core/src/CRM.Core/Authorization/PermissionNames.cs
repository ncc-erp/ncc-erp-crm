using Abp.MultiTenancy;
using System.Collections.Generic;
using static CRM.Authorization.Roles.StaticRoleNames;

namespace CRM.Authorization
{
    public static class PermissionNames
    {
        public const string Pages_Tenants = "Pages.Tenants";

        public const string Pages_Users = "Pages.Users";

        public const string Pages_Roles = "Pages.Roles";

        public const string Pages_Project = "Pages.Project";

        public const string Pages_View_Project = "Pages.View.Project";

        public const string Pages_Customer = "Pages.Customer";

        public const string Pages_Invoice = "Pages.Invoice";
    }
    public class SystemPermission
    {
        public string Permission { get; set; }
        public MultiTenancySides MultiTenancySides { get; set; }
        public string DisplayName { get; set; }
        public bool IsConfiguration { get; set; }

        public static List<SystemPermission> ListPermissions = new List<SystemPermission>()
        {
            // for default
            new SystemPermission{ Permission =  PermissionNames.Pages_Tenants, MultiTenancySides = MultiTenancySides.Host, DisplayName = "Tenants" },

            //for tenant
            new SystemPermission{ Permission =  PermissionNames.Pages_Users, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Users" },
            new SystemPermission{ Permission =  PermissionNames.Pages_Roles, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Roles" },
            new SystemPermission{ Permission =  PermissionNames.Pages_Project, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Projects", IsConfiguration = true },
            new SystemPermission{ Permission =  PermissionNames.Pages_Customer, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Customers", IsConfiguration = true },
            new SystemPermission{ Permission =  PermissionNames.Pages_View_Project, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "ViewProject", IsConfiguration = true },
            new SystemPermission{ Permission =  PermissionNames.Pages_Invoice, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant , DisplayName = "Invoice", IsConfiguration = true },
        };
    }

    public class GrantPermissionRoles
    {
        public static Dictionary<string, List<string>> PermissionRoles = new Dictionary<string, List<string>>()
        {

            {
                Tenants.Admin,
                new List<string>()
                {
                    PermissionNames.Pages_Customer,
                    PermissionNames.Pages_Project,
                    PermissionNames.Pages_Roles,
                    PermissionNames.Pages_Tenants,
                    PermissionNames.Pages_Users,
                    PermissionNames.Pages_View_Project,
                    PermissionNames.Pages_Invoice
                }
            },
            {
                Tenants.PM,
                new List<string>()
                {
                   PermissionNames.Pages_View_Project
                }
            },
            {
                Tenants.Sale,
                new List<string>()
                {
                    PermissionNames.Pages_Customer,
                    PermissionNames.Pages_Project
                }
            },
             {
                Tenants.Accountant,
                new List<string>()
                {
                    PermissionNames.Pages_Invoice
                }
            }

        };
    }
}
