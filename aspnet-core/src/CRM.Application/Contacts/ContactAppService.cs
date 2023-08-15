using Abp.Authorization;
using Abp.UI;
using CRM.Contacts.Dto;
using CRM.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CRM.Paging;
using CRM.Extension;
using Abp.Net.Mail;
using static CRM.Enums.StatusEnum;
using System.Linq.Dynamic.Core;
using CRM.Uitls;

namespace CRM.Contacts
{
    [AbpAuthorize]
    public class ContactAppService : CRMAppServiceBase
    {
        private readonly IEmailSender _emailSender;
        [HttpPost]
        public async Task<SaveContactDto> Save(SaveContactDto input)
        {
            var isExisted = WorkScope.GetAll<Contact>().AsNoTracking()
                .Any(z => z.Mail == input.Mail || z.Phone == input.Phone);
            //create
            if (input.Id <= 0)
            {
                if (isExisted) { throw new UserFriendlyException(CommonUtils.ExistedInfor); }
                var item = new Contact
                {
                    Mail = input.Mail,
                    Name = input.Name,
                    Phone = input.Phone,
                    Role = input.Role,
                    Description = input.Description,
                    FirstName = input.FirstName
                };
                input.Id = await WorkScope.InsertAndGetIdAsync<Contact>(item);
            }
            //update
            else
            {
                var old = await WorkScope.GetAsync<Contact>(input.Id);
                var isExistExceptOld = WorkScope.GetAll<Contact>().AsNoTracking()
                    .Where(x => x.Id != old.Id)
                    .Any(x => x.Mail == input.Mail || x.Phone == input.Phone);
                if (old == null)
                {
                    throw new UserFriendlyException("Contact does not exist!!!");
                }
                if (isExistExceptOld)
                {
                    throw new UserFriendlyException(CommonUtils.ExistedInfor);
                }
                if (old.Name != input.Name)
                {
                    var NameOfEntity = WorkScope.GetAll<EntityAssignment>().Where(s => s.EntityId == input.Id && s.EntityType == EntityDefault.Contact);
                    foreach (var i in NameOfEntity)
                    {
                        i.NameOfEntity = input.Name;
                    }
                    await WorkScope.UpdateRangeAsync(NameOfEntity);
                }

                old.Mail = input.Mail;
                old.Name = input.Name;
                old.Phone = input.Phone;
                old.Role = input.Role;
                old.Description = input.Description;
                old.FirstName = input.FirstName;
                await WorkScope.UpdateAsync(old);
            }
            return input;
        }
        [HttpDelete]
        public async Task Delete(long id)
        {
            var count = await WorkScope.GetAll<ClientContact>().CountAsync(s => s.ContactId == id);
            var old = await WorkScope.GetAsync<Contact>(id);
            if (count > 0)
            {
                throw new UserFriendlyException("Contact has User data!!!");
            }
            if (old == null)
            {
                throw new UserFriendlyException("Contact does not exist!!!");
            }
            await WorkScope.DeleteAsync<Contact>(id);
        }

        [HttpGet]
        public async Task<ViewContactDto> GetById(long id)
        {
            return await (from c in WorkScope.GetAll<Contact>().Where(s => s.Id == id)
                          join cc in WorkScope.GetAll<ClientContact>() on c.Id equals cc.ContactId into t
                          select new ViewContactDto
                          {
                              Id = c.Id,
                              Mail = c.Mail,
                              Name = c.Name,
                              Description = c.Description,
                              Phone = c.Phone,
                              Role = c.Role,
                              Clients = t.Select(s => new ClientInContact
                              {
                                  Id = s.Client.Id,
                                  Country = s.Client.Country,
                                  Description = s.Client.Description,
                                  Mail = s.Client.Mail,
                                  Name = s.Client.Name,
                                  Phone = s.Client.Phone,
                                  Status = s.Client.Status,
                                  Type = s.Client.Type,
                                  Website = s.Client.Website
                              }).ToList()
                          }).FirstOrDefaultAsync();
        }
        [HttpPost]
        public async Task<GridResult<ViewContactDto>> GetAllPagging(GridParam input)
        {
            var query = WorkScope.GetAll<Contact>()
                .Select(s => new ViewContactDto
                {
                    Id = s.Id,
                    Mail = s.Mail,
                    Name = s.Name,
                    Description = s.Description,
                    Phone = s.Phone,
                    Role = s.Role
                });
            return await query.GetGridResult(query, input);
        }
        [HttpGet]
        public async Task<List<ViewContactDto>> GetByClient(long clientId)
        {
            return await (from co in WorkScope.GetAll<Contact>()
                          join ct in WorkScope.GetAll<ClientContact>().Where(s => s.ClientId == clientId) on co.Id equals ct.ContactId
                          select new ViewContactDto
                          {
                              Id = co.Id,
                              Mail = co.Mail,
                              Name = co.Name,
                              Description = co.Description,
                              Phone = co.Phone,
                              Role = co.Role
                          }).ToListAsync();
        }
        [HttpPost]
        public async Task<SaveClientContactDto> SaveClientContact(SaveClientContactDto input)
        {
            var client = await WorkScope.GetAsync<Client>(input.ClientId);
            if (client == null)
            {
                throw new UserFriendlyException("Client doesn't exist!!!");
            }
            var old = await WorkScope.GetAll<ClientContact>().Where(s => s.ClientId == input.ClientId)
                .Select(s => new { s.ContactId, s.Id }).ToDictionaryAsync(s => s.ContactId, s => s.Id);
            var needToAdd = input.Contacts.Except(old.Keys);
            var needToDelete = old.Keys.Except(input.Contacts);
            foreach (var contactId in needToAdd)
            {
                var item = new ClientContact
                {
                    ClientId = input.ClientId,
                    ContactId = contactId
                };
                await WorkScope.InsertAsync<ClientContact>(item);
            }
            foreach (var contactId in needToDelete)
            {
                await WorkScope.DeleteAsync<ClientContact>(old[contactId]);
            }
            return input;
        }
    }
}
