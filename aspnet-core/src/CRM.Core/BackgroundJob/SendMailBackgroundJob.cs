using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Net.Mail;
using CRM.BackgroundWorker;
using CRM.Entities;
using CRM.IoC;
using CRM.Uitls;

namespace CRM.BackgroundJob
{
    public class SendMailBackgroundJob : BackgroundJob<SendMailBGJ>, ITransientDependency
    {
        private readonly IEmailSender _emailSender;
        private readonly IWorkScope _workScope;

        public SendMailBackgroundJob(IEmailSender emailSender, IWorkScope workScope)
        {
            _emailSender = emailSender;
            _workScope = workScope;
        }
        [UnitOfWork]
        public override void Execute(SendMailBGJ args)
        {
            var campcontact = _workScope.Get<CampaignContact>(args.CampaignContactId);

            if (CheckEmailExist.IsExistEmailAddress(args.ReceiverEmail))
            {
                _emailSender.Send(
                   to: args.ReceiverEmail,
                   subject: args.Subject,
                   body: args.Body,
                   isBodyHtml: true
               );
                campcontact.Status = Enums.StatusEnum.CampaignContactStatus.Sent;
                _workScope.Update(campcontact);
            }
            else
            {
                campcontact.Status = Enums.StatusEnum.CampaignContactStatus.EmailNotExist;
                _workScope.Update(campcontact);
            }
        }
    }
}
