using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Collections.Extensions;
using Abp.Extensions;
using Abp.UI;
using CRM.Entities;
using CRM.WorkFlows.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CRM.Enums.StatusEnum;

namespace CRM.WorkFlows
{
    [AbpAuthorize]
    public class WorkflowAppService : CRMAppServiceBase
    {
        /// <summary>
        /// Lấy những trạng thái có thể thay đổi được theo role
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        [AbpAuthorize, HttpGet]
        public async Task<List<WorkflowTransitionDto>> GetWorkflowTransition(int entityId)
        {
            var entityString = Enum.GetName(typeof(EntityDefault), entityId);
            if (entityString.IsNullOrEmpty())
            {
                throw new UserFriendlyException("Entity không hợp lệ");
            }
            var userRole = await WorkScope.GetAll<UserRole>().FirstOrDefaultAsync(ur => ur.UserId == AbpSession.UserId);
            var workFlowTransitions = await (from wt in WorkScope.GetAll<WorkflowTransition>()
                                             join w in WorkScope.GetAll<Workflow>() on wt.WorkflowId equals w.Id
                                             join tr in WorkScope.GetAll<TransitionPermission>().Where(tr => tr.RoleId == userRole.RoleId)
                                             on wt.Id equals tr.WorkflowTransitionId into transitionPermisson
                                             from tr in transitionPermisson.DefaultIfEmpty()
                                             where w.EntityName == entityString
                                             select new WorkflowTransitionDto
                                             {
                                                 FromStatus = wt.FromStatus,
                                                 ToStatus = wt.ToStatus,
                                                 CanChange = tr != null
                                             }).ToListAsync();
            return workFlowTransitions;
        }
        [HttpPost]
        public async Task AddEditWorkflowStatus(WorkflowStatusDto input)
        {
            var oldData =  WorkScope.GetAll<WorkflowStatus>()
                                         .WhereIf(input.Id.HasValue, s => s.Id == input.Id)
                                         .WhereIf(!input.Id.HasValue, s => (s.WorkflowId == input.WorkflowId && s.Status == input.Status))
                                         .FirstOrDefault();
            if (input.Id == null || input.Id<1)
            {
                if (oldData != null)
                {
                    throw new UserFriendlyException("Đã tồn tại status");
                }
                var data = new WorkflowStatus
                {
                    Status = input.Status,
                    StatusName = input.StatusName,
                    WorkflowId = input.WorkflowId
                };
                await WorkScope.InsertAsync(data);
            }
            else
            {
                if (oldData != null)
                {
                    // if (oldData?.Status != input.Status || oldData?.WorkflowId != input.WorkflowId)
                    // {
                    //     if (await WorkScope.GetAll<WorkflowStatus>().AnyAsync(s => s.WorkflowId == input.WorkflowId && s.Status == input.Status))
                    //     {
                    //         throw new UserFriendlyException($"Đã tồn tại status có status = {input.Status}");
                    //     }
                    //     if (await WorkScope.GetAll<WorkflowTransition>().AnyAsync(s => s.WorkflowId == oldData.WorkflowId && (s.ToStatus == oldData.Status || s.FromStatus == oldData.Status)))
                    //     {
                    //         throw new UserFriendlyException("Đang tồn tại transition của status này, vui lòng sửa lại workflow transition trước khi sửa status");
                    //     }

                    // }

                    oldData.Status = input.Status;
                    oldData.StatusName = input.StatusName;
                    oldData.WorkflowId = input.WorkflowId;
                    await WorkScope.UpdateAsync(oldData);
                }
                else
                {
                    throw new UserFriendlyException("Không tìm thấy");
                }
            }
        }
        [HttpGet]
        public async Task DeleteStatus (long WorkflowId,int Status)
        {
            if (await WorkScope.GetAll<WorkflowTransition>().AnyAsync(s => s.WorkflowId == WorkflowId && (s.ToStatus == Status || s.FromStatus == Status)))
            {
                throw new UserFriendlyException("Đang tồn tại transition của status này, vui lòng sửa lại workflow transition trước khi sửa status");
            }
            var Data =  WorkScope.GetAll<WorkflowStatus>().Where(s => s.WorkflowId == WorkflowId && s.Status == Status);
            foreach(var i in Data)
            {
                await WorkScope.DeleteAsync(i);
            }

        }
        public async Task UpdateWorkflowTransition(long WorkFlowId,int FromStatus,int ToStatus)
        {
            if(await WorkScope.GetAll<WorkflowTransition>().AnyAsync(s=>s.WorkflowId==WorkFlowId && s.FromStatus ==FromStatus && s.ToStatus == ToStatus))
            {
                throw new UserFriendlyException("Đã tồn tại Workflow Transition");
            }
            if(!await WorkScope.GetAll<WorkflowStatus>().AnyAsync(s=>s.WorkflowId==WorkFlowId && s.Status == FromStatus))
            {
                throw new UserFriendlyException($"Không tồn tại Workflow ${WorkFlowId} có Status ${FromStatus}");
            }
            if (!await WorkScope.GetAll<WorkflowStatus>().AnyAsync(s => s.WorkflowId == WorkFlowId && s.Status == ToStatus))
            {
                throw new UserFriendlyException($"Không tồn tại Workflow ${WorkFlowId} có Status ${ToStatus}");
            }
            var data = new WorkflowTransition
            {
                FromStatus = FromStatus,
                ToStatus = ToStatus,
                WorkflowId = WorkFlowId,
            };
            await WorkScope.InsertAsync(data);
        }
        
    }
}