namespace CRM.EntityFrameworkCore.Seed.Host
{
    public class InitialHostDbBuilder
    {
        private readonly CRMDbContext _context;

        public InitialHostDbBuilder(CRMDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            new DefaultWorkflowCreator(_context).Create();
            new DefaultEditionCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();

            _context.SaveChanges();
        }
    }
}
