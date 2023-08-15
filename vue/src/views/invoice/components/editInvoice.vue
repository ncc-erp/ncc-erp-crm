<template>
    <div>
        <PageHeading>
            <template #header>{{$t('invoice.editInvoice')}}</template>        
            <template #button>
                <Button
                    name="invoice_delete"
                    class="button btn-edit"
                    @click="deleteInvoice"
                >
                    {{$t('common.delete')}}
                </Button>
            </template>
        </PageHeading>
        <Modal
          v-model="isShowAddInvoice"
          :footer-hide=true
          width="65%">
        <div slot="header" align="center"><h1>{{$t('customerManagement.addCustomer')}}</h1></div>
        <addCustomer v-if="isShowAddInvoice" @onSubmit="openCancelModalAdd"/>
        <div slot="footer" class="button-zone" align="center">
        </div>
        </Modal>
        <div class="add-invoice"  >
            <Form ref="dataSubmit" :model="dataSubmit" :rules="ruleValidate" :label-width="200" label-position="left" class="add-invoice">
                <FormItem :label="$t('invoice.invoiceName')" prop="invoiceName">
                    <Input name="invoiceName_input" v-model="dataSubmit.invoiceName" placeholder="Enter your name"></Input>
                </FormItem>
                <FormItem :label="$t('invoice.projectName')" prop="projectId">
                    <Select name="invoiceProjectId_input" v-model="dataSubmit.projectId" filterable :placeholder="$t('common.placeholderSelect',{field: $t('invoice.projectName')})">
                        <Option v-for="(item, index) in projectList" :value="item.id" :key="index">{{ item.name }}</Option>
                    </Select>
                </FormItem>
                <FormItem :label="$t('invoice.type')" prop="invoiceType">                
                    <Select name="invoiceType_input" v-model="dataSubmit.invoiceType" clearable @on-change="typeChanged" :placeholder="$t('common.placeholderSelect',{field: $t('invoice.type')})">
                        <Option v-for="(item, index) in listType" :value="item.id" :key="index">{{ item.name }}</Option>
                    </Select>
                </FormItem>
                <FormItem :label="$t('invoice.contract')" prop="contractId">                
                    <Select name="invoiceContractId_input" v-model="dataSubmit.contractId" clearable filterable :placeholder="$t('common.placeholderSelect',{field: $t('invoice.contract')})">
                        <Option v-for="item in contractList" :value="item.contractId" :key="item.contractId">{{ item.contractName }}</Option>
                    </Select>
                </FormItem>
                <FormItem :label="$t('invoice.invoiceDate')" prop="time">                
                    <DatePicker name="invoiceTime_input" type="date" v-model="dataSubmit.time" style="width: 100%" @on-change="getInvoiceDate" :placeholder="$t('common.placeholderSelect',{field: $t('invoice.invoiceDate')})"></DatePicker>
                </FormItem>
                <FormItem :label="$t('invoice.value')" prop="value">                
                    <InputNumber name="invoiceValue_input" style="width: 100%" :placeholder="$t('common.placeholderSelect',{field: $t('invoice.value')})" v-model="dataSubmit.value" ></InputNumber>
                </FormItem>
                <FormItem :label="$t('invoice.currency')" prop="currency">                
                    <Select name="invoiceCurrency_input" v-model="dataSubmit.currency" clearable filterable :placeholder="$t('common.placeholderSelect',{field: $t('invoice.currency')})">
                        <Option v-for="item in currencyList" :value="item.id" :key="item.id">{{ item.name }}</Option>
                    </Select>
                </FormItem>        
                <FormItem :label="$t('invoice.assignee')" prop="assignee">                
                    <Select name="invoiceAssigneeId_input" v-model="dataSubmit.assignee" clearable filterable :placeholder="$t('common.placeholderSelect',{field: $t('invoice.assignee')})">
                        <Option v-for="item in assigneeList" :value="item.userId" :key="item.userId">{{ item.userName }}</Option>
                    </Select>
                </FormItem> 
                
            </Form>       
                <Row class="code-row-bg" align="middle">
                    <UploadFile :entity="entity" :entityId="this.$route.params.invoiceId" @handleInputFile="upload"></UploadFile>
                    <Table name="editInvoice_tbl" border :columns="columnsFileTable" :data="dataFileTable" :no-data-text="$t('common.nodatas')" style="width: 100%">
                            <template slot-scope="{ row }" slot="fileName">
                                <a @click="openFile(row.fileUrl)">{{row.fileName}}</a>
                            </template> 
                            <template slot-scope="{ row }" slot="type">
                                {{row.typeString}}
                            </template>
                            <template slot-scope="{ row }" slot="action">
                                <Button
                                    name="invoiceFile_delete"
                                    class="btn-edit"
                                    style="margin-right: 5px"
                                    @click="deleteFile(row.id)"
                                >{{$t('common.delete')}}</Button>
                            </template>
                    </Table>
                </Row>
                <Row class="code-row-bg" align="middle" v-if="showTable">
                    <p class="titleTable">{{$t('invoice.invoice')}}</p>
                    <Table name="editInvoice2_tbl" border :columns="columns" :data="dataTable" style="width: 100%">
                        <template slot-scope="{ row }" slot="employee">
                            <Select name="invoiceEmployeeId_input" @on-change="change(row)" width="100" v-model="row.userId">
                                <Option v-for="item in assigneeList" :value="item.userId" :key="item.userId">{{ item.userName }}</Option>
                            </Select>
                        </template>
                        <template slot-scope="{ row }" slot="position" style="width: 100%">
                            <Input name="invoicePosition_input" @on-change="change(row)" style="width: 100%" :placeholder="$t('invoice.position')" v-model="row.position" ></Input>
                        </template>
                        <template slot-scope="{ row }" slot="rate" style="width: 100%">
                            <Input name="invoiceRate_input" @on-change="change(row)" style="width: 100%" :placeholder=" $t('invoice.rate')" v-model="row.rate" ></Input>
                        </template>
                        <template slot-scope="{ row }" slot="manMonth" style="width: 100%">
                            <Input name="invoiceManMonth_input" @on-change="change(row)" style="width: 100%" :placeholder="$t('invoice.manMonth')" v-model="row.manMonth" ></Input>
                        </template>
                        <template slot-scope="{ row }" slot="action" style="width: 100%">
                            <i name="invoiceData_delete" @click="closeData(row)" class="icon ivu-icon ivu-icon-ios-remove-circle-outline"></i>
                            <i name="invoiceData_add" @click="increaseData" v-if="row.isIncrease" class="icon ivu-icon ivu-icon-ios-add-circle-outline"></i>
                        </template>
                    </Table>
                </Row>
                <Row type="flex" class="code-row-bg error" v-if="error" align="middle">{{$t('invoice.error')}}</Row> 
                <Row type="flex" class="code-row-bg" justify="space-around" align="middle">
                    <Col span="11" align="right">
                        <Button 
                            name="invoice_save"
                            type="error"
                            size="large"
                            @click="handleEditInvoice('dataSubmit')"
                        >
                            {{$t('common.save')}}
                        </Button>
                    </Col>
                    <Col span="12" align="left">
                        <Button 
                            name="invoice_cancel"
                            type="default"
                            size="large"
                            @click="cancel"
                        >
                            {{$t('common.cancel')}}
                        </Button>
                    </Col>
                </Row>
        </div>
    </div>
</template>
<script lang="ts">
    import { Component, Vue, Prop, Watch } from 'vue-property-decorator';
    import AbpBase from '@/lib/abpbase'
    import {ICommonList, ListType, ListStatus, IInvoiceUserDetail, IDataAddInvoice, IdataSavePeopleInCharge, EProjectType, CurrencyType, IAssignee, ISaveProject, IFileDetail} from '@/store/entities/project'
    import UploadFile from '../../../components/upload-file/upload-file.vue'
    import ContractItem from '@/views/setting/project/components/contract-item.vue';
    import { EContractType } from '@/store/entities/contract-type';
    import { EntityDefault } from '@/store/entities/entity';
    import appconst from '../../../lib/appconst'
    import PageHeading from "@/components/heading/heading.vue"; 
    @Component({
        components: {UploadFile, PageHeading}
    })
    export default class editInvoice extends AbpBase{
        error:boolean = false
        loading:boolean = false
        showTable:boolean = false
        isShowAddInvoice: boolean = false

        columns: any = []
        columnsFileTable: any = []
        employee: number = null
        manMonth: number = null
        rate: number = null
        position: string = ""
        invoiceDate: string = ""

        type : number = null
        file: any
        fileType: any
        invoiceUserId: number = null
        projectList: ICommonList[] = []
        listType: ICommonList[] = ListType
        contractList: ICommonList[] = []
        currencyList: ICommonList[] = CurrencyType
        customerList: ICommonList[] = []
        assigneeList: IAssignee[] = []
        
        dataFileTable: IFileDetail[] = []
        dataTable: IInvoiceUserDetail[] = []
        dataSubmit = {} as IDataAddInvoice
        private invoiceName: string = ''
        entity = EntityDefault.Invoice
        ruleValidate:any = {}
        private idPeople : number = 1

        @Watch("$route")
        getEvent() {            
            this.fetchData();
        }
        async fetchData() {
            this.loading = true
            this.dataTable = []
            this.dataSubmit = {
                invoiceName: '',
                projectId: null,
                invoiceType: null,
                contractId: null,
                time: '',
                value: null,
                assignee: null,
                currency: null,
            }
            
            this.columns = [
                {
                    title:this.$t('invoice.peopleInCharges').toString(),
                    align: 'center',   
                    children: [
                        {
                            title: this.$t('invoice.chooseEmployee').toString(),
                            slot: 'employee',
                            key:'employee',
                            width: 200,
                            align: 'center',
                        },
                        {
                            title: this.$t('invoice.position').toString(),
                            slot: 'position',
                            align: 'center',
                            resizable: true,
                        },
                        {
                            title: this.$t('invoice.rate').toString(),
                            align: 'center',
                            slot: 'rate',
                            resizable: true,
                        },
                        {
                            title: this.$t('invoice.manMonth').toString(),
                            align: 'center',
                            slot: 'manMonth',
                            resizable: true,
                        },
                        {
                            title: this.$t('projectManagement.action').toString(),
                            align: 'center',
                            slot: 'action',
                            width: 150,
                            resizable: true,
                        }
                    ]
                }
            ]
            this.columnsFileTable = [
                {
                    title:this.$t('invoice.attachment').toString(),
                    align: 'center',   
                    children: [
                        {
                            title: this.$t('common.name').toString(),
                            align: 'center',
                            slot: 'fileName',
                            resizable: true,
                        },
                        {
                            title: this.$t('common.fileType').toString(),
                            align: 'center',
                            slot: 'type',
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
            this.allProject()
            this.allAssignee()
            this.allCustomer()
            if (this.$route.params.invoiceId) {
                let response = await this.$store.dispatch({
                    type: 'invoice/getInvoiceDetail',
                    invoiceId: this.$route.params.invoiceId
                })
                if(response.result.invoiceUserDetail && response.result.invoiceUserDetail.length > 0){
                    this.dataTable = response.result.invoiceUserDetail.map(element => {
                        this.idPeople = this.idPeople + 1
                        let data = element
                        data.isIncrease = false
                        data.idPeople = this.idPeople
                        return data
                    })
                    this.dataTable[this.dataTable.length - 1].isIncrease = true
                } else this.increaseData()
                response.result.mileStoneDetail = [response.result.mileStoneDetail]
                this.type = response.result.type
                this.dataSubmit.id = response.result.id
                this.dataSubmit.projectId = response.result.projectId
                this.dataSubmit.contractId = response.result.contractId
                this.dataSubmit.time = response.result.invoiceDate
                this.dataSubmit.value = response.result.value
                this.dataSubmit.currency = response.result.currency
                this.dataSubmit.assignee = response.result.assignee
                this.dataSubmit.invoiceType = response.result.type
                this.dataSubmit.invoiceName = response.result.name
                this.dataSubmit.status = response.result.status
                this.invoiceDate = response.result.invoiceDate
                await this.projectChanged(this.dataSubmit.projectId)
                await this.typeChanged(this.dataSubmit.invoiceType)
                // await this.getInvoiceComment()
                await this.getFiles(this.$route.params.invoiceId);
            }
            this.ruleValidate = {
                invoiceName: [
                    { required: true, message: this.$t('invoice.errInvoiceName'), trigger: 'blur' }
                ],
                projectId: [
                    { required: true, type: 'number', message: this.$t('invoice.errProject') , trigger: 'change' },
                ],
                invoiceType: [
                    { required: true, type: 'number', message: this.$t('invoice.errInvoiceType'), trigger: 'change' }
                ],
                contractId: [
                    { required: true, type: 'number', message: this.$t('invoice.errContract'), trigger: 'change' }
                ],
                time: [
                    { required: true, type: 'date', message: this.$t('invoice.errInvoiceDate'), trigger: 'change' },
                ],
                value: [
                    { required: true, type: 'number', message: this.$t('invoice.errValue'), trigger: 'blur' }
                ],
                assignee: [
                    { required: true, type: 'number', message: this.$t('invoice.errAssignee'), trigger: 'change' }
                ],
                currency: [
                    { required: true, type: 'number', message: this.$t('invoice.errCurrency'), trigger: 'change' }]
            }
            this.loading = false
        }
      private beforeMount() {
        this.fetchData()
      }
        typeChanged(value) {
            if(value==EProjectType.ODC || value == EProjectType.TNM) this.showTable = true
            else this.showTable = false
        }
        getInvoiceDate(date) {
            this.invoiceDate = date
        }
        private openCancelModalAdd() {
            this.isShowAddInvoice = !this.isShowAddInvoice
        }
        async deleteInvoice() {
            
            if(this.$route.params.invoiceId) {
                await this.$store.dispatch({
                    type: "invoice/deleteInvoice",
                    id: this.$route.params.invoiceId
                });
                this.$Message.info(this.$t('common.deleted'));
                this.cancel()
            }
        }
        cancel () {
            // this.dataTable = []
            this.$router.push({ name: 'invoice'}).catch()
        }
        async handleEditInvoice (name) {
            const form = await this.$refs[name] as any;     
            await form.validate(async(valid: boolean) => {
                if (valid) {
                    await this.editInvoice()
                } else {
                    this.$Message.error('Fail!');
                }
            })
        }
        async editInvoice() {
            let newData = this.dataTable.filter(element => element.userId !== null)
            let param: any = {
                people: newData
            }
            if(this.checkValidate && (this.dataSubmit.invoiceType == EContractType.ODC || this.dataSubmit.invoiceType == EContractType.TNM)) {
                this.dataSubmit.time = new Date(new Date(this.invoiceDate).setHours(0, 0, 0, 0)).toISOString()
                const dataInvoice = await this.$store.dispatch({
                    type: 'invoice/saveInvoice',
                    data: this.dataSubmit
                })
                param.invoiceId = dataInvoice.id
                if(newData.length > 0 && (this.dataSubmit.invoiceType == EContractType.ODC || this.dataSubmit.invoiceType == EContractType.TNM)) {
                    await this.$store.dispatch({
                        type: 'invoice/savePeopleInChangre',
                        data: param
                    })
                }
                this.$Message.info(this.$t('common.saved'));
                this.cancel()
            } 
            else if(((!this.checkValidate) && (this.dataSubmit.invoiceType == EContractType.ODC || this.dataSubmit.invoiceType == EContractType.TNM))) {
                this.$Message.error('Invoice type ODC/ TNM can be empty!');
            }
            else if((this.dataSubmit.invoiceType != EContractType.ODC || this.dataSubmit.invoiceType != EContractType.TNM)) {
                this.dataSubmit.time = new Date(new Date(this.invoiceDate).setHours(0, 0, 0, 0)).toISOString()
                const dataInvoice = await this.$store.dispatch({
                    type: 'invoice/saveInvoice',
                    data: this.dataSubmit
                })
                
                this.$Message.info(this.$t('common.saved'));
                this.cancel()
            }

        }
    async allProject() {
        let param = {"maxResultCount": 100}
        let data = await this.$store.dispatch({
            type:'project/getProject',
            data: param
        })
        data.result.items.forEach(itemData => {
            ListType.forEach(itemType => {
                if(itemData.type==itemType.id){ 
                itemData.typeName = itemType.name
                }
            })
            ListStatus.forEach(itemStatus => {
                if(itemData.status == itemStatus.id) {
                itemData.statusName = itemStatus.name
                }
            })  
        })
        this.projectList = data.result.items
    }
    async allAssignee() {
        let data = await this.$store.dispatch({
            type: 'invoice/getUserForInvoiceUser'
        })
        this.assigneeList = data.result
    }
    async projectChanged(projectId) {
        let data = await this.$store.dispatch({
            type: 'project/getProjectById',
            projectId: projectId
        })  
        this.contractList = data.result.projectContractDetails
    }
    async allCustomer() {
        let param = {"maxResultCount": 100}
        const data = await this.$store.dispatch({
            type: "customer/getAllPaging",
            params: param
        })
        this.customerList = data.items
    }
    async getFiles(invoiceId) {
        let data = await this.$store.dispatch({
            type: 'invoice/getFileByInvoice',
            invoiceId: invoiceId
        })  
        this.dataFileTable = data.result
    }
    async upload(e) {
        if(e){
            await this.getFiles(this.$route.params.invoiceId);
        }
    }
    async deleteFile(invoiceFileId) {
        let data = await this.$store.dispatch({
            type: 'invoice/deleteInvoiceFile',
            invoiceFileId: invoiceFileId
        })
        await this.getFiles(this.$route.params.invoiceId);
    }
    openFile(url){
        window.open(appconst.remoteServiceBaseUrl + url)
    }
    increaseData() {
        this.dataTable = this.dataTable.map(element => {
            let newData = element
            newData.isIncrease = false
            return newData
        })
        this.idPeople = this.idPeople + 1
        this.dataTable.push({
                    userId: null,
                    position: '',
                    rate: null,
                    manMonth: null,
                    isIncrease: true,
                    idPeople: this.idPeople
                })
    }
    closeData(data: any) {
        if (this.dataTable.length > 1) {
            this.dataTable = this.dataTable.filter(element => element.idPeople !== data.idPeople)
            this.dataTable[this.dataTable.length - 1].isIncrease = true
        } else {
            this.dataTable = this.dataTable.filter(element => element.idPeople !== data.idPeople)
            this.dataTable.push({
                    userId: null,
                    position: '',
                    rate: null,
                    manMonth: null,
                    isIncrease: true,
                    idPeople: this.idPeople
                })}
    }
    get checkValidate() {
        let validated = true
        this.dataTable.forEach(element => {
            if (!element.userId || !element.position || !element.rate || !element.manMonth) {
                validated = false
                if (!element.userId && !element.position && !element.rate && !element.manMonth) {
                    validated = true
                }
            }
        })
        return validated
    }
    change(data: any)  {
        this.dataTable = this.dataTable.map(element => {
            let newData = element
            if (data.idPeople === element.idPeople) {
                newData = data
            }
            return newData
        })
    }
}
</script>
<style lang="scss" scoped>
    .titleTable {
        margin-top: 5px;
        margin-bottom: 5px;
    }
    .code-row-bg{
        margin-top: 10px;
    }
    .addCustomer {
        width: 80%;
        margin-left: 20%;
    }
    .add-invoice{
        padding: 20px;
    }
    .error {
        text-align: center;
        color:red;
    }
    .icon {
        font-size: 32px;
        padding-left: 10px;
    }
    .item-contact {
        padding-bottom: 10px;
    }
</style>
<style lang="scss">
    .add-invoice {
        .ivu-select-dropdown {
            max-width: 100%;
        }
        .ivu-table-wrapper {
        overflow: visible;
        }
    }
</style>