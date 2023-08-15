using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using CRM.Configuration;
using CRM.Web;

namespace CRM.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class CRMDbContextFactory : IDesignTimeDbContextFactory<CRMDbContext>
    {
        public CRMDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<CRMDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            CRMDbContextConfigurer.Configure(builder, configuration.GetConnectionString(CRMConsts.ConnectionStringName));

            return new CRMDbContext(builder.Options);
        }
    }
}
