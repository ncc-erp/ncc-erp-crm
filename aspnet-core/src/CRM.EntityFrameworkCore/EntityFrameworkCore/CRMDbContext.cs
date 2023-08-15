using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using CRM.Authorization.Roles;
using CRM.Authorization.Users;
using CRM.MultiTenancy;
using CRM.Entities;

namespace CRM.EntityFrameworkCore
{
    public class CRMDbContext : AbpZeroDbContext<Tenant, Role, User, CRMDbContext>
    {
        /* Define a DbSet for each entity of the application */

        public DbSet<Client> Clients { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectAssignee> ProjectAssignee { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ContractMileStone> ContractMileStones { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceUser> InvoiceUsers { get; set; }
        public DbSet<InvoiceFile> InvoiceFiles { get; set; }
        public DbSet<InvoiceComment> InvoiceComments { get; set; }
        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<WorkflowStatus> WorkflowStatuses { get; set; }
        public DbSet<WorkflowTransition> WorkflowTransitions { get; set; }
        public DbSet<TransitionPermission> TransitionPermissions { get; set; }
        public DbSet<ContractFile> ContractFiles { get; set; }
        public DbSet<Deal> Deals { get; set; }
        public DbSet<DealFile> DealFiles { get; set; }
        public DbSet<DealComment> DealComments { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ClientContact> ClientContacts { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamPermission> TeamPermissions { get; set; }
        public DbSet<UserTeamPermission> UserTeamPermissions { get; set; }
        public DbSet<UserTeam> UserTeams { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<EntityAssignment> EntityAssignments { get; set; }
        public DbSet<UserAssignment> UserAssignments { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<CampaignContact> CampaignContacts { get; set; }
        public DbSet<CampaignEmailTemplate> CampaignEmailTemplates { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<TechStack> TechStacks { get; set; }
        public DbSet<TechStackDeal> TechStacksDeal { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<DealDetail> DealDetails { get; set; }
        public CRMDbContext(DbContextOptions<CRMDbContext> options)
            : base(options)
        {
        }
    }
}
