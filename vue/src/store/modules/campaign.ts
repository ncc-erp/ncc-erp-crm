import ajax from '../../lib/ajax';
import util from '../../lib/util'
import {Store,Module,ActionContext} from 'vuex'
interface EmailState{
}
class EmailStore implements Module<EmailState,any>{
    namespaced=true;
    state={
        application:null,
        user:null,
        tenant:null
    }
    actions={
        async getAllEmail(content:ActionContext<EmailState,any>){
            let response = await ajax.get('/api/services/app/EmailTemplate/GetAll');
            return response.data
        },
        async getAllCampaign(content:ActionContext<EmailState,any>){
            let response = await ajax.get('/api/services/app/CampaignContact/GetAll');
            return response.data
        },
        async getAllPaging(content:ActionContext<EmailState,any>, payload: any) {
            let response = await ajax.post('/api/services/app/CampaignContact/GetAllPaging', payload.params);
            return response.data.result
        },
        async addCampaign(content:ActionContext<EmailState,any>, payload: any){
            let response = await ajax.post('/api/services/app/CampaignContact/Save',payload.data);
            return response.data
        },
        async campaignContact(content:ActionContext<EmailState,any>, payload: any){
            let response = await ajax.get(`/api/services/app/CampaignContact/Get?campaignId=${payload.param}`);
            return response.data
        },
        async sendMail(content: any, payload: any){
            let response = await ajax.post('/api/services/app/EmailTemplate/SendEmail',payload.data);
            return response.data.result
        },
        async getAssigneeList(content:ActionContext<EmailState,any>){
            let response = await ajax.get('/api/services/app/Invoice/GetUserForInvoiceUser');
            return response.data
        },
        async getProjectById(content:ActionContext<EmailState,any>, payload: any){
            let response = await ajax.get(`/api/services/app/Project/GetProject?id=${payload.projectId}`);
            return response.data
        }
    }
}
const emailStore = new EmailStore();
export default emailStore;
