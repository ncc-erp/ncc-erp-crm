using Abp.Authorization;
using Abp.UI;
using CRM.DealComments.Dto;
using CRM.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CRM.Authorization.Users;

namespace CRM.DealComments
{
    [AbpAuthorize]
    public class DealCommentAppService : CRMAppServiceBase
    {
        [HttpPost]
        public async Task<SaveDealCommentDto> Save(SaveDealCommentDto input)
        {
            //create
            if (input.Id <= 0)
            {
                var comment = new DealComment
                {
                    DealId = input.DealId,
                    Content = input.Content
                };
                input.Id = await WorkScope.InsertAndGetIdAsync<DealComment>(comment);
            }
            //update
            else
            {
                var old = await WorkScope.GetAsync<DealComment>(input.Id);
                if (old == null)
                {
                    throw new UserFriendlyException("Comment does not exsit!!!");
                }
                old.Content = input.Content;
                await WorkScope.UpdateAsync<DealComment>(old);
            }
            return input;
        }
        [HttpPost]
        public async Task<Object> Reply(SaveDealCommentDto input)
        {
            var comment = new DealComment
            {
                DealId = input.DealId,
                Content = input.Content,
                ParentId = input.Id,
            };
             input.Id= await WorkScope.InsertAndGetIdAsync<DealComment>(comment);
             return input;
        }
        [HttpDelete]
        public async Task Delete(long id)
        {
            var old = await WorkScope.GetAsync<DealComment>(id);
            if (old == null)
            {
                throw new UserFriendlyException("Comment does not exsit!!!");
            }
            await WorkScope.DeleteAsync<DealComment>(id);
        }
        [HttpGet]
        public async Task<ViewDealCommentDto> GetById(long id)
        {
            return await WorkScope.GetAll<DealComment>().Where(s => s.Id == id)
                .Select(s => new ViewDealCommentDto
                {
                    Id = s.Id,
                    Content = s.Content
                }).FirstOrDefaultAsync();
        }
        [HttpGet]
        public List<ViewDealCommentDto> GetByDeal(long dealId)
        {
            var listComment = WorkScope.GetAll<DealComment>().Where(s => s.DealId == dealId).ToList();
            var listUserComment = listComment.Select(x=>x.CreatorUserId).Distinct().ToList();
            var lisUserName = WorkScope.GetAll<User>()
                .Where(x => listUserComment
                .Contains(x.Id))
                .Select(t => new { Id = t.Id, UserName = t.UserName }).ToList();
            return listComment.Select(z => new ViewDealCommentDto
            {
                Id = z.Id,
                Content = z.Content,
                CommentTime = z.CreationTime,
                ParentId = z.ParentId,
                UserName = lisUserName.Find(x=>x.Id==z.CreatorUserId.Value).UserName
            }).ToList();
        }
    }
}
