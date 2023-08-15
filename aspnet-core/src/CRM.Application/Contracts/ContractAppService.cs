using Abp.UI;
using CRM.Contracts.Dto;
using CRM.Entities;
using CRM.Paging;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using CRM.Extension;
using Microsoft.EntityFrameworkCore;
using CRM.ContractMileStones.Dto;
using CRM.Enums;
using CRM.Authorization.Users;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using static CRM.Enums.StatusEnum;

namespace CRM.Contracts
{
    public class ContractAppService : CRMAppServiceBase
    {
        private IHostingEnvironment _hostingEnvironment;
        public ContractAppService(IHostingEnvironment environment)
        {
            _hostingEnvironment = environment;
        }
        [HttpPost]
        public async Task<SaveContractDto> Save(SaveContractDto input)
        {
            var allInvoice = WorkScope.GetAll<Invoice>();
            long lastInvoiceOrder = 0;
            var currentProject = WorkScope.Get<Project>(input.ProjectId);
            if (input.EndTime < DateTime.Now || input.StartTime > input.EndTime)
            {
                throw new UserFriendlyException("Ngày kết thúc phải lớn hơn ngày hiện tại hoặc ngày bắt đầu");
            }
            //tạo mới
            if (input.Id <= 0)
            {
                input.Id = await WorkScope.InsertAndGetIdAsync<Contract>(new Contract
                {
                    ContractValue = input.ContractValue,
                    Currency = input.ContractCurrency,
                    EndTime = input.EndTime,
                    Name = input.Name,
                    ProjectId = input.ProjectId,
                    StartTime = input.StartTime,
                    Status = input.Status,
                    Type = input.Type,
                });
                await CurrentUnitOfWork.SaveChangesAsync();

                var invoiceCount = allInvoice.Where(i => i.ContractId == input.Id).Count();

                var lastInvoice = allInvoice.LastOrDefault(i => i.ContractId == input.Id);
                if (lastInvoice != default)
                {
                    lastInvoiceOrder = lastInvoice.Order;
                }

                if (input.Type == StatusEnum.ContractType.FixedPrice)
                {
                    foreach (var milestone in input.MileStones)
                    {
                        var contractMilestoneId = await WorkScope.InsertAndGetIdAsync<ContractMileStone>(new ContractMileStone
                        {
                            ContractId = input.Id,
                            Currency = input.ContractCurrency,
                            MileStoneDate = milestone.MileStoneDate.Value,
                            Description = milestone.Description,
                            Value = milestone.MileStoneValue.Value,
                            Name = milestone.Name,
                            Percentage = milestone.Percentage.Value,
                        });
                        await CurrentUnitOfWork.SaveChangesAsync();
                        invoiceCount++;
                        await WorkScope.InsertAsync<Invoice>(new Invoice
                        {
                            Assignee = AbpSession.UserId.Value,
                            ContractId = input.Id,
                            InvoiceDate = milestone.MileStoneDate.Value,
                            Currency = input.ContractCurrency,
                            Value = Convert.ToSingle(input.ContractValue * milestone.Percentage.Value),
                            ContractMileStoneId = contractMilestoneId,
                            Name = $"{currentProject.Name + milestone.Name + invoiceCount + DateTime.Now.Date + invoiceCount}",
                            Order = 0,
                            Status = StatusEnum.InvoiceStatus.Pending,
                            Type = input.Type,
                        });
                        await CurrentUnitOfWork.SaveChangesAsync();
                    }
                }

                if ((input.Type == StatusEnum.ContractType.ODC || input.Type == StatusEnum.ContractType.TNM))
                {
                    var now = DateTime.Now;
                    var lastdayInmonth = now.AddDays(-(now.Day)).AddMonths(1);
                    invoiceCount++;
                    await WorkScope.InsertAsync<Invoice>(new Invoice
                    {
                        Assignee = AbpSession.UserId.Value,
                        ContractId = input.Id,
                        InvoiceDate = lastdayInmonth,
                        Currency = input.ContractCurrency,
                        ContractMileStoneId = null,
                        Name = $"{currentProject.Name + invoiceCount + DateTime.Now.Date + invoiceCount}",
                        Order = lastInvoiceOrder + 1,
                        Status = StatusEnum.InvoiceStatus.Pending,
                        Type = input.Type,
                    });

                }
            }
            //update
            else
            {

                var allMileStone = WorkScope.GetAll<ContractMileStone>();
                var currentContract = await WorkScope.GetAsync<Contract>(input.Id);
                if (currentContract.Name != input.Name)
                {
                    var NameOfEntity = WorkScope.GetAll<EntityAssignment>().Where(s => s.EntityId == input.Id && s.EntityType == EntityDefault.Contract);
                    foreach (var i in NameOfEntity)
                    {
                        i.NameOfEntity = input.Name;
                    }
                    await WorkScope.UpdateRangeAsync(NameOfEntity);
                }
                if (currentContract.Status == StatusEnum.ContractStatus.Finish)
                {
                    currentContract.ContractValue = input.ContractValue;
                }
                currentContract.Currency = input.ContractCurrency;
                currentContract.EndTime = input.EndTime;
                currentContract.Name = input.Name;
                currentContract.ProjectId = input.ProjectId;
                currentContract.StartTime = input.StartTime;
                currentContract.Status = input.Status;
                currentContract.Type = input.Type;
                await CurrentUnitOfWork.SaveChangesAsync();
                var currentMileStones = await WorkScope.GetAll<ContractMileStone>().Where(s => s.ContractId == input.Id).ToListAsync();
                var mileStoneNeedToAdd = input.MileStones.Where(s => s.Id == 0);
                var invoiceCount = allInvoice.Where(i => i.ContractId == input.Id).Count();
                if (input.Type == StatusEnum.ContractType.FixedPrice)
                {
                    foreach (var add in mileStoneNeedToAdd)
                    {
                        var contractMilestoneId = await WorkScope.InsertAndGetIdAsync<ContractMileStone>(new ContractMileStone
                        {
                            ContractId = input.Id,
                            Currency = input.ContractCurrency,
                            MileStoneDate = add.MileStoneDate.Value,
                            Description = add.Description,
                            Value = add.MileStoneValue.Value,
                            Name = add.Name,
                            Percentage = add.Percentage.Value,
                        });
                        await CurrentUnitOfWork.SaveChangesAsync();
                        invoiceCount++;
                        await WorkScope.InsertAsync<Invoice>(new Invoice
                        { 
                            Assignee = AbpSession.UserId.Value,
                            ContractId = input.Id,
                            InvoiceDate = add.MileStoneDate.Value,
                            Currency = input.ContractCurrency,
                            ContractMileStoneId = contractMilestoneId,
                            Name = $"{currentProject.Name + add.Name + invoiceCount + DateTime.Now.Date + invoiceCount}",
                            Order = 0,
                            Status = StatusEnum.InvoiceStatus.Pending,
                            Type = input.Type,
                        });
                        await CurrentUnitOfWork.SaveChangesAsync();
                    }
                    var currentMileStoneId = currentMileStones.Select(x => x.Id);
                    var inputUpdateMileStoneId = input.MileStones.Where(x => x.Id.HasValue && x.Id.Value > 0).Select(x => x.Id.Value);
                    var mileStoneNeedUpdate = currentMileStoneId.Intersect(inputUpdateMileStoneId);
                    foreach (var updateId in mileStoneNeedUpdate)
                    {
                        var currentInvoiceOfMileStone = allInvoice.FirstOrDefault(i => i.ContractMileStoneId == updateId);
                        if (currentInvoiceOfMileStone.Status == StatusEnum.InvoiceStatus.Paid)
                        {
                            throw new UserFriendlyException("Hóa đơn đã được thanh toán không thể chỉnh sửa");
                        }
                        var mileStoneforUpdate = allMileStone.FirstOrDefault(x => x.Id == updateId);
                        if (mileStoneforUpdate == default)
                        {
                            throw new UserFriendlyException("Không tìm thấy MileStone");
                        }
                        foreach (var milestone in input.MileStones.Where(x => x.Id == updateId))
                        {
                            if (currentContract.Status != StatusEnum.ContractStatus.Finish)
                            {
                                mileStoneforUpdate.ContractId = input.Id;
                                mileStoneforUpdate.Currency = input.ContractCurrency;
                                mileStoneforUpdate.MileStoneDate = milestone.MileStoneDate.Value;
                                mileStoneforUpdate.Description = milestone.Description;
                                mileStoneforUpdate.Value = milestone.MileStoneValue.Value;
                                mileStoneforUpdate.Name = milestone.Name;
                                mileStoneforUpdate.Percentage = milestone.Percentage.Value;
                            }
                            else
                            {
                                throw new UserFriendlyException("Không được chỉnh sửa Milestone dã thanh toán");
                            }
                        }
                        await CurrentUnitOfWork.SaveChangesAsync();
                    }
                    // delete old milestone
                    var mileStoneNeedToDelete = currentMileStones.Where(x => !inputUpdateMileStoneId.Contains(x.Id));
                    foreach (var deleteMilestone in mileStoneNeedToDelete)
                    {
                        await WorkScope.DeleteAsync<ContractMileStone>(deleteMilestone);
                    }
                }

                if ((input.Type == StatusEnum.ContractType.ODC || input.Type == StatusEnum.ContractType.TNM))
                {
                    var now = DateTime.Now;
                    var lastdayInmonth = now.AddDays(-(now.Day)).AddMonths(1);
                    var lastInvoice = allInvoice.LastOrDefault(i => i.ContractId == input.Id);
                    if (lastInvoice != default)
                    {
                        lastInvoiceOrder = lastInvoice.Order;
                    }
                    invoiceCount++;
                    if (lastInvoice == null)
                    {
                        await WorkScope.InsertAsync<Invoice>(new Invoice
                        {
                            Assignee = AbpSession.UserId.Value,
                            ContractId = input.Id,
                            InvoiceDate = lastdayInmonth,
                            Currency = input.ContractCurrency,
                            ContractMileStoneId = null,
                            Name = $"{currentProject.Name + invoiceCount + DateTime.Now.Date + invoiceCount}",
                            Order = lastInvoiceOrder + 1,
                            Status = StatusEnum.InvoiceStatus.Pending,
                            Type = input.Type,
                        });
                    }
                    else if (lastInvoice != null)
                    {
                        if (lastInvoice.InvoiceDate < input.EndTime)
                        {
                            if (lastdayInmonth.Month > lastInvoice.InvoiceDate.Month)
                            {
                                await WorkScope.InsertAsync<Invoice>(new Invoice
                                {
                                    Assignee = AbpSession.UserId.Value,
                                    ContractId = input.Id,
                                    InvoiceDate = lastdayInmonth,
                                    Currency = input.ContractCurrency,
                                    ContractMileStoneId = null,
                                    Name = $"{currentProject.Name + invoiceCount + DateTime.Now.Date + invoiceCount}",
                                    Order = lastInvoiceOrder + 1,
                                    Status = StatusEnum.InvoiceStatus.Pending,
                                    Type = input.Type,
                                });
                            }
                        }
                    }
                    if (input.EndTime != null)
                    {
                        var endTime = (DateTime)input.EndTime;
                        if (lastInvoice.InvoiceDate > input.EndTime)
                        {
                            var currentInvoiceByDate = allInvoice.FirstOrDefault(i => i.InvoiceDate == lastdayInmonth);
                            if (currentInvoiceByDate.InvoiceDate.Month > endTime.Month)
                            {
                                if (currentInvoiceByDate.Status != StatusEnum.InvoiceStatus.Paid)
                                {
                                    await WorkScope.DeleteAsync(currentInvoiceByDate);
                                }
                            }
                        }
                    }
                }

            }
            return input;
        }


        [HttpDelete]
        public async Task Delete(long id)
        {
            var count = await WorkScope.GetAll<Invoice>().CountAsync(s => s.ContractId == id);
            var old = await WorkScope.GetAsync<Contract>(id);
            if (count > 0)
            {
                throw new UserFriendlyException(String.Format("Contract {0} has Invoice data!!!", old.Name));
            }           
            if (old != null)
            {
                await WorkScope.DeleteAsync<Contract>(id);
                var listFile = await WorkScope.GetAll<ContractFile>().Where(s => s.ContractId == id).ToListAsync();
                foreach (var deleteFile in listFile)
                {
                    //delete in db
                    await WorkScope.DeleteAsync<ContractFile>(deleteFile.Id);
                    //delete in storage
                    var path = Path.Combine(_hostingEnvironment.WebRootPath, "contract_file", deleteFile.FileUrl);
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }
                var listContractMileStone = await WorkScope.GetAll<ContractMileStone>().Where(s => s.ContractId == id)
                    .Select(s => s.Id).ToListAsync();
                foreach (var deleteId in listContractMileStone)
                {
                    await WorkScope.DeleteAsync<ContractMileStone>(deleteId);
                }
                if (old.Type == StatusEnum.ContractType.FixedPrice)
                {
                    var mileStone = WorkScope.GetAll<ContractMileStone>().Where(x => x.ContractId == id);
                    foreach (var item in mileStone)
                    {
                        await WorkScope.DeleteAsync(item);
                    }
                }
                if (old.Type == StatusEnum.ContractType.ODC || old.Type == StatusEnum.ContractType.TNM)
                {
                    var invoice = WorkScope.GetAll<ContractMileStone>().Where(x => x.ContractId == id);
                    foreach (var item in invoice)
                    {
                        await WorkScope.DeleteAsync(item);
                    }
                }
            }
            else
            {
                throw new UserFriendlyException(String.Format("Contract %s not found", old.Name));
            }
        }
        [HttpGet]
        public async Task<ViewContractDto> GetById(long id)
        {
            return await (from c in WorkScope.GetAll<Contract>().Where(s => s.Id == id)
                          join p in WorkScope.GetAll<Project>() on c.ProjectId equals p.Id
                          join cl in WorkScope.GetAll<Client>() on p.ClientId equals cl.Id
                          join m in WorkScope.GetAll<ContractMileStone>() on c.Id equals m.ContractId into t
                          select new ViewContractDto
                          {
                              Id = c.Id,
                              ClientId = cl.Id,
                              ClientName = cl.Name,
                              ProjectId = p.Id,
                              ProjectName = p.Name,
                              Name = c.Name,
                              StartTime = c.StartTime,
                              EndTime = c.EndTime,
                              Status = c.Status,
                              Type = c.Type,
                              ContractValue = c.ContractValue,
                              MileStones = t.Select(s => new MileStoneDto
                              {
                                  Id = s.Id,
                                  Name = s.Name,
                                  MileStoneDate = s.MileStoneDate
                              }).ToList()
                          }).FirstOrDefaultAsync();
        }
        [HttpPost]
        public async Task<GridResult<ViewContractDto>> GetAllPaging(GridParam input)
        {
            var contracts = from c in WorkScope.GetAll<Contract>()
                            join p in WorkScope.GetAll<Project>() on c.ProjectId equals p.Id
                            join cl in WorkScope.GetAll<Client>() on p.ClientId equals cl.Id
                            select new ViewContractDto
                            {
                                Id = c.Id,
                                ClientId = cl.Id,
                                ClientName = cl.Name,
                                ProjectId = p.Id,
                                ProjectName = p.Name,
                                Name = c.Name,
                                StartTime = c.StartTime,
                                EndTime = c.EndTime,
                                Status = c.Status,
                                Type = c.Type,
                                ContractValue = c.ContractValue
                            };
            return await contracts.GetGridResult(contracts, input);
        }
        [HttpPost]
        public async Task<ViewContractFileDto> AddFile([FromForm] SaveContractFileDto input)
        {
            var existContract = await WorkScope.GetAll<Contract>().AnyAsync(s => s.Id == input.ContractId);
            if (!existContract)
            {
                throw new UserFriendlyException("Contract do not exist!!!");
            }
            else
            {
                if (input != null && input.File != null && input.File.Length > 0)
                {
                    var fileUrl = input.File.FileName;
                    var tempPath = Path.Combine(_hostingEnvironment.WebRootPath, "contract_file");
                    if (!Directory.Exists(tempPath))
                    {
                        Directory.CreateDirectory(tempPath);
                    }
                    using (var stream = File.Create(Path.Combine(_hostingEnvironment.WebRootPath, "contract_file", fileUrl)))
                    {
                        await input.File.CopyToAsync(stream);
                        input.Id = await WorkScope.InsertAndGetIdAsync<ContractFile>(new ContractFile
                        {
                            ContractId = input.ContractId,
                            FileUrl = fileUrl
                        });
                    }
                    return new ViewContractFileDto
                    {
                        Id = input.Id,
                        FileUrl = $"contract_file/{ fileUrl }"
                    };
                }
                else
                {
                    throw new UserFriendlyException("File do not include!!!");
                }
            }
        }
        [HttpDelete]
        public async Task DeleteFile(long id)
        {
            var contractFile = await WorkScope.GetAsync<ContractFile>(id);
            if (contractFile != null)
            {
                //delete in db
                await WorkScope.DeleteAsync<ContractFile>(id);
                //delete in storage
                var path = Path.Combine(_hostingEnvironment.WebRootPath, "contract_file", contractFile.FileUrl);
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
        public async Task<List<ViewContractFileDto>> GetFileByContrtract(long contractId)
        {
            return await WorkScope.GetAll<ContractFile>().Where(s => s.ContractId == contractId)
                .Select(s => new ViewContractFileDto
                {
                    Id = s.Id,
                    FileUrl = $"contract_file/{ s.FileUrl }"
                }).ToListAsync();
        }
    }
}
