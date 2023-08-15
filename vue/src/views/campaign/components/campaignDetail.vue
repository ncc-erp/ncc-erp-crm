<template>
    <div>
        <PageHeading>
            <template #header>{{$t('emailCampaign.emailCampaign')}}</template>
            <template #button>
                <Button class="button btn-add" @click="saveTemplate">{{$t('emailTemplate.save')}}</Button>
                <Button class="button btn-add" @click="sendEmail">{{$t('common.send')}}</Button>
            </template>
        </PageHeading>
        <div class="container">
            <Row>
                <Col span="5"><b>{{$t('emailCampaign.name')}}: </b></Col>
                <Col span="12">{{data.campaignName}}</Col>
            </Row>
            <Row>
                <Col span="5"><b>Email Template: </b></Col>
                <Col span="5">
                    <Select v-model="templateId" @on-change="selectTemplate" :clearable="true" :placeholder="'Select email template'" @on-clear="clearContent">
                        <Option v-for="item in templateList" :value="item.id" :key="item.id">{{ item.name }}</Option>
                    </Select>
                </Col>
            </Row>
            <Editor :value="content" @onChange="change"/>
            <div class="margin-top-10">
                <div style="margin: 15px 0"><b>{{$t('emailCampaign.contactList')}} :</b></div>
                <Table name="sent_tbl" :columns="columns_1" :data="selectedList" height="250" :no-data-text="$t('common.nodatas')" >

                <template slot-scope="{ row }" slot="action">
                    <Button
                    class="btn-edit"
                    style="margin-right: 5px"
                    @click="onDelete(row)"
                    >{{$t('common.delete')}}</Button>
                </template>
                </Table>
            </div>
            <div class="margin-top-10">
                <div style="margin: 15px 0"><b>{{$t('emailCampaign.addToList')}} :</b></div>
                <Table name="contact_tbl" @on-selection-change="selectListCheckbox" :columns="columns_2" :data="notSelectedYetList" height="250" :no-data-text="$t('common.nodatas')" border>

                <template slot-scope="{ row }" slot="action">
                    <Button
                    class="btn-edit"
                    style="margin-right: 5px"
                    @click="openEditContact(row)"
                    >{{$t('common.delete')}}</Button>
                </template>
                </Table>
                <Button
                class="btn-update"
                style="margin-top: 15px"
                type="primary"
                @click="onUpdate()"
                >{{$t('common.update')}}</Button>
            </div>
        </div>
    </div>
</template>
<script lang="ts">
import { Component, Vue, Inject, Prop } from "vue-property-decorator";
import AbpBase from "../../../lib/abpbase";
import editorComponent from "../../tinymce/editorComponent.vue";
import PageHeading from "../../../components/heading/heading.vue";
import moment from "moment";
import { EmailStatus } from '../../../store/entities/task';
@Component({
  name: 'campaignDetail',
  components: {
    Editor: editorComponent,
    PageHeading
  }
})
export default class Roles extends AbpBase {
    content: string = ""
    templateId: number = 0
    subject: string = ""
    isLoading: boolean = false
    campaignId: any = null
    isShowInfo: boolean = false;
    getEmailtemplate: any = [];
    columns_1: any = []
    columns_2: any = []
    selectedList: any = []
    listContactOnAdd: any = []
    selectedListTemp: any = []
    allContact: any = []
    templateList: any = []
    notSelectedYetList: any = []
    data: any = {}
    dataSendMail: any = {}
    async sendEmail() {       
        const dataSendMail = {
            campaignId: this.$route.params.campaignId,
            emailTemplateDto: {
                subject: this.templateId == null ? null : this.subject,
                content: this.content,
                id: this.templateId == null ? 0 : this.templateId
            }
        } 
        await this.$store.dispatch({
            type: "campaign/sendMail",
            data: dataSendMail
        })
        this.$Message.info('Email had been sent!');
        this.$router.push({ name: 'campaign'});
    }
    change(value: any) {
        this.content = value
    }
    showInfor() {
        this.isShowInfo = !this.isShowInfo;
    }
    onDelete(row: any) {
        let index = this.selectedList.findIndex((item:any) =>{
            return item.id == row.id
        })
        this.selectedList.splice(index,1)
        this.restoreNotSeletedYetList(row)
    }
    restoreNotSeletedYetList(row: any) {
        this.notSelectedYetList.push(row)
        this.notSelectedYetList.sort((a, b) => (a.id > b.id) ? 1 : -1)
    }
    async created() {
        this.isLoading = true
        this.columns_1 = [
            {
                title: this.$t('contact.name'),
                resizable: true,
                align: 'center',
                key: 'contactName',
            },
            {
                title: this.$t('contact.email'),
                key: "contactEmail",
                resizable: true,
                align: 'center',
            },
            {
                title: this.$t('common.status'),
                key: "status",
                resizable: true,
                align: 'center',
            },
            {
                title: this.$t('common.setting'),
                slot: "action",
                resizable: true,
                align: 'center',
            },
        ]
        this.columns_2 = [
            {
                type: 'selection',
                width: 60,
                align: 'center'
            },
            {
                title: this.$t('contact.name'),
                resizable: true,
                align: 'center',
                key: "contactName",
            },
            {
                title: this.$t('contact.email'),
                key: "contactEmail",
                resizable: true,
                align: 'center',
            },
        ]
        if(this.$route.params.campaignId) this.campaignId = this.$route.params.campaignId
        const campaignContact = await this.$store.dispatch({
            type: "campaign/campaignContact",
            param: this.campaignId
        })

        let listContact = await this.$store.dispatch({
            type:'contact/getAllPagging',
            params: {maxResultCount: 10000}
        })
        this.allContact = listContact.items // global list all contact
        this.data = campaignContact.result
        this.selectedList = campaignContact.result.contacts
        if (this.selectedList.length > 0) {
            this.selectedList.forEach(element => {
                const ele = EmailStatus.find(x => x.id == element.status)
                element.status = ele.name
            });
        }
        this.selectedListTemp = campaignContact.result.contacts //global list contact selected

        this.notSelectedYetList = listContact.items
        this.notSelectedYetList = this.notSelectedYetList.map((item:any) =>{
            if(item.name)
            return {
                contactEmail: item.mail,
                contactName: item.name,
                id: item.id,
                status: item.status
            }
            else return item
        })
        this.allContact = this.allContact.map((item:any) =>{
            if(item.name)
            return {
                contactEmail: item.mail,
                contactName: item.name,
                id: item.id,
                status: item.status
            }
            else return item
        })
        this.spliceListContact()
        const response = await this.$store.dispatch({
        type: "campaign/getAllEmail"
        })
        this.templateList = response.result       
        if (campaignContact.result.mailTemplates.length > 0) {
            this.getEmailtemplate = campaignContact.result.mailTemplates;
            this.templateId = campaignContact.result.mailTemplates[0].id;
            this.content = campaignContact.result.mailTemplates[0].content; 
            this.subject = campaignContact.result.mailTemplates[0].subject;        
        }
    }
    spliceListContact() {//splice  selected not yet list
        this.selectedList.forEach((item:any) =>{
            let index = this.notSelectedYetList.findIndex((ite:any) =>{
                if(item.id == ite.id) return ite
            })
            this.notSelectedYetList.splice(index,1)
        })
    }
    clearContent() {
        this.templateId = 0;
        this.content = '';
        this.subject = null;
    }
    async saveTemplate() {
        const dataSendMail = {
            campaignId: this.$route.params.campaignId,
            emailTemplateDto: {
                subject: this.templateId == null ? null : this.subject,
                content: this.content,
                id: this.templateId == null ? 0 : this.templateId
            }
        }
        await this.$store.dispatch({
            type: 'email/saveCampaignEmailTemplate',
            data: dataSendMail
        })
        this.$Message.info(this.$t('common.saved'));
    }
    convertedDateTime(value: string) {
        if(value) return moment(value).format('HH:mm YYYY/MM/DD')
        else return ""
    }
    selectListCheckbox(list:any) {
        let listSelected = list.map((item:any) => {
            return item.id
        })
        this.listContactOnAdd = []
        this.allContact.forEach((item:any) =>{
            listSelected.forEach((ite:any) =>{
                if(item.id == ite) this.listContactOnAdd.push(item)
            })
        })
    }
    async onUpdate() {
        if(this.listContactOnAdd.length) {
            this.selectedList = this.selectedList.concat(this.listContactOnAdd) // mapping 2 array
            this.spliceListContact()
            let listContactId = this.selectedList.map((item:any) =>{
                return item.id
            })
            let dataSubmit = {
                id: this.campaignId,
                name: this.data.campaignName,
                contactId: listContactId,
            }
            const response = await this.$store.dispatch({
                type: "campaign/addCampaign",
                data: dataSubmit
            })
            this.$Message.info('Updated!');
            this.listContactOnAdd = []
        }
        else this.$Message.error('List contact can not be empty!');
    }
    selectTemplate(id: any) {
        const findItem = this.getEmailtemplate.find((item: any) => item.id == id);
        if (findItem != null) {
            this.content = findItem.content;
            this.subject = findItem.subject;
            this.data.emailTemplate = findItem;
        }
        else {
            const item = this.templateList.find((item: any) => item.id == id);
            this.content = item.content
            this.subject = item.subject;
            this.data.emailTemplate = item;
        }
    }
}
</script>
<style lang="scss" scope>
    .ivu-row {
        padding: 10px;
    }
    .container {
        margin-bottom: 20px;
    }
</style>
<style lang="scss">

    .tox .tox-notification--warn, .tox .tox-notification--warning {
        display: none !important;
    }
    .ivu-modal{
        width: fit-content !important;
    }
</style>
