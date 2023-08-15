import ajax from '../../lib/ajax';
import util from '../../lib/util'
import {Store,Module,ActionContext} from 'vuex'
interface TaskState{
}
class TaskStore implements Module<TaskState,any>{
    namespaced=true;
    state={
        application:null,
        user:null,
        tenant:null
    }
    actions={
        async getAllPaging(content:ActionContext<TaskState,any>, payload: any){
            let response = await ajax.post('/api/services/app/Assignment/GetAllPaging',payload.data);
            return response.data
        },
        async getAllEntity(content:ActionContext<TaskState,any>){
            let response = await ajax.get('/api/services/app/Assignment/GetAllEntity');
            return response.data
        },
        async saveTask(content:ActionContext<TaskState,any>, payload: any){
            await ajax.post('/api/services/app/Assignment/Save',payload.data);
        },
        async getTask(content:ActionContext<TaskState,any>, payload: any){
            let response = await ajax.get(`/api/services/app/Assignment/GetAssignmentById?Id=${payload.param.Id}`)
            return response.data.result
        },
        async getListTasK(content: ActionContext<TaskState, any>, payload: any) {
            let response = await ajax.get(`/api/services/app/Assignment/GetAssignmentByEntityId?EntityType=${payload.param.EntityType}&EntityId=${payload.param.EntityId}`)
            return response.data.result
        },
        async changeStatusClient(content:ActionContext<TaskState,any>, payload: any){
            let response = await ajax.post('/api/services/app/Client/ChangeStatusClient',payload.data);
            return response.data.result
        },
        async addFile(content: any, payload: any){
            let response = await ajax.post(`/api/services/app/Assignment/AddFile`, payload.data);
            return response.data.result
        },
        async deleteTaskFile(content: any, payload: any){
            let response = await ajax.delete(`/api/services/app/Assignment/DeleteFile?id=${payload.taskFileId}`);
            return response.data.result
        },
        async deleteTask(content: any, payload: any){
            let response = await ajax.delete(`/api/services/app/Assignment/Delete?id=${payload.id}`);
            return response.data.result
        },
        async getFileByTask(content:ActionContext<TaskState,any>, payload: any){
            let response = await ajax.get(`/api/services/app/Assignment/GetFileByTask?taskId=${payload.taskId}`);
            return response.data
        },
        async changeStatusTask(content:ActionContext<TaskState,any>, payload: any){
            let response = await ajax.post(`/api/services/app/Assignment/ChangeTaskStatus`, payload.data);
            return response.data
        },
        async getTaskComment(content:ActionContext<TaskState,any>, payload: any){
            let response = await ajax.get(`/api/services/app/TaskComment/GetByTask?taskId=${payload.data.taskId}`, payload.data);
            return response.data
        },
        async submitComment(content:ActionContext<TaskState,any>, payload: any){
            let response = await ajax.post(`/api/services/app/TaskComment/Save`, payload.data);
            return response.data
        },
    }
}
const taskStore = new TaskStore();
export default taskStore;
