import ajax from '../../lib/ajax';
import util from '../../lib/util'
import {Store,Module,ActionContext} from 'vuex' 
interface ContractState{
}
class ContractStore implements Module<ContractState,any>{
    namespaced=true;
    state={
        application:null,
        user:null,
        tenant:null
    }
    actions={
        async saveContract(content:any, payload: any){
            let response = await ajax.post('/api/services/app/Contract/Save', payload.data);
            return response.data;
        },
        async deleteContract(content:ActionContext<ContractState,any>, payload: any){
            let response = await ajax.delete(`/api/services/app/Contract/Delete?id=${payload.contractId}`, );
            return response.data;
        },
        async GetAllPaging(content:ActionContext<ContractState,any>, payload: any){
            let response = await ajax.post('/api/services/app/Contract/GetAllPaging', payload.data);
            return response.data;
        },
        async addFile(content: any, payload: any){
            let response = await ajax.post(`/api/services/app/Contract/AddFile`, payload.data);
            return response.data.result
        },
        async deleteContractFile(content: any, payload: any){
            let response = await ajax.delete(`/api/services/app/Contract/DeleteFile?id=${payload.contractFileId}`);
            return response.data.result
        },
        async getFileByContrtract(content:ActionContext<ContractState,any>, payload: any){
            let response = await ajax.post(`/api/services/app/Contract/getFileByContrtract?=${payload.contractId}`);
            return response.data
        },
    }
}
const contractStore = new ContractStore();
export default contractStore;