import ajax from '../../lib/ajax';
import util from '../../lib/util'
import {Store,Module,ActionContext} from 'vuex' 
interface DealState{
}
class DealStore implements Module<DealState,any>{
    namespaced=true;
    state={
        application:null,
        user:null,
        tenant:null
    }
    actions={
        async getAllPaging(content:ActionContext<DealState,any>, payload: any){
            let response = await ajax.post('/api/services/app/Deal/GetAllPagging',payload.data);
            return response.data
        },
        async saveDeal(content:ActionContext<DealState,any>, payload: any){
            await ajax.post('/api/services/app/Deal/Save',payload.data);
        },
        async quickSaveDeal(content:ActionContext<DealState,any>, payload: any){
            console.log(payload)
            await ajax.post('/api/services/app/Deal/QuickSave',payload.data);
        },
        async getDeal(content:ActionContext<DealState,any>, payload: any){
            let response = await ajax.get('/api/services/app/Deal/GetById?Id='+payload.id);
            return response.data.result
        },
        async changeStatusClient(content:ActionContext<DealState,any>, payload: any){
            let response = await ajax.post('/api/services/app/Client/ChangeStatusClient',payload.data);
            return response.data.result
        },
        async addFile(content: any, payload: any){
            let response = await ajax.post(`/api/services/app/Deal/AddFile`, payload.data);
            return response.data.result
        },
        async deleteDealFile(content: any, payload: any){
            let response = await ajax.delete(`/api/services/app/Deal/DeleteFile?id=${payload.dealFileId}`);
            return response.data.result
        },
        async deleteDeal(content: any, payload: any){            
            let response = await ajax.delete(`/api/services/app/Deal/Delete?id=${payload.id}`);
            return response.data.result
        },
        async getFileByDeal(content:ActionContext<DealState,any>, payload: any){
            let response = await ajax.get(`/api/services/app/Deal/GetFileByDeal?dealId=${payload.dealId}`);
            return response.data
        },
        async changeStatusDeal(content:ActionContext<DealState,any>, payload: any){
            let response = await ajax.post(`/api/services/app/Deal/ChangeDealStatus`, payload.data);
            return response.data
        },
        async getDealComment(content:ActionContext<DealState,any>, payload: any){
            let response = await ajax.get(`/api/services/app/DealComment/GetByDeal?dealId=${payload.data.dealId}`, payload.data);
            return response.data
        },
        async submitComment(content:ActionContext<DealState,any>, payload: any){
            let response = await ajax.post(`/api/services/app/DealComment/Save`, payload.data);
            return response.data
        },
        async submitCommentReply(content:ActionContext<DealState,any>, payload: any){
            let response = await ajax.post(`/api/services/app/DealComment/Reply`, payload.data);
            return response.data
        },
        async getAllStatus(content:ActionContext<DealState,any>, payload: any){
            let response = await ajax.get('/api/services/app/WorkflowController/GetAllStatus?entityName='+payload.entityName);
            return response.data.result
        },
        async getDropdownLevels(content:ActionContext<DealState,any>, payload: any){
            let response = await ajax.get('/api/services/app/Deal/GetDropdownLevels');
            return response.data.result
        },
        async getDropdownSkills(content:ActionContext<DealState,any>, payload: any){
            let response = await ajax.get('/api/services/app/Deal/GetDropdownSkills');
            return response.data.result
        },
        async updateDealLastFollow(content:ActionContext<DealState,any>, payload: any){
            let response = await ajax.post('/api/services/app/Deal/UpdateDealLastFollow', payload.data)
            return response;
        }
    }
}
const dealStore = new DealStore();
export default dealStore;