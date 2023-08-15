import ajax from '../../lib/ajax';
import util from '../../lib/util'
import {Store,Module,ActionContext} from 'vuex' 
interface InvoiceState{
}
class InvoiceStore implements Module<InvoiceState,any>{
    namespaced=true;
    state={
        application:null,
        user:null,
        tenant:null
    }
    actions={
        async savePeopleInChangre(content:ActionContext<InvoiceState,any>, payload: any){
            let response = await ajax.post('/api/services/app/Invoice/SavePeopleInCharge', payload.data);
            return response.data
        },
        async getAllInvoices(content:ActionContext<InvoiceState,any>, payload: any){
            let response = await ajax.post('/api/services/app/Invoice/GetAllInvoices',payload.data);
            return response.data.result
        },
        async getUserForInvoiceUser(content:ActionContext<InvoiceState,any>, payload: any){
            let response = await ajax.get('/api/services/app/Invoice/GetUserForInvoiceUser');
            return response.data
        },
        async getInvoiceDetail(content:ActionContext<InvoiceState,any>, payload: any){
            let response = await ajax.get(`/api/services/app/Invoice/GetInvoiceDetail?=${payload.invoiceId}`);
            return response.data
        },
        async getInvoiceComment(content:ActionContext<InvoiceState,any>, payload: any){
            let response = await ajax.post(`/api/services/app/InvoiceComment/GetByInvoicePaging?invoiceId=${payload.data.invoiceId}`, payload.data);
            return response.data
        },
        async submitComment(content:ActionContext<InvoiceState,any>, payload: any){
            let response = await ajax.post(`/api/services/app/InvoiceComment/Save`, payload.data);
            return response.data
        },
        async deleteInvoice(content: any, payload: any){            
            let response = await ajax.delete(`/api/services/app/Invoice/Delete?id=${payload.id}`);
            return response.data.result
        },
        async changeStatusInvoice(content:ActionContext<InvoiceState,any>, payload: any){
            let response = await ajax.post(`/api/services/app/Invoice/ChangeInvoiceStatus`, payload.data);
            return response.data
        },
        async saveInvoice(content: any, payload: any){
            let response = await ajax.post(`/api/services/app/Invoice/Save`, payload.data);
            return response.data.result
        },
        async addFile(content: any, payload: any){
            let response = await ajax.post(`/api/services/app/Invoice/AddFile`, payload.data);
            return response.data.result
        },
        async deleteInvoiceFile(content: any, payload: any){
            let response = await ajax.delete(`/api/services/app/Invoice/DeleteFile?id=${payload.invoiceFileId}`);
            return response.data.result
        },
        async getFileByInvoice(content:ActionContext<InvoiceState,any>, payload: any){
            let response = await ajax.post(`/api/services/app/Invoice/GetFileByInvoice?=${payload.invoiceId}`);
            return response.data
        },
        async exportInvoice(content:ActionContext<InvoiceState,any>, payload: any){
            let response = await ajax.get(`/api/services/app/ExportExcel/ExportInvoice?=${payload.invoiceId}`);
            return response.data.result
        },
    }
}
const invoiceStore = new InvoiceStore();
export default invoiceStore;