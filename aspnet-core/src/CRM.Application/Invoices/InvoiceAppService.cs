using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.UI;
using CRM.APIWorkflowController;
using CRM.Authorization.Users;
using CRM.Configuration;
using CRM.Entities;
using CRM.Invoices.Dto;
using CRM.WorkFlows;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static CRM.Enums.StatusEnum;

namespace CRM.Invoices
{
    public class InvoiceAppService : CRMAppServiceBase
    {
        private readonly WorkflowAppService _workflowAppService;
        private IHostingEnvironment _hostingEnvironment;
        private WorkflowControllerAppService _workflowControllerAppService;
        public InvoiceAppService(WorkflowAppService workflowAppService, IHostingEnvironment environment, WorkflowControllerAppService workflowControllerAppService)
        {
            _workflowAppService = workflowAppService;
            _hostingEnvironment = environment;
            _workflowControllerAppService = workflowControllerAppService;
        }
        public async Task<object> GetUserForInvoiceUser()
        {
            var emailBodyTemplate = await SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.EmailTemplate.InvoiceEmailHeader) ?? string.Empty;
            var users = WorkScope.GetAll<User>().Select(x => new { UserId = x.Id, UserName = x.Name }).ToList();
            return users;
        }

        public async Task<InvoiceDetailDto> GetInvoiceDetail(long id)
        {
            var invoice = await WorkScope.GetAsync<Invoice>(id);
            var contract = await WorkScope.GetAsync<Contract>(invoice.ContractId);
            var project = await WorkScope.GetAll<Project>().Include(x => x.Client).FirstOrDefaultAsync(p => p.Id == contract.ProjectId);
            var user = await WorkScope.GetAsync<User>(invoice.Assignee);
            var contractMilestone = WorkScope.GetAll<ContractMileStone>().FirstOrDefault(cm => invoice.ContractMileStoneId.HasValue && cm.Id == invoice.ContractMileStoneId.Value);
            var invoiceUser = await WorkScope.GetAll<InvoiceUser>().Where(iu => iu.InvoiceId == invoice.Id).ToListAsync();
            var invoiceDetail = new InvoiceDetailDto
            {
                Assignee = invoice.Assignee,
                AssigneeName = user.Name,
                ContractName = contract.Name,
                ContractId = contract.Id,
                Id = invoice.Id,
                Name = invoice.Name,
                Currency = invoice.Currency,
                CustomerId = project.ClientId,
                CustomerName = project.Client.Name,
                InvoiceDate = invoice.InvoiceDate,
                Order = invoice.Order,
                ProjectId = project.Id,
                ProjectName = project.Name,
                Status = invoice.Status,
                Type = invoice.Type,
                Value = invoice.Value,
                ChangeFromFailToChasing = invoice.Status == Enums.StatusEnum.InvoiceStatus.Fail ? true : false,
                ChangeFromPaidToChasing = invoice.Status == Enums.StatusEnum.InvoiceStatus.Paid ? true : false,
                ChangeFromChasing = invoice.Status == Enums.StatusEnum.InvoiceStatus.Chasing ? true : false,
                ChangeFromPendingToChasing = invoice.Status == Enums.StatusEnum.InvoiceStatus.Pending ? true : false,
                MileStoneDetail = new InvoiceMileStoneDetailDto { },

            };
            if (contractMilestone != null)
            {
                invoiceDetail.MileStoneDetail.MileStoneId = contractMilestone.Id;
                invoiceDetail.MileStoneDetail.MileStoneDescription = contractMilestone.Description;
                invoiceDetail.MileStoneDetail.MileStoneName = contractMilestone.Name;
                invoiceDetail.MileStoneDetail.MilestoneTime = contractMilestone.MileStoneDate;
                invoiceDetail.MileStoneDetail.Percentage = contractMilestone.Percentage;
            }
            if (contractMilestone == null)
            {
                invoiceDetail.InvoiceUserDetail = (from iu in invoiceUser
                                                   select new InvoiceUserDetailDto
                                                   {
                                                       InvoiceId = iu.InvoiceId,
                                                       ManMonth = iu.ManMonth,
                                                       Position = iu.Position,
                                                       Rate = iu.Rate,
                                                       UserId = iu.UserId,
                                                       Id = iu.Id
                                                   }).ToList();
            }
            return invoiceDetail;
        }

        public async Task SavePeopleInCharge(PeopleInChargeInput input)
        {
            var currentInvoiceUsers = await WorkScope.GetAll<InvoiceUser>().Include(iu => iu.Invoice).Where(iu => iu.InvoiceId == input.InvoiceId).ToListAsync();
            var allInvoice = await WorkScope.GetAll<Invoice>().ToListAsync();
            var currentInvoice = allInvoice.FirstOrDefault(i => i.Id == input.InvoiceId);
            var currentInvoiceHasSameContract = allInvoice.Where(i => i.ContractId == currentInvoice.ContractId);
            var currentInvoiceHasSameContractIds = currentInvoiceHasSameContract.Select(x => x.Id);
            var invoiceForInsert = input.People.Where(x => x.InvoiceUserId == 0);
            var invoiceFofUpdate = input.People.Where(x => currentInvoiceUsers.Select(iu => iu.Id).Contains(x.InvoiceUserId));
            var invoiceForDelete = currentInvoiceUsers.Where(c => !input.People.Select(p => p.InvoiceUserId).Contains(c.Id));
            foreach (var insert in invoiceForInsert)
            {
                if (insert.IsAllMonth == false)
                {
                    await WorkScope.InsertAsync<InvoiceUser>(new InvoiceUser
                    {
                        InvoiceId = input.InvoiceId,
                        ManMonth = insert.ManMonth,
                        Position = insert.Position,
                        Rate = insert.Rate,
                        UserId = insert.UserId
                    });
                }
                else
                {
                    foreach (var id in currentInvoiceHasSameContractIds)
                    {
                        await WorkScope.InsertAsync<InvoiceUser>(new InvoiceUser
                        {
                            InvoiceId = id,
                            ManMonth = insert.ManMonth,
                            Position = insert.Position,
                            Rate = insert.Rate,
                            UserId = insert.UserId
                        });
                    }
                }
            }
            await CurrentUnitOfWork.SaveChangesAsync();

            foreach (var update in invoiceFofUpdate)
            {
                var currentInvoiceUser = currentInvoiceUsers.FirstOrDefault(x => x.Id == update.InvoiceUserId);
                currentInvoiceUser.ManMonth = update.ManMonth;
                currentInvoiceUser.Position = update.Position;
                currentInvoiceUser.Rate = update.Rate;
                currentInvoiceUser.UserId = update.UserId;
            }

            foreach (var del in invoiceForDelete)
            {
                var currentInvoiceUser = currentInvoiceUsers.FirstOrDefault(x => x.Id == del.Id);
                await WorkScope.DeleteAsync(currentInvoiceUser);
            }
        }

        [AbpAuthorize, HttpPost]
        public async Task<List<InvoiceOutPut>> GetAllInvoices(InvoiceInput input)
        {
            var project = WorkScope.GetAll<Project>().Include(x => x.Client);
            var contract = WorkScope.GetAll<Contract>();
            var invoice = WorkScope.GetAll<Invoice>();
            var users = WorkScope.GetAll<User>();
            var workflowTransitions = await _workflowAppService.GetWorkflowTransition((int)EntityDefault.Invoice);
            var invoices = (from p in project
                            join c in contract on p.Id equals c.ProjectId
                            join i in invoice on c.Id equals i.ContractId
                            join u in users on i.Assignee equals u.Id
                            select new InvoiceOutPut
                            {
                                Id = i.Id,
                                Assigne = i.Assignee,
                                AssigneeName = u.Name,
                                InvoiceName = i.Name,
                                ClientId = p.ClientId,
                                ClientName = p.Client.Name,
                                ProjectId = p.Id,
                                ProjectName = p.Name,
                                Status = i.Status,
                                Time = i.InvoiceDate,
                                Type = i.Type,
                                TypeName = ((Enums.StatusEnum.ProjectTypeList)i.Type).ToString(),
                                ChangeFromPaidToChasing = i.Status == Enums.StatusEnum.InvoiceStatus.Paid
                                                          && workflowTransitions.Any(wt => wt.FromStatus == (int)i.Status && wt.ToStatus == (int)Enums.StatusEnum.InvoiceStatus.Chasing && wt.CanChange),
                                ChangeFromChasing = i.Status == Enums.StatusEnum.InvoiceStatus.Chasing ? true : false,
                                ChangeFromPendingToChasing = i.Status == Enums.StatusEnum.InvoiceStatus.Pending
                                                          && workflowTransitions.Any(wt => wt.FromStatus == (int)i.Status && wt.ToStatus == (int)Enums.StatusEnum.InvoiceStatus.Chasing && wt.CanChange),
                                ChangeFromChasingToPending = i.Status == Enums.StatusEnum.InvoiceStatus.Chasing
                                                          && workflowTransitions.Any(wt => wt.FromStatus == (int)i.Status && wt.ToStatus == (int)Enums.StatusEnum.InvoiceStatus.Pending && wt.CanChange),
                                ChangeFromChasingToFail = i.Status == Enums.StatusEnum.InvoiceStatus.Chasing
                                                          && workflowTransitions.Any(wt => wt.FromStatus == (int)i.Status && wt.ToStatus == (int)Enums.StatusEnum.InvoiceStatus.Fail && wt.CanChange),
                                ChangeFromChasingToPaid = i.Status == Enums.StatusEnum.InvoiceStatus.Chasing
                                                          && workflowTransitions.Any(wt => wt.FromStatus == (int)i.Status && wt.ToStatus == (int)Enums.StatusEnum.InvoiceStatus.Paid && wt.CanChange)
                            })
                            .WhereIf(input.Assigne.HasValue, x => x.Assigne == input.Assigne)
                            .WhereIf(input.EndTime.HasValue, x => x.Time <= input.EndTime)
                            .WhereIf(input.StartTime.HasValue, x => x.Time >= input.StartTime)
                            .WhereIf(input.ProjectId.HasValue, x => x.ProjectId == input.ProjectId)
                            .WhereIf(input.Status.HasValue, x => x.Status == input.Status)
                            .WhereIf(input.Type.HasValue, x => x.Type == input.Type)
                            .ToList();
            return invoices;
        }

        [HttpPost]
        public async Task<InvoiceDetailDto> Save(InvoiceInfomationInput input)
        {
            if (input.Id > 0)
            {//Edit
                var currentInvoice = WorkScope.Get<Invoice>(input.Id);
                if (currentInvoice == null)
                {
                    throw new UserFriendlyException("Không tìm thấy Invoice");
                }
                if(currentInvoice.Name != input.InvoiceName)
                {
                    var NameOfEntity = WorkScope.GetAll<EntityAssignment>().Where(s => s.EntityId == input.Id && s.EntityType == EntityDefault.Invoice);
                    foreach(var i in NameOfEntity)
                    {
                        i.NameOfEntity = input.InvoiceName;
                    }
                    await WorkScope.UpdateRangeAsync(NameOfEntity);
                }
                currentInvoice.Assignee = input.Assignee;
                //if (currentInvoice.Status == Enums.StatusEnum.InvoiceStatus.Fail && input.Status == Enums.StatusEnum.InvoiceStatus.Chasing)
                //{
                //    currentInvoice.Status = input.Status;
                //}
                //else if (currentInvoice.Status == Enums.StatusEnum.InvoiceStatus.Paid && input.Status == Enums.StatusEnum.InvoiceStatus.Chasing)
                //{
                //    currentInvoice.Status = input.Status;
                //}
                //else if (currentInvoice.Status == Enums.StatusEnum.InvoiceStatus.Pending && input.Status == Enums.StatusEnum.InvoiceStatus.Chasing)
                //{
                //    currentInvoice.Status = input.Status;
                //}
                //else if (currentInvoice.Status == Enums.StatusEnum.InvoiceStatus.Chasing)
                //{
                //    currentInvoice.Status = input.Status;
                //}
                currentInvoice.Name = input.InvoiceName;
                currentInvoice.Type = input.InvoiceType;
                _workflowControllerAppService.ChangeStatusInvoice(input.Id, input.Status);
                await CurrentUnitOfWork.SaveChangesAsync();
                var invoice = await GetInvoiceDetail(input.Id);
                
                return invoice;
            }
            else
            {
                var invoice = ObjectMapper.Map<Invoice>(input);
                invoice.Type = input.InvoiceType;
                invoice.Name = input.InvoiceName;
                invoice.InvoiceDate = input.Time;
                var invoiceId = await WorkScope.GetRepo<Invoice>().InsertAndGetIdAsync(invoice);
                var currentInvoice = await WorkScope.GetAsync<Invoice>(invoiceId);
                if (currentInvoice == null)
                {
                    throw new UserFriendlyException("Không thành công");
                }
                var invoiceDetail = await GetInvoiceDetail(invoiceId);
                return invoiceDetail;
            }
        }

        [HttpPost]
        public async Task ChangeInvoiceStatus(InvoiceStatusInput input)
        {
            var currentInvoice = WorkScope.Get<Invoice>(input.Id);
            if (currentInvoice.Status == Enums.StatusEnum.InvoiceStatus.Fail && input.Status == Enums.StatusEnum.InvoiceStatus.Chasing)
            {
                currentInvoice.Status = input.Status;
            }
            else if (currentInvoice.Status == Enums.StatusEnum.InvoiceStatus.Paid && input.Status == Enums.StatusEnum.InvoiceStatus.Chasing)
            {
                currentInvoice.Status = input.Status;
            }
            else if (currentInvoice.Status == Enums.StatusEnum.InvoiceStatus.Pending && input.Status == Enums.StatusEnum.InvoiceStatus.Chasing)
            {
                currentInvoice.Status = input.Status;
            }
            else if (currentInvoice.Status == Enums.StatusEnum.InvoiceStatus.Chasing)
            {
                currentInvoice.Status = input.Status;
            }
            else
            {
                throw new UserFriendlyException("Sai trạng thái");
            }
            await CurrentUnitOfWork.SaveChangesAsync();
        }
        [HttpPost]
        public async Task<ViewInvoiceFileDto> AddFile([FromForm]SaveInvoiceFileDto input)
        {
            var existInvoice = await WorkScope.GetAll<Invoice>().AnyAsync(s => s.Id == input.InvoiceId);
            if (!existInvoice)
            {
                throw new UserFriendlyException("Invoice do not exist!!!");
            }
            else
            {
                if (input != null && input.File != null && input.File.Length > 0)
                {
                    var fileUrl = input.File.FileName;
                    var tempPath = Path.Combine(_hostingEnvironment.WebRootPath, "invoice_file");
                    if (!Directory.Exists(tempPath))
                    {
                        Directory.CreateDirectory(tempPath);
                    }
                    using (var stream = File.Create(Path.Combine(_hostingEnvironment.WebRootPath, "invoice_file", fileUrl)))
                    {
                        await input.File.CopyToAsync(stream);
                        input.Id = await WorkScope.InsertAndGetIdAsync<InvoiceFile>(new InvoiceFile
                        {
                            InvoiceId = input.InvoiceId,
                            Type = input.Type,
                            FileUrl = fileUrl
                        });
                    }
                    return new ViewInvoiceFileDto
                    {
                        Id = input.Id,
                        Type = input.Type,
                        FileUrl = $"invoice_file/{fileUrl}",
                        FileName = fileUrl,
                        TypeString = Enum.GetName(typeof(InvoiceFileType), input.Type)
                    };
                }
                else
                {
                    throw new UserFriendlyException("File do not include!!!");
                }
            }
        }
        [HttpDelete]
        public async Task Delete(long id)
        {
            var old = await WorkScope.GetAsync<Invoice>(id);
            if (old != null)
            {
                await WorkScope.DeleteAsync<Invoice>(id);
                var invoiceFiles = await WorkScope.GetAll<InvoiceFile>().Where(s => s.InvoiceId == id).ToListAsync();
                foreach (var invoiceFile in invoiceFiles)
                {
                    //delete in db
                    await WorkScope.DeleteAsync<InvoiceFile>(id);
                    //delete in storage
                    var path = Path.Combine(_hostingEnvironment.WebRootPath, "invoice_file", invoiceFile.FileUrl);
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }
            }
            else
            {
                throw new UserFriendlyException("Invoice do not exist!!!");
            }
        }
        [HttpDelete]
        public async Task DeleteFile(long id)
        {
            var invoiceFile = await WorkScope.GetAsync<InvoiceFile>(id);
            if (invoiceFile != null)
            {
                //delete in db
                await WorkScope.DeleteAsync<InvoiceFile>(id);
                //delete in storage
                var path = Path.Combine(_hostingEnvironment.WebRootPath, "invoice_file", invoiceFile.FileUrl);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            else
            {
                throw new UserFriendlyException("File do not exist!!!");
            }
        }
        [HttpPost]
        public async Task<List<ViewInvoiceFileDto>> GetFileByInvoice(long invoiceId)
        {
            return await WorkScope.GetAll<InvoiceFile>().Where(s => s.InvoiceId == invoiceId)
                .Select(s => new ViewInvoiceFileDto
                {
                    Id = s.Id,
                    Type = s.Type,
                    FileUrl = $"invoice_file/{s.FileUrl}",
                    FileName = s.FileUrl,
                    TypeString = Enum.GetName(typeof(InvoiceFileType), s.Type)
                }).ToListAsync();
        }
    }
}
