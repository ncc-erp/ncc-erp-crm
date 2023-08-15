using Abp.Authorization;
using CRM.CampaignContacts.Dto;
using CRM.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Abp.Application.Services.Dto;
using Abp.UI;
using CRM.Extension;
using CRM.Paging;

namespace CRM.CampaignContacts
{
    [AbpAuthorize]
    public class CampaignContactAppService : CRMAppServiceBase
    {
        [HttpPost]
        public async Task<CreateCampaignContactDto> Save(CreateCampaignContactDto input)
        {
            if (input.Id <= 0)
            {
                var campaign = new Campaign
                {
                    Name = input.Name
                };
                input.Id = await WorkScope.InsertAndGetIdAsync<Campaign>(campaign);
                foreach (var c in input.ContactId.Distinct())
                {
                    var camcon = new CampaignContact
                    {
                        ContactId = c,
                        CampaignId = input.Id,
                        Status = Enums.StatusEnum.CampaignContactStatus.Unsent
                    };
                    var isExist = await WorkScope.GetAll<CampaignContact>().AnyAsync(s => s.CampaignId == input.Id && s.ContactId == c);
                    if (!isExist)
                    {
                        await WorkScope.InsertAsync<CampaignContact>(camcon);
                    }
                }
            }
            else
            {
                var campaignOld = await WorkScope.GetAsync<Campaign>(input.Id);
                campaignOld.Name = input.Name;
                await WorkScope.UpdateAsync<Campaign>(campaignOld);
                var campaignContact = await WorkScope.GetAll<CampaignContact>().Where(s => s.CampaignId == input.Id).ToListAsync();
                var contactNew = input.ContactId.Except(campaignContact.Select(s => s.ContactId));
                var contactDelete = campaignContact.Select(s => s.ContactId).Except(input.ContactId);
                //insert
                foreach (var c in contactNew.Distinct())
                {
                    var camcon = new CampaignContact
                    {
                        ContactId = c,
                        CampaignId = input.Id,
                        Status = Enums.StatusEnum.CampaignContactStatus.Unsent
                    };
                    await WorkScope.InsertAsync<CampaignContact>(camcon);
                    var isExist = await WorkScope.GetAll<CampaignContact>().AnyAsync(s => s.CampaignId == input.Id && s.ContactId == c);
                    if (!isExist)
                    {
                        await WorkScope.InsertAsync<CampaignContact>(camcon);
                    }
                }

                //delete
                var listDelete = campaignContact.Where(s => contactDelete.Contains(s.ContactId));
                foreach (var c in listDelete)
                {
                    await WorkScope.DeleteAsync<CampaignContact>(c.Id);
                }
            }
            return input;
        }
        [HttpDelete]
        public async System.Threading.Tasks.Task Delete(EntityDto<long> input)
        {
            var isExist = await WorkScope.GetAll<Campaign>().AnyAsync(s => s.Id == input.Id);
            if (!isExist)
            {
                throw new UserFriendlyException(string.Format("Campaign Id = {0} isn't exist", input.Id));
            }
            await WorkScope.DeleteAsync<Campaign>(input.Id);
            var campaignContact = await WorkScope.GetAll<CampaignContact>().Where(s => s.CampaignId == input.Id).ToListAsync();
            foreach (var cam in campaignContact)
            {
                await WorkScope.DeleteAsync<CampaignContact>(cam.Id);
            }
        }
        [HttpGet]
        public async Task<List<GetCampaignContactDto>> GetAll()
        {
            var query = await (WorkScope.GetAll<CampaignContact>().Select(s => new
            {
                CampaignName = s.Campaign.Name,
                CampaignId = s.CampaignId,
                ContactName = s.Contact.Name,
                ContactMail = s.Contact.Mail,
                ContactId = s.ContactId,
                s.Status
            }).GroupBy(s => new { s.CampaignId, s.CampaignName })
            .Select(s => new GetCampaignContactDto
            {
                Id = s.Key.CampaignId,
                CampaignName = s.Key.CampaignName,
                Contacts = s.Select(v => new GetContactDto
                {
                    Id = v.ContactId,
                    ContactEmail = v.ContactMail,
                    ContactName = v.ContactName,
                    Status = v.Status
                })
            })).ToListAsync();
            /*foreach (var q in query)
            {
                var emailTemplate = await WorkScope.GetAll<CampaignEmailTemplate>().Where(s => s.CampaignId == q.Id).LastOrDefaultAsync();
                if(emailTemplate != null)
                {
                    var email = new GetEmailTemplateDto
                    {
                        Content = emailTemplate.Content,
                        Subject = emailTemplate.Subject,
                    };
                    q.MailTemplate = email;
                }
            }*/
            return query;

        }
        [HttpGet]
        public async Task<GetCampaignContactDto> Get(long campaignId)
        {
            var isExist = await WorkScope.GetAll<Campaign>().AnyAsync(s => s.Id == campaignId);
            if (!isExist)
            {
                throw new UserFriendlyException(string.Format("Campaign Id = {0} isn't exist", campaignId));
            }
            var query =  await (WorkScope.GetAll<CampaignContact>().Where(s => s.CampaignId == campaignId).Select(s => new
            {
                CampaignName = s.Campaign.Name,
                CampaignId = s.CampaignId,
                ContactName = s.Contact.Name,
                ContactMail = s.Contact.Mail,
                ContactId = s.ContactId,
                s.Status
            }).GroupBy(s => new { s.CampaignId, s.CampaignName })
            .Select(s => new GetCampaignContactDto
            {
                Id = s.Key.CampaignId,
                CampaignName = s.Key.CampaignName,
                Contacts = s.Select(v => new GetContactDto
                {
                    Id = v.ContactId,
                    ContactEmail = v.ContactMail,
                    ContactName = v.ContactName,
                    Status = v.Status
                })
            })).FirstOrDefaultAsync();

            /*var emailTemplate = await WorkScope.GetAll<CampaignEmailTemplate>().Where(s => s.CampaignId == query.Id).LastOrDefaultAsync();
            if (emailTemplate != null)
            {
                var email = new GetEmailTemplateDto
                {
                    Content = emailTemplate.Content,
                    Subject = emailTemplate.Subject,
                };
                query.MailTemplate = email;
            }*/
            var emailTemplates = WorkScope
                .GetAll<CampaignEmailTemplate>()
                .Where(s => s.CampaignId == query.Id)
                .OrderByDescending(x => x.CreationTime)
                .ThenByDescending(x => x.LastModificationTime)
                .Select(x => new GetEmailTemplatesDto
                {
                    Id = x.TemplateId.Value,
                    Content = x.Content,
                    Subject = x.Subject,
                }).ToList();
            if (emailTemplates != null) query.MailTemplates = emailTemplates;
            return query;
        }

        [HttpPost]
        public async Task<GridResult<GetCampaignContactDto>> GetAllPaging(GridParam input)
        {
            var campaignContacts = WorkScope.GetAll<CampaignContact>().Select(s => new
            {
                CampaignName = s.Campaign.Name,
                CampaignId = s.CampaignId,
                ContactName = s.Contact.Name,
                ContactMail = s.Contact.Mail,
                ContactId = s.ContactId,
                s.Status
            })
                .GroupBy(s => new { s.CampaignId, s.CampaignName })
                .Select(s => new GetCampaignContactDto
                {
                    Id = s.Key.CampaignId,
                    CampaignName = s.Key.CampaignName,
                    Contacts = s.Select(v => new GetContactDto
                    {
                        Id = v.ContactId,
                        ContactEmail = v.ContactMail,
                        ContactName = v.ContactName,
                        Status = v.Status
                    })
                });
            return await campaignContacts.GetGridResult(campaignContacts, input);
        }
    }
}
