import ajax from '../../lib/ajax';
import util from '../../lib/util'
import {Store,Module,ActionContext} from 'vuex' 
interface assignmentState{
}
class assignment implements Module<assignmentState,any>{
    namespaced=true;
    state={
        application:null,
        user:null,
        tenant:null
    }
    actions={
        async getCurrentLoginInformations(content:ActionContext<assignmentState,any>, payload: any){
            let response = await ajax.get('/api/services/app/Session/GetCurrentLoginInformations');
            return response.data.result
        },

        async getAllPaging(content:ActionContext<assignmentState,any>, payload: any){
            let response = await ajax.post('/api/services/app/Assignment/GetAllPaging',payload.param);
            return response.data.result
        },
       
    }
}
const assignmentStore = new assignment();
export default assignmentStore;