import ajax from '../../lib/ajax';
import util from '../../lib/util'
import {Store,Module,ActionContext} from 'vuex' 
interface ProjectState{
}
class ProjectStore implements Module<ProjectState,any>{
    namespaced=true;
    state={
        application:null,
        user:null,
        tenant:null
    }
    actions={
        async getProject(content:ActionContext<ProjectState,any>, payload: any){
            let response = await ajax.post('/api/services/app/Project/GetAllPaging',payload.data);
            return response.data
        },
        async addProject(content:ActionContext<ProjectState,any>, payload: any){
            let response = await ajax.post('/api/services/app/Project/Save',payload.data);
            return response.data
        },
        async getClient(content:ActionContext<ProjectState,any>){
            let response = await ajax.get('/api/services/app/Client/GetClientForProject');
            return response.data
        },
        async deleteProject(content: any, payload: any){            
            let response = await ajax.delete(`/api/services/app/Project/Delete?id=${payload.id}`);
            return response.data.result
        },
        async getAssigneeList(content:ActionContext<ProjectState,any>){
            let response = await ajax.get('/api/services/app/Invoice/GetUserForInvoiceUser');
            return response.data
        },
        async getProjectById(content:ActionContext<ProjectState,any>, payload: any){
            let response = await ajax.get(`/api/services/app/Project/GetProject?id=${payload.projectId}`);
            return response.data
        }
    }
}
const projectStore = new ProjectStore();
export default projectStore;