using Abp.UI;
using CRM.Entities;
using CRM.Extension;
using CRM.InvoiceComments.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CRM.Paging;
using CRM.Authorization.Users;

namespace CRM.InvoiceComments
{
    public class InvoiceCommentAppService : CRMAppServiceBase
    {
        [HttpPost]
        public async Task<SaveInvoiceCommentDto> Save(SaveInvoiceCommentDto input)
        {
            //create
            if (input.Id <= 0)
            {
                var item = ObjectMapper.Map<InvoiceComment>(input);
                input.Id = await WorkScope.InsertAndGetIdAsync<InvoiceComment>(item);
            }
            //update
            else
            {
                var old = await WorkScope.GetAsync<InvoiceComment>(input.Id);
                ObjectMapper.Map<SaveInvoiceCommentDto, InvoiceComment>(input, old);
            }
            return input;
        }
        [HttpDelete]
        public async Task Delete(long id)
        {
            var old = await WorkScope.GetAsync<InvoiceComment>(id);
            if (old != null)
            {
                await WorkScope.DeleteAsync<InvoiceComment>(id);
            }
            else
            {
                throw new UserFriendlyException(String.Format("InvoiceComment not found"));
            }
        }
        [HttpGet]
        public async Task<ViewInvoiceCommentDto> GetById(long id)
        {
            return await WorkScope.GetAll<InvoiceComment>().Where(s => s.Id == id)
                .Select(s => new ViewInvoiceCommentDto
                {
                    Id = s.Id,
                    Comment = s.Comment,
                    InvoiceId = s.InvoiceId,
                    InvoiceName = s.Invoice.Name,
                    UserId = s.UserId,
                    UserName = s.User.Name,
                    CreatedDate = s.CreationTime,
                    ModifiedDate = s.LastModificationTime
                }).FirstOrDefaultAsync();
        }
        [HttpPost]
        public async Task<GridResult<ViewInvoiceCommentDto>> GetAllPaging(GridParam input)
        {
            var comments = (from ic in WorkScope.GetAll<InvoiceComment>()
                            join u in WorkScope.GetAll<User>() on ic.UserId equals u.Id
                            select new ViewInvoiceCommentDto
                            {
                                Id = ic.Id,
                                Comment = ic.Comment,
                                InvoiceId = ic.InvoiceId,
                                UserId = ic.UserId,
                                UserName = ic.User.Name,
                                CreatedDate = ic.CreationTime,
                                ModifiedDate = ic.LastModificationTime
                            }).OrderByDescending(s => s.CreatedDate).ThenByDescending(s => s.ModifiedDate);
            return await comments.GetGridResult(comments, input);
        }
        [HttpPost]
        public async Task<GridResult<ViewInvoiceCommentDto>> GetByInvoicePaging(long invoiceId, GridParam input)
        {
            var comments = (from ic in WorkScope.GetAll<InvoiceComment>().Where(s => s.InvoiceId == invoiceId)
                            join u in WorkScope.GetAll<User>() on ic.UserId equals u.Id
                            select new ViewInvoiceCommentDto
                            {
                                Id = ic.Id,
                                Comment = ic.Comment,
                                InvoiceId = ic.InvoiceId,
                                UserId = ic.UserId,
                                UserName = ic.User.Name,
                                CreatedDate = ic.CreationTime,
                                ModifiedDate = ic.LastModificationTime
                            }).OrderByDescending(s => s.CreatedDate).ThenByDescending(s => s.ModifiedDate);
            return await comments.GetGridResult(comments, input);
        }
    }
}
