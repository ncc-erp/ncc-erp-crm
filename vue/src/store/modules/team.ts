import ajax from '../../lib/ajax';
import util from '../../lib/util'
import {Store,Module,ActionContext} from 'vuex' 
interface teamState{
}
class team implements Module<teamState,any>{
    namespaced=true;
    state={
        application:null,
        user:null,
        tenant:null
    }
    actions={
        async getAllPagging(content:ActionContext<teamState,any>, payload: any){
            let response = await ajax.get('/api/services/app/Team/GetAll');
            return response.data.result
        },

        async saveTeam(content:ActionContext<teamState,any>, payload: any){
          await ajax.post('/api/services/app/Team/Save',payload.params);
        },

        async delete(content: any, payload: any){            
            let response = await ajax.delete(`/api/services/app/Team/Delete?id=${payload.id}`);
            return response.data.result
        },

        async active(content:ActionContext<teamState,any>, payload: any){
            let response = await ajax.post('/api/services/app/Team/Active', payload.teamId);
            return response.data.result
        },

        async deActive(content:ActionContext<teamState,any>, payload: any){
            let response = await ajax.post('/api/services/app/Team/DeActive', payload.teamId);
            return response.data.result
        },

        async getUserTeam(content:ActionContext<teamState,any>, payload: any){
            let response = await ajax.get(`/api/services/app/UserTeam/Get?id=${payload.teamId}`);
            return response.data.result
        },

        async updateUserTeam(content:ActionContext<teamState,any>, payload: any){
            let response = await ajax.post('/api/services/app/UserTeam/Update', payload.createData);
            return response.data.result
        },
    }
}
const teamStore = new team();
export default teamStore;