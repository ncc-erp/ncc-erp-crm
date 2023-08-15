import ajax from '../../lib/ajax';
import util from '../../lib/util'
import {Store,Module,ActionContext} from 'vuex' 
interface dashboardState{
}
class dashboard implements Module<dashboardState,any>{
    namespaced=true;
    state={
        application:null,
        user:null,
        tenant:null
    }
    actions={
        async getLatestInformation(content:ActionContext<dashboardState,any>, payload: any){
            let response = await ajax.get('/api/services/app/Dashboard/GetLatestInformation?startDate='+ payload.params.startDate+'&endDate='+payload.params.endDate);
            return response.data.result
        },
       
    }
}
const dashboardStore = new dashboard();
export default dashboardStore;