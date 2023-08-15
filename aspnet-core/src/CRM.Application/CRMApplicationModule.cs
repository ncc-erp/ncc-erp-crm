using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using CRM.Authorization;

namespace CRM
{
    [DependsOn(
        typeof(CRMCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class CRMApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<CRMAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(CRMApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
