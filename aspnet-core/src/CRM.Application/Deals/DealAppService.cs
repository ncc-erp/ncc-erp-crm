using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.UI;
using CRM.APIWorkflowController;
using CRM.Authorization.Users;
using CRM.Constant;
using CRM.Deals.Dto;
using CRM.Entities;
using CRM.ExportExcel.Dto;
using CRM.Extension;
using CRM.Paging;
using CRM.WorkFlows;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static CRM.Enums.StatusEnum;

namespace CRM.Deals
{
    [AbpAuthorize]
    public class DealAppService : CRMAppServiceBase
    {
        private IHostingEnvironment _hostingEnvironment;
        private readonly WorkflowAppService _workflowAppService;
        private WorkflowControllerAppService _workflowControllerAppService;
        public DealAppService(IHostingEnvironment environment, WorkflowAppService workflowAppService, WorkflowControllerAppService workflowControllerAppService)
        {
            _hostingEnvironment = environment;
            _workflowAppService = workflowAppService;
            _workflowControllerAppService = workflowControllerAppService;
        }
        [HttpPost]
        public async Task<long> Save(SaveDealDto input)
        {
            // create
            if (input.Id <= 0)
            {
                var owner = await WorkScope.GetAsync<User>(input.OwnerId);
                if (owner == null)
                {
                    throw new UserFriendlyException("Owner does not exist!!!");
                }
                if (input.ClientId.HasValue)
                {
                    var client = await WorkScope.GetAsync<Client>(input.ClientId.Value);
                    if (client == null)
                    {
                        throw new UserFriendlyException("Client does not exist!!!");
                    }
                }
                if (input.ContactId.HasValue)
                {
                    var contact = await WorkScope.GetAsync<Contact>(input.ContactId.Value);
                    if (contact == null)
                    {
                        throw new UserFriendlyException("Client does not exist!!!");
                    }
                }
                var item = new Deal
                {
                    Name = input.Name,
                    ClientId = input.ClientId,
                    OwnerId = input.OwnerId,
                    Amount = input.Amount.HasValue ? input.Amount.Value : 0,
                    Description = input.Description,
                    ContactID=input.ContactId,
                    Status = input.Status,
                    Priority=input.Priority,
                    LastModificationTime = DateTime.Now,
                    DealStartDate = input.DealStartDate,
                    DealLastFollow = input.DealLastFollow
                };
                input.Id = await WorkScope.InsertAndGetIdAsync<Deal>(item);
                if (input.DealDetail != null)
                {
                    await AddDealDetail(input.DealDetail, input.Id);
                }
                await CurrentUnitOfWork.SaveChangesAsync();
                // add task              
            }
            // update
            else
            {
                var old = await WorkScope.GetAsync<Deal>(input.Id);
                if (old == null)
                {
                    throw new UserFriendlyException("Deal does not exist!!!");
                }
                if (old.Name != input.Name)
                {
                    var NameOfEntity = WorkScope.GetAll<EntityAssignment>().Where(s => s.EntityId == input.Id && s.EntityType == EntityDefault.Deal);
                    foreach (var i in NameOfEntity)
                    {
                        i.NameOfEntity = input.Name;
                    }
                    await WorkScope.UpdateRangeAsync(NameOfEntity);
                }
                old.Name = input.Name;
                old.ClientId = input.ClientId;
                old.OwnerId = input.OwnerId;
                old.Amount = input.Amount.HasValue ? input.Amount.Value : 0;
                old.ContactID=input.ContactId;
                old.Description = input.Description;
                old.WinReason = input.WinReason;
                old.LoseReason = input.LoseReason;
                old.Priority = input.Priority;
                old.DealStartDate = input.DealStartDate;
                old.DealLastFollow = input.DealLastFollow;
                _workflowControllerAppService.ChangeStatusDeal(input.Id, input.Status);

                //update deal/skill/level of deal
                await UpdateDealDetail(old.Id, input.DealDetail);

                //set closing date
                if (input.Status == DealStatus.DealLost || input.Status == DealStatus.ProjectFail || input.Status == DealStatus.ProjectWin)
                {
                    old.ClosingDate = DateTime.Now;
                }
                await WorkScope.UpdateAsync(old);
            }
            return input.Id;
        }
        [HttpPost]
        public async Task<long> QuickSave(SaveDealDto input)
        {
            
            var old = await WorkScope.GetAsync<Deal>(input.Id);
            if (old == null)
            {
                throw new UserFriendlyException("Deal does not exist!!!");
            }
            old.OwnerId = input.OwnerId;
            old.Description = input.Description;
            old.Priority = input.Priority;
            _workflowControllerAppService.ChangeStatusDeal(input.Id, input.Status);
           
            //set closing date
            if (input.Status == DealStatus.DealLost || input.Status == DealStatus.ProjectFail || input.Status == DealStatus.ProjectWin)
            {
                old.ClosingDate = DateTime.Now;
            }
            await WorkScope.UpdateAsync(old);
            return input.Id;
        }
        [HttpDelete]
        public async Task Delete(long id)
        {
            var count = await WorkScope.GetAll<Project>().CountAsync(s => s.DealId == id);
            var old = await WorkScope.GetAsync<Deal>(id);
            if (count > 0)
            {
                throw new UserFriendlyException(String.Format("Deal {0} has Project data!!!", old.Name));
            }
            if (old == null)
            {
                throw new UserFriendlyException("Deal does not exist!!!");
            }
            var project = await WorkScope.GetAll<Project>().Where(s => s.DealId == id).FirstOrDefaultAsync();
            if (project != null)
            {
                throw new UserFriendlyException(String.Format("Can not delete. Deal has project: {0}!!!", project.Name));
            }
            await WorkScope.DeleteAsync<Deal>(id);
            //delete file
            var listFile = await WorkScope.GetAll<DealFile>().Where(s => s.DealId == id).ToListAsync();
            foreach (var deleteFile in listFile)
            {
                //delete in db
                await WorkScope.DeleteAsync<DealFile>(deleteFile.Id);
                //delete in storage
                var path = Path.Combine(_hostingEnvironment.WebRootPath, "deal_file", deleteFile.Path);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            //delete comment
            var listComment = await WorkScope.GetAll<DealComment>().Where(s => s.DealId == id).Select(s => s.Id).ToListAsync();
            foreach (var deleteCommentId in listComment)
            {
                await WorkScope.DeleteAsync<DealComment>(deleteCommentId);
            }
            //delete task
        }
        [HttpGet]
        public async Task<ViewDealDto> GetById(long id)
        {
            var DealClient = from d in WorkScope.GetAll<Deal>().Where(s => s.Id == id)
                             join c in WorkScope.GetAll<Client>() on d.ClientId equals c.Id into temped
                             from s in temped.DefaultIfEmpty()
                             select new DealContact
                             {
                                 dealId = d.Id,
                                 jointId = s!= null?s.Id:0,
                                 jointName = s !=null?s.Name:null,
                             };
            var DealContact = from d in WorkScope.GetAll<Deal>().Where(s => s.Id == id)
                              join c in WorkScope.GetAll<Contact>() on d.ClientId equals c.Id into temped
                              from s in temped.DefaultIfEmpty()
                              select new 
                              {
                                  dealId = d.Id,
                                  contactId = s!=null?s.Id:0,
                                  contactName = s!=null?s.Name:null
                              };

            return await (from d in WorkScope.GetAll<Deal>().Where(s => s.Id == id)
                          join c in DealClient on d.Id equals c.dealId
                          join e in DealContact on d.Id equals e.dealId
                          join u in WorkScope.GetAll<User>() on d.OwnerId equals u.Id
                          join p in WorkScope.GetAll<Project>() on d.Id equals p.DealId into t
                          join ds in WorkScope.GetAll<DealDetail>() on d.Id equals ds.DealId into dsl
                          select new ViewDealDto
                          {
                              Id = d.Id,
                              Name = d.Name,
                              Amount = d.Amount,
                              Description = d.Description,
                              OwnerId = u.Id,
                              OwnerName = u.FullName,
                              ClientId = c.jointId,
                              ClientName = c.jointName,
                              ContactId=e.contactId,
                              ContactName=e.contactName,
                              Status = d.Status,
                              ClosingDate = d.ClosingDate,
                              WinReason = d.WinReason,
                              LoseReason = d.LoseReason,
                              CreationTime=d.CreationTime,
                              Priority=d.Priority,
                              Project = t.Count() > 0 ? t.Select(s => new ProjectInDeal
                              {
                                  Id = s.Id,
                                  Code = s.Code,
                                  Name = s.Name,
                                  Status = s.Status
                              }).FirstOrDefault() : null,
                              DealDetails = dsl.Any() ?
                                            dsl.Select(s => new DealDetailDto
                                            {
                                                LevelId = s.LevelId,
                                                LevelName = s.Level.Name,
                                                SkillId = s.SkillId,
                                                SkillName = s.Skill.Name,
                                                Quantity = s.Quantity,
                                            }).ToList()
                                        : null,
                              DealStartDate = d.DealStartDate,
                              DealLastFollow = d.DealLastFollow
                          }).FirstOrDefaultAsync();
        }
        [HttpPost]
        public async Task<GridResult<ViewDealDto>> GetAllPagging(GetallInput input)
        {
            if (input.StartDate.HasValue && input.EndDate.HasValue)
            {
                if (input.StartDate.Value > input.EndDate.Value)
                {
                    throw new UserFriendlyException("Start Date must be less than End Date");
                }
            }

            var workflowTransitions = await _workflowAppService.GetWorkflowTransition((int)EntityDefault.Deal);
            var query = (from d in WorkScope.GetAll<Deal>()
                        join c in WorkScope.GetAll<Client>() on d.ClientId equals c.Id
                        join u in WorkScope.GetAll<User>() on d.OwnerId equals u.Id
                        join p in WorkScope.GetAll<Project>() on d.Id equals p.DealId into t
                        join ds in WorkScope.GetAll<DealDetail>() on d.Id equals ds.DealId into dsl
                        select new ViewDealDto
                        {
                            Id = d.Id,
                            Name = d.Name,
                            Amount = d.Amount,
                            Description = d.Description,
                            OwnerId = u.Id,
                            OwnerName = u.FullName,
                            ClientId = c.Id,
                            ClientName = c.Name,
                            LastModificationTime = d.LastModificationTime,
                            Status = d.Status,
                            ClosingDate = d.ClosingDate,
                            WinReason = d.WinReason,
                            LoseReason = d.LoseReason,
                            CreationTime=d.CreationTime,
                            Priority = d.Priority,
                            Project = t.Count() > 0 ? t.Select(s => new ProjectInDeal
                            {
                                Id = s.Id,
                                Code = s.Code,
                                Name = s.Name,
                                Status = s.Status
                            }).FirstOrDefault() : null,
                            ChangeFromInProgressToWin = d.Status == DealStatus.ProjectInProgress
                                                          && workflowTransitions.Any(wt => wt.FromStatus == (int)d.Status && wt.ToStatus == (int)DealStatus.ProjectWin && wt.CanChange),
                            ChangeFromInProgressToFail = d.Status == DealStatus.ProjectInProgress
                                                          && workflowTransitions.Any(wt => wt.FromStatus == (int)d.Status && wt.ToStatus == (int)DealStatus.ProjectFail && wt.CanChange),
                            ChangeFromProcessingToLost = d.Status == DealStatus.ProcessingRequest
                                                          && workflowTransitions.Any(wt => wt.FromStatus == (int)d.Status && wt.ToStatus == (int)DealStatus.DealLost && wt.CanChange),
                            ChangeFromProcessingToInProgress = d.Status == DealStatus.ProcessingRequest
                                                          && workflowTransitions.Any(wt => wt.FromStatus == (int)d.Status && wt.ToStatus == (int)DealStatus.ProjectInProgress && wt.CanChange),
                            ChangeToProcessing = true,
                            DealLastFollow = d.DealLastFollow,
                            DealStartDate = d.DealStartDate,
                            DealDetails = dsl.Any() ?
                                           dsl.Select(s => new DealDetailDto
                                           {
                                               LevelId = s.LevelId,
                                               LevelName = s.Level.Name,
                                               SkillId = s.SkillId,
                                               SkillName = s.Skill.Name,
                                               Quantity = s.Quantity,
                                           }).ToList()
                                        : null,
                        })
                        .WhereIf(input.StartDate.HasValue, s => s.DealStartDate >= input.StartDate)
                        .WhereIf(input.EndDate.HasValue, s => s.DealStartDate <= input.EndDate)
                        .OrderByDescending(s => s.LastModificationTime).ThenByDescending(s => s.CreationTime).AsQueryable();
            //orderby deal start date
            if (!String.IsNullOrEmpty(input.DealStartDateSort))
            {
                if (input.DealStartDateSort.Equals("desc"))
                {
                    query = query.OrderByDescending(x => x.DealStartDate ?? DateTime.MinValue);
                }
                else if (input.DealStartDateSort.Equals("asc"))
                {
                    query = query.OrderBy(x => x.DealStartDate ?? DateTime.MaxValue);
                }
            }

            //orderby deal last follow
            if (!String.IsNullOrEmpty(input.DealLastFollowSort))
            {
                if (input.DealLastFollowSort.Equals("desc"))
                {
                    query = query.OrderByDescending(x => x.DealLastFollow ?? DateTime.MinValue);
                }
                else if (input.DealLastFollowSort.Equals("asc"))
                {
                    query = query.OrderBy(x => x.DealLastFollow ?? DateTime.MaxValue);
                }
            }

            return  query.GetGridResultSync(query, input.Param);
        }
        public async Task<IActionResult> ExportDealToExcel()
        {
            var Data = from d in WorkScope.GetAll<Deal>()
                       join c in WorkScope.GetAll<Client>() on d.ClientId equals c.Id
                       join u in WorkScope.GetAll<User>() on d.OwnerId equals u.Id
                       join p in WorkScope.GetAll<Project>() on d.Id equals p.DealId into t
                       select new 
                       {
                           Id = d.Id,
                           Name = d.Name,
                           Amount = d.Amount,
                           Description = d.Description,
                           OwnerId = u.Id,
                           OwnerName = u.FullName,
                           ClientId = c.Id,
                           ClientName = c.Name,
                           Status = d.Status.ToString(),
                           ClosingDate = d.ClosingDate.HasValue?d.ClosingDate.Value.ToString("dd/MM/yyyy"):"",
                           WinReason = d.WinReason,
                           LoseReason = d.LoseReason,
                           CreationTime = d.CreationTime.ToString("dd/MM/yyyy"),
                           Priority = d.Priority.ToString(),
                           Project = t.Count() > 0 ? t.Select(s => new 
                           {
                               Id = s.Id,
                               Code = s.Code,
                               Name = s.Name,
                               Status = s.Status.ToString()
                           }).FirstOrDefault() : null,

                       };
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            
            string sFileName = @"Deal.xlsx";
            
            string URL = string.Format("{0}/{1}", VariableConstant.ServerRootAddress, sFileName);
            
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            
            var memory = new MemoryStream();
            
            using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                
                ISheet excelSheet = workbook.CreateSheet("Deal");
                
                IRow row = excelSheet.CreateRow(0);
                row.CreateCell(0).SetCellValue("Stt");
                row.CreateCell(1).SetCellValue("Tên Deal");
                row.CreateCell(2).SetCellValue("Số tiền");
                row.CreateCell(3).SetCellValue("Người sở hữu");
                row.CreateCell(4).SetCellValue("Tên khách hàng");
                row.CreateCell(5).SetCellValue("Trạng thái");
                row.CreateCell(6).SetCellValue("Mức độ ưu tiên");
                row.CreateCell(7).SetCellValue("Ngày khởi tạo Deal");
                row.CreateCell(8).SetCellValue("Ngày kết thúc");
                row.CreateCell(9).SetCellValue("Lý do thắng");
                row.CreateCell(10).SetCellValue("Lý do thua");
                row.CreateCell(11).SetCellValue("Project của Deal");
                row.CreateCell(12).SetCellValue("Status của Project");
                //
                var k = 0;
                foreach(var i in Data)
                {
                    k++;
                    row = excelSheet.CreateRow(k);
                    row.CreateCell(0).SetCellValue(k);
                    row.CreateCell(1).SetCellValue(i.Name);
                    row.CreateCell(2).SetCellValue(i.Amount);
                    row.CreateCell(3).SetCellValue(i.OwnerName);
                    row.CreateCell(4).SetCellValue(i.ClientName);
                    row.CreateCell(5).SetCellValue(i.Status);
                    row.CreateCell(6).SetCellValue(i.Priority);
                    row.CreateCell(7).SetCellValue(i.CreationTime);
                    row.CreateCell(8).SetCellValue(i.ClosingDate);
                    row.CreateCell(9).SetCellValue(i.WinReason);
                    row.CreateCell(10).SetCellValue(i.LoseReason);
                    row.CreateCell(11).SetCellValue($"{i.Project?.Name}");
                    row.CreateCell(12).SetCellValue($"{i.Project?.Status}");
                }
                workbook.Write(fs);
            }
            using (var stream = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open))
                
            {
                
                await stream.CopyToAsync(memory);
                
            }
            
            memory.Position = 0;

            /* return File.(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);*/
            return new FileStreamResult(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = $"Deal Report {DateTime.Now.ToString("dd/MM/yyyy")}"
            };

        }
        [HttpPost]
        public async Task ChangeDealStatus(DealStatusInput input)
        {
            var old = WorkScope.Get<Deal>(input.Id);
            var workflowTransitions = await _workflowAppService.GetWorkflowTransition((int)EntityDefault.Deal);
            var flag = workflowTransitions.Any(wt => wt.FromStatus == (int)old.Status && wt.ToStatus == (int)input.Status && wt.CanChange);
            if (flag)
            {
                old.Status = input.Status;
            }
            else
            {
                throw new UserFriendlyException("Status dose not permit!!!");
            }
            await CurrentUnitOfWork.SaveChangesAsync();
        }
        [HttpPost]
        public async Task<ViewDealFileDto> AddFile([FromForm] SaveDealFileDto input)
        {
            var existDeal = await WorkScope.GetAll<Deal>().AnyAsync(s => s.Id == input.DealId);
            if (!existDeal)
            {
                throw new UserFriendlyException("Deal does not exist!!!");
            }
            else
            {
                if (input != null && input.File != null && input.File.Length > 0)
                {
                    var fileUrl = input.File.FileName;
                    var tempPath = Path.Combine(_hostingEnvironment.WebRootPath, "deal_file");
                    if (!Directory.Exists(tempPath))
                    {
                        Directory.CreateDirectory(tempPath);
                    }
                    using (var stream = File.Create(Path.Combine(_hostingEnvironment.WebRootPath, "deal_file", fileUrl)))
                    {
                        await input.File.CopyToAsync(stream);
                        input.Id = await WorkScope.InsertAndGetIdAsync<DealFile>(new DealFile
                        {
                            DealId = input.DealId,
                            Path = fileUrl
                        });
                    }
                    return new ViewDealFileDto
                    {
                        Id = input.Id,
                        FileUrl = $"deal_file/{fileUrl}"
                    };
                }
                else
                {
                    throw new UserFriendlyException("File does not include!!!");
                }
            }
        }
        [HttpDelete]
        public async Task DeleteFile(long id)
        {
            var contractFile = await WorkScope.GetAsync<DealFile>(id);
            if (contractFile != null)
            {
                //delete in db
                await WorkScope.DeleteAsync<DealFile>(id);
                //delete in storage
                var path = Path.Combine(_hostingEnvironment.WebRootPath, "deal_file", contractFile.Path);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            else
            {
                throw new UserFriendlyException("File does not exist!!!");
            }
        }
        [HttpGet]
        public async Task<List<ViewDealFileDto>> GetFileByDeal(long dealId)
        {
            return await WorkScope.GetAll<DealFile>().Where(s => s.DealId == dealId)
                .Select(s => new ViewDealFileDto
                {
                    Id = s.Id,
                    FileUrl = $"deal_file/{s.Path}"
                }).ToListAsync();
        }
        [HttpGet]
        public async Task<IActionResult> GetDropdownLevels()
        {
            var levels = await WorkScope.GetAll<Level>()
                                .Select(s => new
                                {
                                    Id = s.Id,
                                    LevelName = s.Name
                                }).ToListAsync();
            return new OkObjectResult(levels);
        }
        [HttpGet]
        public async Task<IActionResult> GetDropdownSkills()
        {
            var skills = await WorkScope.GetAll<Skill>()
                                .Select(s => new
                                {
                                    Id = s.Id,
                                    SkillName = s.Name
                                }).ToListAsync();
            return new OkObjectResult(skills);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateDealLastFollow(UpdateDealLastFollowDto input)
        {
            var dealItem = await WorkScope.GetAll<Deal>().FirstOrDefaultAsync(x => x.Id == input.DealId);
            if(dealItem != null)
            {
                dealItem.DealLastFollow = input.DealLastFollow;
                await WorkScope.UpdateAsync<Deal>(dealItem);
                return new OkObjectResult("Update Success!");
            }
            return new BadRequestObjectResult("Not Found Deal!");
        }
        private async Task AddDealDetail(List<CreateDealDetailDto> dealDetails, long dealId)
        {
            var newDealDetails = new List<DealDetail>();
            foreach (var item in dealDetails)
            {
                var dealDetailItem = new DealDetail
                {
                    CreationTime = DateTime.Now,
                    IsDeleted = false,
                    Quantity = item.Quantity,
                    LevelId = item.LevelId,
                    SkillId = item.SkillId,
                    DealId = dealId,
                };
                newDealDetails.Add(dealDetailItem);
            }
            await WorkScope.InsertRangeAsync<DealDetail>(newDealDetails);
        }
        private async Task UpdateDealDetail(long dealId, List<CreateDealDetailDto> dealDetails)
        {
            //get all old dealdetail of deal
            var oldDealDetails = await WorkScope.GetAll<DealDetail>()
                   .Where(x => x.DealId == dealId)
                   .ToListAsync();
            if (dealDetails != null)
            {
                foreach (var item in dealDetails)
                {
                    var dealDetailItem = oldDealDetails.FirstOrDefault(x => x.LevelId == item.LevelId && x.SkillId == item.SkillId);
                    if(dealDetailItem != null)
                    {
                        if(dealDetailItem.Quantity != item.Quantity)
                        {
                            dealDetailItem.Quantity = item.Quantity;
                            await WorkScope.UpdateAsync<DealDetail>(dealDetailItem);
                        }
                        oldDealDetails.Remove(dealDetailItem);
                    }
                    else
                    {
                        var newDealDetailItem = new DealDetail
                        {
                            DealId = dealId,
                            SkillId = item.SkillId,
                            LevelId = item.LevelId,
                            IsDeleted = false,
                            CreationTime = DateTime.Now,
                            Quantity = item.Quantity,
                        };
                        await WorkScope.InsertAsync<DealDetail>(newDealDetailItem);
                    }
                }
            }
            if(oldDealDetails != null)
            {
                await DeleteListDealDetail(oldDealDetails);
            }
        }
        private async Task DeleteListDealDetail(List<DealDetail> oldDealSkills)
        {
            foreach (var item in oldDealSkills)
            {
                await WorkScope.DeleteAsync<DealDetail>(item);
            }
        }
    }
}
