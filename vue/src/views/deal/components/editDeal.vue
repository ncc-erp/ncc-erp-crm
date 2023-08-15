<template>
    <div class="detail-deal">
      <PageHeading>
        <template #header>{{$t('deal.editDeal')}}</template>
        <template #button>
          <Button name="deal_save" class="button btn-add"  @click="onSubmit('dataSubmit')" >{{$t('common.save')}}</Button>
          <Button name="deal_back" type="default" @click="onBack">{{$t('common.backToList')}}</Button>
          <Button name="deal_delete" type="default"  @click="openConfirmDeletePopup">{{$t('common.delete')}}</Button>
        </template>
      </PageHeading>
        <Form ref="dataSubmit" :model="dataSubmit" :rules="ruleValidate" :label-width="200" label-position="left">
                <FormItem :label="$t('deal.name')" prop="name">                    
                    <Input name="dealName_input" v-model="dataSubmit.name" :placeholder="$t('common.placeholderEnter',{field: $t('deal.name')})" />
                </FormItem>
                <FormItem :label="$t('deal.owner')" prop="ownerId">                    
                    <Select name="dealOwnerId_input" v-model="dataSubmit.ownerId" filterable :placeholder="$t('common.placeholderEnter',{field: $t('deal.owner')})">
                        <Option v-for="item in assigneeList" :value="item.userId" :key="item.userId">{{ item.userName }}</Option>
                    </Select>
                </FormItem>
                <FormItem :label="$t('deal.description')" prop="description">
                    <Input name="dealDescription_input" v-model="dataSubmit.description" :autosize="{minRows: 5}" :placeholder="$t('common.placeholderEnter',{field: $t('deal.description')})" type="textarea" />
                </FormItem>
                <FormItem :label="$t('deal.client')" prop="clientId">
                    <Select name="dealClientId_input" v-model="dataSubmit.clientId" filterable :placeholder="$t('common.placeholderSelect',{field: $t('deal.client')})">
                        <Option v-for="(item, index) of clientData" v-bind:key="index" :value="item.id">{{item.name}}</Option>
                    </Select>
                </FormItem>
                <FormItem :label="$t('deal.status')" prop="status">
                    <Select name="dealStatus_input" v-model="dataSubmit.status" @on-change="selectStatus" :placeholder="$t('common.placeholderEnter',{field: $t('deal.status')})">
                        <Option v-for="(item, index) of statusList" v-bind:key="index" :value="item.id">{{item.name}}</Option>
                    </Select>
                </FormItem>
                <FormItem :label="$t('task.priority')" prop="priority">
                    <Select name="dealPriority_input" v-model="dataSubmit.priority" @on-change="selectStatus" :placeholder="$t('common.placeholderEnter',{field: $t('task.priority')})">
                        <Option v-for="(item, index) of priorityList" v-bind:key="index" :value="item.id">{{item.name}}</Option>
                    </Select>
                </FormItem>

                <FormItem :label="$t('deal.amount')" prop="amount">
                    <InputNumber name="dealAmount_input" style="width: 100%" v-model="dataSubmit.amount" :min="0" :placeholder="$t('common.placeholderEnter', {field: $t('deal.amount')})"></InputNumber>
                </FormItem>
                <FormItem :label="$t('deal.dealStartDate')" prop="dealStartDate">
                    <Col :md="4" class="deal-start-date">
                        <DatePicker name="dealStartDate_input" v-model="dataSubmit.dealStartDate" :clearable="false"></DatePicker>
                    </Col>
                </FormItem>
                <FormItem :label="$t('deal.dealLastFollow')" prop="dealLastFollow">
                    <Col :md="4">
                        <DatePicker name="dealLastFollow_input" v-model="dataSubmit.dealLastFollow" :clearable="true"></DatePicker>
                    </Col>
                </FormItem>
                <FormItem :label="$t('deal.employeeFuture')">
                    <Row v-for="(item, index) in dealLevels" :key="index">
                        <Col :md="4">
                            <Select :name="`dealSkillId${index}_input`" v-model="item['skillId']" @change="showData()" :placeholder="$t('common.placeholderEnter', {field: $t('deal.skill')})" :clearable="true">
                                <Option v-for="(skill, i) of skills" v-bind:key="i" :value="skill.id">{{skill.skillName}}</Option>
                            </Select>
                        </Col>
                        <Col :md="8">
                            <Row class="">
                                <Col :md="12" class="text-center">
                                    <label>{{$t('deal.level')}}</label>
                                </Col>
                                <Col :md="12">
                                    <Select :name="`dealLevelId${index}_input`" v-model="item['levelId']" :placeholder="$t('common.placeholderEnter', {field: $t('deal.level')})" :clearable="true">
                                        <Option v-for="(level, j) of levels" v-bind:key="j" :value="level.id">{{level.levelName}}</Option>
                                    </Select>
                                </Col>
                            </Row>
                        </Col>
                        <Col :md="8">
                            <Row>
                                <Col :md="12" class="text-center">
                                    <label>{{$t('deal.quantity')}}</label>
                                </Col>
                                <Col :md="12">
                                    <Input :name="`dealQuantity${index}_input`" v-model="item['quantity']" :placeholder="$t('common.placeholderEnter', {field: $t('deal.quantity')})"/>
                                </Col>
                            </Row>
                        </Col>
                        <Col :md="4">
                            <Icon v-if="index >= 0" type="md-close" size="25" @click="removeRowSkill(index)"/>
                        </Col>
                    </Row>
                    <Row class="">
                        <Icon  type="md-add" size="25" @click="addRowSkill()"/>
                    </Row>
                </FormItem>
                <FormItem :label="$t('deal.winReason')" prop="winReason">                    
                    <Input name="dealWinReason_input" v-model="dataSubmit.winReason" :placeholder="$t('common.placeholderEnter',{field: $t('deal.winReason')})" />
                </FormItem>
                <FormItem :label="$t('deal.loseReason')" prop="loseReason">                    
                    <Input name="dealLoseReason_input" v-model="dataSubmit.loseReason" :placeholder="$t('common.placeholderEnter',{field: $t('deal.loseReason')})" />
                </FormItem>
                    <UploadFile :entity="entity" :entityId="this.$route.params.dealId" @handleInputFile="upload"></UploadFile>
                    <Table name="editDeal_tbl" border :columns="columnsFileTable" :data="dataFileTable" :no-data-text="$t('common.nodatas')" style="width: 100%">
                            <template slot-scope="{ row }" slot="fileUrl">
                                <a @click="openFile(row.fileUrl)">{{row.fileName}}</a>
                            </template> 
                            <template slot-scope="{ row }" slot="action">
                                <Icon type="md-close-circle" size="30"  @click="deleteFile(row.id)" />
                            </template>
                    </Table>
            </Form>
            <Modal v-model="isShowConfirmDelete" :title="$t('common.notification')">
                <p>{{$t('assignment.confirmDeleteNotice')}}</p>
                <Row slot="footer" class="button-zone">
                <Button
                    name="deal_confirmDelete"
                    class="button btn-add"
                    size="default"
                    @click="deleteDeal"
                  >{{$t('common.accept')}}</Button>
               <Button
                    name="deal_cancelDelete"
                    type="error"
                    size="default"
                    ghost
                    @click="isShowConfirmDelete=false"
                >{{$t('common.cancel')}}</Button>
              </Row>
            </Modal>
      </div>
</template>
<script lang="ts">
    import { Component, Vue, Prop, Emit, Watch } from 'vue-property-decorator';
    import Util from '@/lib/util'
    import AbpBase from '@/lib/abpbase'
    import PageHeading from "@/components/heading/heading.vue";
    import {ICommonList, IFileDetail, ListType, ListStatus, ListStatusDeal, IAssignee, ISaveDeal} from '@/store/entities/project'
    import { saveAs } from 'file-saver';
    import UploadFile from '../../../components/upload-file/upload-file.vue'
    import appconst from '../../../lib/appconst'
    import { EntityDefault } from '@/store/entities/entity';
    import { EPriorityList } from '@/store/entities/task';
import moment from 'moment';

    @Component({
        name: 'editDeal',
        components: {
            PageHeading, UploadFile
        }
    })
export default class editDeal extends AbpBase{
    private dataSubmit = {} as ISaveDeal
    private isClickSubmit: boolean = false
    ruleValidate:any = {}
    typeList = ListType
    statusList = []
    clientData: ICommonList[] = []
    assigneeList: IAssignee[] = []
    isProjectWin: boolean = false
    isProjectLose: boolean = false
    columnsFileTable: any = []
    dataFileTable: IFileDetail[] = []
    entity = EntityDefault.Deal
    isShowConfirmDelete = false
    priorityList = EPriorityList
    dealLevels: any[] = []
    levels: any[] = [];
    skills: any[] = [];
    @Watch("$route")
    getEvent() {
        this.fetchData();
    }
    selectStatus(statusId: number) {
        if(statusId == this.statusList[3].id) {this.isProjectWin = true; this.isProjectLose = false}
        else if(statusId == this.statusList[4].id || statusId == this.statusList[5].id) {this.isProjectLose = true; this.isProjectWin = false}
        else {this.isProjectWin = false; this.isProjectLose = false}
    }
    async deleteDeal() {
        if(this.$route.params.dealId) {
            await this.$store.dispatch({
            type: "deal/deleteDeal",
            id: this.$route.params.dealId
            });
        }
        this.$Message.info(this.$t('common.deleted'));
        this.isShowConfirmDelete = false
        this.$router.push({ name: 'deal'}).catch(()=>{})
    }
    async created() {
        await this.getDropdownLevels();
        await this.getDropdownSkills();
        await this.fetchData()
        this.columnsFileTable = [
            {
                title:this.$t('invoice.attachment').toString(),
                align: 'center',   
                children: [
                    {
                        title: this.$t('common.name').toString(),
                        align: 'center',
                        slot: 'fileUrl',
                        resizable: true,
                    },
                    {
                        title: this.$t("projectManagement.action").toString(),
                        align: 'center',
                        slot: "action"
                    }
                ]
            }
        ]
    }
    private async fetchData() {
        this.dataSubmit =  {
            ownerId: null,
            description: "",
            clientId: null,
            status: null,
            priority: null,
            amount: null,
            name: "",
            id: null,
            winReason: "",
            loseReason: "",
            dealStartDate: moment(new Date()).format('YYYY/MM/DD'),
            dealLastFollow: null,
            dealDetail: null
        }
        let respone = await this.$store.dispatch({
            type: "deal/getAllStatus",
            entityName: 'deal'
        });
        this.statusList = respone.listStatus.map((el) => {
            const _el = el;
            _el.id = el.status
            _el.name = el.statusName
            return _el
        });
        if(this.$route.params.dealId) {
            let response = await this.$store.dispatch({
                type: "deal/getDeal",
                id: this.$route.params.dealId
            });
            this.dealLevels = []
            if(response.dealDetails != null){
                this.dealLevels = response.dealDetails.map((item) => {
                    return {
                        skillId: item.skillId, 
                        levelId: item.levelId, 
                        quantity: item.quantity
                    };
                })
            }
            else{
                this.addRowSkill()
            }
            for(let key in this.dataSubmit) {
                if(key)
                this.dataSubmit[key] = response[key]
            }
        }
        this.ruleValidate = {
            name: [
                { required: true, message: this.$t('deal.name') , trigger: 'blur' },
            ],
            ownerId: [
                { required: true, type: 'number', message: this.$t('deal.errOwner') , trigger: 'change' },
            ],
            description: [
                { required: true, message: this.$t('deal.errDescription'), trigger: 'blur' },
            ],
            clientId: [
                { required: true, type: 'number', message: this.$t('deal.errClient'), trigger: 'change' }
            ],
            status: [
                { required: true, type: 'number', message: this.$t('deal.errStatus'), trigger: 'change' }
            ],
            priority: [
                { required: true, type: 'number', message: this.$t('deal.errStatus'), trigger: 'change' }
            ],
            // amount: [
            //     { required: true, type: 'number', message: this.$t('deal.errAmount'), trigger: 'blur' }
            // ],
            dealStartDate: [
                { required: true, type: 'date', message: this.$t('deal.errDealStartDate'), trigger: 'change' }
            ]
        }
        
        let clientData = await this.$store.dispatch({
            type:'project/getClient'
        })
        this.clientData = clientData.result
        let assigneeList = await this.$store.dispatch({
            type: 'project/getAssigneeList'
        }) 
        this.assigneeList = assigneeList.result
        this.selectStatus(this.dataSubmit.status)
        this.getFiles(this.$route.params.dealId)
    }

    async getFiles(dealId) {
        if(dealId){
        let data = await this.$store.dispatch({
            type: 'deal/getFileByDeal',
            dealId: dealId
        })  
        this.dataFileTable = data.result
        this.dataFileTable.forEach((item:any)=>{
            item.fileName = item.fileUrl.replace("deal_file/","")
        })}
    }
    async upload(e) {
        if(e){
            await this.getFiles(this.$route.params.dealId);
        }
    }
    async deleteFile(dealFileId) {
        let data = await this.$store.dispatch({
            type: 'deal/deleteDealFile',
            dealFileId: dealFileId
        })
        await this.getFiles(this.$route.params.dealId);
    }
    openFile(url){
        window.open(appconst.remoteServiceBaseUrl + url)
    }
    private async onSubmit(name) {
    const form = await this.$refs[name] as any;     
    this.dataSubmit.dealDetail = this.cleanData()
    if(this.validateForm(this.dataSubmit)){
            await this.edit()
        }
    else {
            this.$Message.error('Fail!');
         }
    }
    validateForm(formData){
       return formData.name 
       && formData.ownerId
       && formData.description 
       && formData.clientId 
       && formData.status != null 
       && formData.priority 
       && formData.dealStartDate
    }
    async edit() {
        this.dataSubmit.dealStartDate = this.convertedDate(this.dataSubmit.dealStartDate)
        this.dataSubmit.dealLastFollow = this.convertedDate(this.dataSubmit.dealLastFollow)
        await this.$store.dispatch({
            type: 'deal/saveDeal',
            data: this.dataSubmit
        }) 
        this.$Message.info(this.$t('common.saved'));
        await this.onBack()
    }
    private onBack() {
    this.$router.push({ name: 'dealDetail', params: { dealId: this.$route.params.dealId } }).catch(()=>{})
    }
    openConfirmDeletePopup() {
    this.isShowConfirmDelete = true
    }
    async getDropdownLevels(){
        this.levels = await this.$store.dispatch({
            type: "deal/getDropdownLevels"
        })
    }

    async getDropdownSkills(){
        this.skills = await this.$store.dispatch({
            type: "deal/getDropdownSkills"
        })
    }
    async addRowSkill(){
        let obj = {
            skillId: null, 
            levelId: null, 
            quantity: 1
        };
        this.dealLevels.push(obj)
    }
    removeRowSkill(index){
        this.dealLevels.splice(index,1)
    }
    cleanData(){
        //remove skillid null
        let myDealLevels = this.dealLevels.filter((obj) => {
            return obj.skillId != null;
        })
        return myDealLevels;
    }
    convertedDate(value: string) {
        if(value)
            return moment(value).format('YYYY/MM/DD')
        return null
    }
}
</script>
<style lang="scss" scoped>  
    .code-row-bg{
        margin-top: 10px;
    }
    .detail-deal{
        padding: 20px;
    }
    .button-back{
      margin-top: 20px;
      .btn-submit{
        margin-right: 15px;
      }
    }
    .error-text {
        color: red;
    }
    Button {
        margin-right: 5px;
    }
    .ivu-date-picker{
        display: block;
    }
    .text-center{
        text-align: center;
    }
    ::v-deep .deal-start-date .ivu-icon-ios-loading:before{
        content: none !important;
    }
</style>
