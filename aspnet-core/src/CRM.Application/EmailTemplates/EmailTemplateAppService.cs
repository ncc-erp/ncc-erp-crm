using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.BackgroundJobs;
using Abp.Net.Mail;
using Abp.UI;
using CRM.BackgroundJob;
using CRM.BackgroundWorker;
using CRM.Contacts.Dto;
using CRM.EmailTemplates.Dto;
using CRM.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CRM.Enums.StatusEnum;

namespace CRM.EmailTemplates
{
    [AbpAuthorize]
    public class EmailTemplateAppService : CRMAppServiceBase
    {
        private readonly BackgroundJobManager _backgroundJobManager;
        private readonly IEmailSender _emailSender;
        public EmailTemplateAppService(IEmailSender emailSender, BackgroundJobManager backgroundJobManager)
        {
            _emailSender = emailSender;
            _backgroundJobManager = backgroundJobManager;
        }
        [HttpPost]
        public async Task<EmailTemplateDto> Save(EmailTemplateDto input)
        {
            if (input.Id <= 0)
            {
                var template = ObjectMapper.Map<EmailTemplate>(input);
                input.Id = await WorkScope.GetRepo<EmailTemplate>().InsertAndGetIdAsync(template);
            }
            else
            {
                var template = await WorkScope.GetAsync<EmailTemplate>(input.Id);
                ObjectMapper.Map<EmailTemplateDto, EmailTemplate>(input, template);
                await WorkScope.UpdateAsync<EmailTemplate>(template);
            }
            return input;
        }
        [HttpDelete]
        public async System.Threading.Tasks.Task Delete(EntityDto<long> input)
        {
            var isExist = await WorkScope.GetAll<EmailTemplate>().AnyAsync(s => s.Id == input.Id);
            if (!isExist)
            {
                throw new UserFriendlyException(string.Format("Email template Id = {0} isn't exist", input.Id));
            }
            await WorkScope.DeleteAsync<EmailTemplate>(input.Id);
        }
        [HttpGet]
        public async Task<List<EmailTemplateDto>> GetAll()
        {
            return await WorkScope.GetAll<EmailTemplate>()
                .Select(s => new EmailTemplateDto { Id = s.Id, Subject = s.Subject, Content = s.Content, Name = s.Name }).ToListAsync();
        }
        [HttpGet]
        public async Task<EmailTemplateDto> Get(long Id)
        {
            return await WorkScope.GetAll<EmailTemplate>().Where(s => s.Id == Id)
                .Select(s => new EmailTemplateDto { Id = s.Id, Subject = s.Subject, Content = s.Content, Name = s.Name }).FirstOrDefaultAsync();
        }
        [HttpPost]
        public async Task<string> SendEmail(SendEmailDto input)
        {
            var campaignContact = WorkScope.GetAll<CampaignContact>()
                .Include(s => s.Contact)
                .Include(s => s.Campaign)
                .Where(s => input.CampaignId == s.CampaignId && s.Status == CampaignContactStatus.Unsent).ToList();
            var listEmail = campaignContact.Select(s => new { s.Contact.Mail, s.Contact.Name, s.Contact.FirstName, s.Id }).ToList();
            //insert campaignEmailTemplate

            var camTem = WorkScope.GetAll<CampaignEmailTemplate>()
                .Where(x => (x.CampaignId == input.CampaignId && x.TemplateId == input.EmailTemplateDto.Id)).FirstOrDefault();
            if (camTem != null)
            {
                camTem.Content = input.EmailTemplateDto.Content;
                await WorkScope.UpdateAsync(camTem);
            }
            else
            {
                await WorkScope.InsertAsync(new CampaignEmailTemplate
                {
                    CampaignId = input.CampaignId,
                    Subject = input.EmailTemplateDto.Subject,
                    Content = input.EmailTemplateDto.Content,
                    TemplateId = input.EmailTemplateDto.Id.Value
                });
            }
            foreach (var u in listEmail)
            {
                var email = new SendMailBGJ
                {
                    CampaignContactId = u.Id,
                    ReceiverEmail = u.Mail,
                    Body = input.EmailTemplateDto.Content.Replace("[Name]", u.Name).Replace("[FirstName]", u.FirstName),
                    Subject = input.EmailTemplateDto.Subject
                };
                _backgroundJobManager.Enqueue<SendMailBackgroundJob, SendMailBGJ>(email, BackgroundJobPriority.High);
            }
            foreach (var c in campaignContact)
            {
                c.Status = CampaignContactStatus.Sending;
                await WorkScope.UpdateAsync(c);
            }
            return $"Sending to {campaignContact.ToList().Count} contacts";
        }

        [HttpPost]
        public async Task<SendEmailDto> SaveCampaignEmailTemplate(SendEmailDto input)
        {
            var camTem = WorkScope.GetAll<CampaignEmailTemplate>()
                .Where(x => (x.CampaignId == input.CampaignId && x.TemplateId == input.EmailTemplateDto.Id)).FirstOrDefault();
            if (camTem != null)
            {
                camTem.Content = input.EmailTemplateDto.Content;
                await WorkScope.UpdateAsync(camTem);
            }
            else
            {
                await WorkScope.InsertAsync(new CampaignEmailTemplate
                {
                    CampaignId = input.CampaignId,
                    Subject = input.EmailTemplateDto.Subject,
                    Content = input.EmailTemplateDto.Content,
                    TemplateId = input.EmailTemplateDto.Id
                });
            }
            return input;
        }
    }

}
