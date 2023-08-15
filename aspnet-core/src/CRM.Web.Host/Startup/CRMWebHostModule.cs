using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using CRM.Configuration;
using CRM.Localization;
using Abp.Threading.BackgroundWorkers;
using CRM.BackgroundWorker;

namespace CRM.Web.Host.Startup
{
    [DependsOn(
       typeof(CRMWebCoreModule))]
    public class CRMWebHostModule : AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public CRMWebHostModule(IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(CRMWebHostModule).GetAssembly());
            var workManager = IocManager.Resolve<IBackgroundWorkerManager>();
            workManager.Add(IocManager.Resolve<EmailBackgroundWorker>());
            workManager.Add(IocManager.Resolve<InvoiceBackgroundWorker>());
        }
    }
}
