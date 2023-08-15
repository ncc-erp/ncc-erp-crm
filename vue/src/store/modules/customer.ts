import ajax from '../../lib/ajax';
import util from '../../lib/util'
import {Store,Module,ActionContext} from 'vuex' 
interface CustomerState{
}
class CustomerStore implements Module<CustomerState,any>{
    namespaced=true;
    state={
        application:null,
        user:null,
        tenant:null
    }
    actions={
        async getAllPaging(content:ActionContext<CustomerState,any>, payload: any){
            let response = await ajax.post('/api/services/app/Client/GetAllPaging',payload.params);
            return response.data.result
        },
        async saveClient(content:ActionContext<CustomerState,any>, payload: any){
            let response = await ajax.post('/api/services/app/Client/Save',payload.data);
            return response.data.result
        },
        async deleteClient(content: any, payload: any){            
            let response = await ajax.delete(`/api/services/app/Client/Delete?id=${payload.id}`);
            return response.data.result
        },
        async getCustomer(content:ActionContext<CustomerState,any>, payload: any){
            let response = await ajax.get('/api/services/app/Client/GetById?Id='+payload.id);
            return response.data.result
        },
        async changeStatusClient(content:ActionContext<CustomerState,any>, payload: any){
            let response = await ajax.post('/api/services/app/Client/ChangeStatusClient',payload.data);
            return response.data.result
        },
        async getDropdownRegion(content:ActionContext<CustomerState,any>, payload: any){
            let response = await ajax.get('/api/services/app/client/GetDropdownRegion');
            return response.data.result
        }
    }
}
const customerStore = new CustomerStore();
export default customerStore;