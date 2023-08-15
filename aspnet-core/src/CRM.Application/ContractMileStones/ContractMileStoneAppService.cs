using Abp.UI;
using CRM.ContractMileStones.Dto;
using CRM.Entities;
using CRM.Extension;
using CRM.Paging;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CRM.ContractMileStones
{
    public class ContractMileStoneAppService : CRMAppServiceBase
    {
        [HttpPost]
        public async Task<SaveContractMileStoneDto> Save(SaveContractMileStoneDto input)
        {
            //create
            if (input.Id <= 0)
            {
                var item = ObjectMapper.Map<ContractMileStone>(input);
                input.Id = await WorkScope.InsertAndGetIdAsync<ContractMileStone>(item);
            }
            //update
            else
            {
                var old = await WorkScope.GetAsync<ContractMileStone>(input.Id);
                ObjectMapper.Map<SaveContractMileStoneDto, ContractMileStone>(input, old);
            }
            return input;
        }
        [HttpDelete]
        public async Task Delete(long id)
        {
            var old = await WorkScope.GetAsync<ContractMileStone>(id);
            if (old != null)
            {
                await WorkScope.DeleteAsync<ContractMileStone>(id);
            }
            else
            {
                throw new UserFriendlyException(String.Format("ContractMileStone %s not found", old.Name));
            }
        }
        [HttpGet]
        public async Task<ViewContractMileStoneDto> GetById(long id)
        {
            return await WorkScope.GetAll<ContractMileStone>().Where(s => s.Id == id)
                .Select(s => new ViewContractMileStoneDto
                {
                    Id = s.Id,
                    ContractId = s.ContractId,
                    ContractName = s.Contract.Name,
                    Name = s.Name,
                    Description = s.Description,
                    MileStoneDate = s.MileStoneDate,
                    Percentage = s.Percentage,
                    Value = s.Value
                }).FirstOrDefaultAsync();
        }
        [HttpPost]
        public async Task<GridResult<ViewContractMileStoneDto>> GetAllPaging(GridParam input)
        {
            var mileStones = from m in WorkScope.GetAll<ContractMileStone>()
                             join c in WorkScope.GetAll<Contract>() on m.ContractId equals c.Id
                             select new ViewContractMileStoneDto
                             {
                                 Id = m.Id,
                                 ContractId = m.ContractId,
                                 ContractName = c.Name,
                                 Name = m.Name,
                                 Description = m.Description,
                                 MileStoneDate = m.MileStoneDate,
                                 Percentage = m.Percentage,
                                 Value = m.Value
                             };
            return await mileStones.GetGridResult(mileStones, input);
        }
        [HttpGet]
        public async Task<GridResult<ViewContractMileStoneDto>> GetByContract(long contractId, GridParam input)
        {
            var mileStones = from m in WorkScope.GetAll<ContractMileStone>().Where(s => s.ContractId == contractId)
                             join c in WorkScope.GetAll<Contract>() on m.ContractId equals c.Id
                             select new ViewContractMileStoneDto
                             {
                                 Id = m.Id,
                                 ContractId = m.ContractId,
                                 ContractName = c.Name,
                                 Name = m.Name,
                                 Description = m.Description,
                                 MileStoneDate = m.MileStoneDate,
                                 Percentage = m.Percentage,
                                 Value = m.Value
                             };
            return await mileStones.GetGridResult(mileStones, input);
        }
    }
}
