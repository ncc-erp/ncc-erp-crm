import ajax from '../../lib/ajax';
import util from '../../lib/util'
import {Store,Module,ActionContext} from 'vuex' 
interface contactState{
}
class contact implements Module<contactState,any>{
    namespaced=true;
    state={
        application:null,
        user:null,
        tenant:null
    }
    actions={
        async getAllPagging(content:ActionContext<contactState,any>, payload: any){
            let response = await ajax.post('/api/services/app/Contact/GetAllPagging',payload.params);
            return response.data.result
        },

        async saveContact(content:ActionContext<contactState,any>, payload: any){
            let response = await ajax.post('/api/services/app/Contact/Save',payload.params);
          return response.data.result
        },

        async getById(content:ActionContext<contactState,any>, payload: any){
            let response = await ajax.get('/api/services/app/Contact/GetById?id=' + payload.contactId);
            return response.data.result
        },

        async saveClientContact(content:ActionContext<contactState,any>, payload: any){
            await ajax.post('/api/services/app/Contact/SaveClientContact', payload.listContact);
        },
        async deleteContact(content: any, payload: any){            
            let response = await ajax.delete(`/api/services/app/Contact/Delete?id=${payload.id}`);
            return response.data.result
        },

        async getByClient(content:ActionContext<contactState,any>, payload: any){
            let response = await ajax.get('/api/services/app/Contact/GetByClient?clientId=' + payload.clientId);
            return response.data.result
        },
       
    }
}
const contactStore = new contact();
export default contactStore;