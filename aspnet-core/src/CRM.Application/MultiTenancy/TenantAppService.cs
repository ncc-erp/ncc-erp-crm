using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using CRM.Authorization;
using CRM.Authorization.Roles;
using CRM.Authorization.Users;
using CRM.Editions;
using CRM.MultiTenancy.Dto;
using Microsoft.AspNetCore.Identity;

namespace CRM.MultiTenancy
{
    [AbpAuthorize(PermissionNames.Pages_Tenants)]
    public class TenantAppService : AsyncCrudAppService<Tenant, TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>, ITenantAppService
    {
        private readonly TenantManager _tenantManager;
        private readonly EditionManager _editionManager;
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IAbpZeroDbMigrator _abpZeroDbMigrator;
        private readonly IPermissionManager _permissionManager;

        public TenantAppService(
            IRepository<Tenant, int> repository,
            TenantManager tenantManager,
            EditionManager editionManager,
            UserManager userManager,
            RoleManager roleManager,
            IAbpZeroDbMigrator abpZeroDbMigrator,
            IPermissionManager permissionManager)
            : base(repository)
        {
            _tenantManager = tenantManager;
            _editionManager = editionManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _abpZeroDbMigrator = abpZeroDbMigrator;
            _permissionManager = permissionManager;
        }

        public override async Task<TenantDto> Create(CreateTenantDto input)
        {
            CheckCreatePermission();

            // Create tenant
            var tenant = ObjectMapper.Map<Tenant>(input);
            tenant.ConnectionString = input.ConnectionString.IsNullOrEmpty()
                ? null
                : SimpleStringCipher.Instance.Encrypt(input.ConnectionString);

            var defaultEdition = await _editionManager.FindByNameAsync(EditionManager.DefaultEditionName);
            if (defaultEdition != null)
            {
                tenant.EditionId = defaultEdition.Id;
            }

            await _tenantManager.CreateAsync(tenant);
            await CurrentUnitOfWork.SaveChangesAsync(); // To get new tenant's id.

            // Create tenant database
            _abpZeroDbMigrator.CreateOrMigrateForTenant(tenant);

            // We are working entities of new tenant, so changing tenant filter
            using (CurrentUnitOfWork.SetTenantId(tenant.Id))
            {
                // Create static roles for new tenant
                CheckErrors(await _roleManager.CreateStaticRoles(tenant.Id));

                await CurrentUnitOfWork.SaveChangesAsync(); // To get static role ids

                // Grant all permissions to admin role
                var adminRole = _roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.Admin);
                var allPermissions = _permissionManager.GetAllPermissions(tenancyFilter: false).ToList();
                await AssignPermissionToRole(adminRole, allPermissions);
                //await _roleManager.GrantAllPermissionsAsync(adminRole);

                // Create admin user for the tenant
                var adminUser = User.CreateTenantAdminUser(tenant.Id, input.AdminEmailAddress);
                await _userManager.InitializeOptionsAsync(tenant.Id);
                CheckErrors(await _userManager.CreateAsync(adminUser, User.DefaultPassword));
                await CurrentUnitOfWork.SaveChangesAsync(); // To get admin user's id

                /* accountant Administrator Role*/
                var accountantAdminName = StaticRoleNames.Tenants.Accountant;
                var accountantAdminRole = new Role
                {
                    Name = accountantAdminName,
                    DisplayName = accountantAdminName,
                    Description = accountantAdminName,
                    IsStatic = false
                };
                accountantAdminRole.SetNormalizedName();
                await _roleManager.CreateAsync(accountantAdminRole);//wait for creating courseAdminRole completed
                await CurrentUnitOfWork.SaveChangesAsync(); // To get static role ids

                /* PM Administrator Role*/
                var PMAdminName = StaticRoleNames.Tenants.PM;
                var PMAdminRole = new Role
                {
                    Name = PMAdminName,
                    DisplayName = PMAdminName,
                    Description = PMAdminName,
                    IsStatic = false
                };
                PMAdminRole.SetNormalizedName();
                await _roleManager.CreateAsync(PMAdminRole);//wait for creating studentRole completed
                await CurrentUnitOfWork.SaveChangesAsync(); // To get static role ids

                /* Sale Administrator Role*/
                var saleAdminName = StaticRoleNames.Tenants.Sale;
                var saleAdminRole = new Role
                {
                    Name = saleAdminName,
                    DisplayName = saleAdminName,
                    Description = saleAdminName,
                    IsStatic = false
                };
                saleAdminRole.SetNormalizedName();
                await _roleManager.CreateAsync(saleAdminRole);//wait for creating studentRole completed
                await CurrentUnitOfWork.SaveChangesAsync(); // To get static role ids

                // Assign admin user to role!
                CheckErrors(await _userManager.AddToRoleAsync(adminUser, adminRole.Name));
                await CurrentUnitOfWork.SaveChangesAsync();
            }

            return MapToEntityDto(tenant);
        }

        private async Task AssignPermissionToRole(Role r, List<Permission> listPermissions)
        {
            List<string> permissions = GrantPermissionRoles.PermissionRoles.ContainsKey(r.Name) ? GrantPermissionRoles.PermissionRoles[r.Name] : new List<string>();
            if (permissions != null && permissions.Count > 0)
            {
                var dbpermissions = listPermissions.Where(s => permissions.Contains(s.Name));
                await _roleManager.SetGrantedPermissionsAsync(r, dbpermissions);
            }
        }

        protected override IQueryable<Tenant> CreateFilteredQuery(PagedTenantResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.TenancyName.Contains(input.Keyword) || x.Name.Contains(input.Keyword))
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive);
        }

        protected override void MapToEntity(TenantDto updateInput, Tenant entity)
        {
            // Manually mapped since TenantDto contains non-editable properties too.
            entity.Name = updateInput.Name;
            entity.TenancyName = updateInput.TenancyName;
            entity.IsActive = updateInput.IsActive;
        }

        public override async Task Delete(EntityDto<int> input)
        {
            CheckDeletePermission();

            var tenant = await _tenantManager.GetByIdAsync(input.Id);
            await _tenantManager.DeleteAsync(tenant);
        }

        private void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}

