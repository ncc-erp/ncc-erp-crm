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
        async getEmailTemplate(content:ActionContext<EmailState,any>, payload: any){
            let response = await ajax.get('/api/services/app/EmailTemplate/GetAll');
            return response.data.result
        },

        async saveEmailTemplate(content:ActionContext<EmailState,any>, payload: any){
            let response = await ajax.post('/api/services/app/EmailTemplate/Save', payload.data);
            return response.data.result
        },
        async saveCampaignEmailTemplate(content:ActionContext<EmailState,any>, payload: any){
            let response = await ajax.post('/api/services/app/EmailTemplate/SaveCampaignEmailTemplate', payload.data);
            return response.data.result
        },
        
        async getDetailEmailTemplate(content:ActionContext<EmailState,any>, payload: any){
            let response = await ajax.get('/api/services/app/EmailTemplate/Get?Id=' +  payload.emailId);
            return response.data.result
        },

        async uploadImage(content:ActionContext<EmailState,any>, payload: any){
            let response = await ajax.post('/api/services/app/Image/UploadImage',  payload.data);
            return response
        },

        async getEmailSetting(content:ActionContext<EmailState,any>, payload: any){
            let response = await ajax.get('/api/services/app/EmailSetting/Get');
            return response.data.result
        },

        async changeEmailSetting(content:ActionContext<EmailState,any>, payload: any){
            let response = await ajax.post('/api/services/app/EmailSetting/Change', payload.data);
            return response.data.result
        },
    }
}
const emailStore = new EmailStore();
export default emailStore;