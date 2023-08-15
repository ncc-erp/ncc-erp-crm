<template>
    <div>
         <Modal
          v-model="isShowAddInvoice"
          :footer-hide=true
          width="65%">
        <div slot="header" align="center"><h1>{{$t('customerManagement.addCustomer')}}</h1></div>
        <addCustomer v-if="isShowAddInvoice" @onSubmit="openCancelModalAdd"/>
        <div slot="footer" class="button-zone" align="center">
        </div>
        </Modal>
        <!-- <div class="add-invoice"> -->
        <Form ref="dataAddInvoice" :model="dataAddInvoice" :rules="ruleValidate" :label-width="200" label-position="left" class="add-invoice">
            <FormItem :label="$t('invoice.invoiceName')" prop="invoiceName">
                <Input name="invoiceName_input" v-model="dataAddInvoice.invoiceName" placeholder="Enter your name"></Input>
            </FormItem>
            <FormItem :label="$t('invoice.projectName')" prop="projectId">
                <Select name="invoiceProjectId_input" v-model="dataAddInvoice.projectId" clearable @on-change="projectChanged" filterable :placeholder="$t('common.placeholderSelect',{field: $t('invoice.projectName')})">
                    <Option v-for="(item, index) in projectList" :value="item.id" :key="index">{{ item.name }}</Option>
                </Select>
            </FormItem>
            <FormItem :label="$t('invoice.type')" prop="invoiceType">                
                <Select name="invoiceType_input" v-model="dataAddInvoice.invoiceType" clearable @on-change="typeChanged" :placeholder="$t('common.placeholderSelect',{field: $t('invoice.type')})">
                    <Option v-for="(item, index) in listType" :value="item.id" :key="index">{{ item.name }}</Option>
                </Select>
            </FormItem>
            <FormItem :label="$t('invoice.contract')" prop="contractId">                
                <Select name="invoiceContractId_input" v-model="dataAddInvoice.contractId" clearable filterable :placeholder="$t('common.placeholderSelect',{field: $t('invoice.contract')})">
                    <Option v-for="item in contractList" :value="item.contractId" :key="item.contractId">{{ item.contractName }}</Option>
                </Select>
            </FormItem>
            <FormItem :label="$t('invoice.invoiceDate')" prop="time">                
                <DatePicker name="invoiceTime_input" type="date" v-model="dataAddInvoice.time" style="width: 100%" @on-change="getInvoiceDate" :placeholder="$t('common.placeholderSelect',{field: $t('invoice.invoiceDate')})"></DatePicker>
            </FormItem>
            <FormItem :label="$t('invoice.value')" prop="value">                
                <InputNumber name="invoiceValue_input" style="width: 100%" :placeholder="$t('common.placeholderSelect',{field: $t('invoice.value')})" v-model="dataAddInvoice.value" ></InputNumber>
            </FormItem>
            <FormItem :label="$t('invoice.currency')" prop="currency">                
                <Select name="invoiceCurrency_input" v-model="dataAddInvoice.currency" clearable filterable :placeholder="$t('common.placeholderSelect',{field: $t('invoice.currency')})">
                    <Option v-for="item in currencyList" :value="item.id" :key="item.id">{{ item.name }}</Option>
                </Select>
            </FormItem>        
            <FormItem :label="$t('invoice.assignee')" prop="assignee">                
                <Select name="invoiceAssigneeId_input" v-model="dataAddInvoice.assignee" clearable filterable :placeholder="$t('common.placeholderSelect',{field: $t('invoice.assignee')})">
                    <Option v-for="item in assigneeList" :value="item.userId" :key="item.userId">{{ item.userName }}</Option>
                </Select>
            </FormItem>        
            
            <Row class="code-row-bg" align="middle" v-if="showTable">
                <p class="titleTable">{{$t('invoice.invoice')}}</p>
                <Table name="addInvoice_tbl" border :columns="columns" :data="dataTable" style="width: 100%">
                    <template slot="employee">
                        <Select name="invoiceEmployeeId_input" width="100" v-model="employee">
                            <Option v-for="item in assigneeList" :value="item.userId" :key="item.userId">{{ item.userName }}</Option>
                        </Select>
                    </template>
                    <template slot="position" style="width: 100%">
                        <Input name="invoicePosition_input" style="width: 100%" :placeholder="$t('invoice.position')" v-model="position" ></Input>
                    </template>
                    <template slot="rate" style="width: 100%">
                        <Input name="invoiceRate_input" style="width: 100%" :placeholder=" $t('invoice.rate')" v-model="rate" ></Input>
                    </template>
                    <template slot="manMonth" style="width: 100%">
                        <Input name="invoiceManMonth_input" style="width: 100%" :placeholder="$t('invoice.manMonth')" v-model="manMonth" ></Input>
                    </template>
                </Table>
            </Row> 
            <Row type="flex" class="code-row-bg" justify="space-around" align="middle">
                <Col span="11" align="right">
                    <Button 
                        name="invoice_save"
                        type="error"
                        size="large"
                        @click="handleAddInvoice('dataAddInvoice')"
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
        </FORM>
        <!-- </div> -->
    </div>
</template>
<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import AbpBase from '@/lib/abpbase'
    import {ICommonList, ListType, ListStatus, IInvoiceUserDetail, IDataAddInvoice, IdataSavePeopleInCharge, EProjectType, CurrencyType, IAssignee, ISaveProject} from '@/store/entities/project'
    import UploadFile from '../../../components/upload-file/upload-file.vue'
import { EContractType } from '@/store/entities/contract-type';
    @Component({
        name: 'AddInvoice',
        components: {UploadFile}
    })
    export default class AddInvoice extends AbpBase{
        error:boolean = false
        loading:boolean = false
        showTable:boolean = false
        isShowAddInvoice: boolean = false

        dataAddInvoice? = {} as IDataAddInvoice
        ruleValidate:any = {}
        
        columns: any = []
        employee: number = null
        manMonth: number = null
        rate: number = null
        position: string = ""
        
        invoiceDate: string = ""

        projectList: ICommonList[] = []
        listType: ICommonList[] = ListType
        contractList: ICommonList[] = []
        currencyList: ICommonList[] = CurrencyType
        customerList: ICommonList[] = []
        assigneeList: IAssignee[] = []
        
        dataTable: IInvoiceUserDetail[] = []
        private invoiceName: string = ''

        async created() {
            this.dataAddInvoice = {
                invoiceName: '',
                projectId: null,
                invoiceType: null,
                contractId: null,
                time: '',
                value: null,
                assignee: null,
                currency: null,
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
            this.allProject()
            this.allAssignee()
            this.allCustomer()
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
                        }
                    ]
                }
            ]
            this.dataTable.push({
                userId: this.employee,
                position: this.position,
                rate: this.rate,
                manMonth: this.manMonth
            })
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
      
        cancel () {
            this.$Message.info(this.$t('common.cancel'));
            this.$emit("showAdd",false)
        }
        async handleAddInvoice (name) {
            const form = await this.$refs[name] as any;     
            await form.validate(async(valid: boolean) => {
                if (valid) {
                    this.loading = true
                    await this.addInvoice()
                    this.loading = false
                } else {
                    this.$Message.error('Fail!');
                }
            })
        }
        async addInvoice() {
            this.dataTable = []
            this.dataTable.push({
                userId: this.employee,
                position: this.position,
                rate: this.rate,
                manMonth: this.manMonth
            })
            let param: any = {
                people: this.dataTable
            }
            
            if(this.employee && this.position && this.rate && this.manMonth && (this.dataAddInvoice.invoiceType == EContractType.ODC || this.dataAddInvoice.invoiceType == EContractType.TNM)) {
                this.dataAddInvoice.time = new Date(new Date(this.invoiceDate).setHours(0, 0, 0, 0)).toISOString()
                const dataInvoice = await this.$store.dispatch({
                    type: 'invoice/saveInvoice',
                    data: this.dataAddInvoice
                })
                param.invoiceId = dataInvoice.id
                if(this.dataTable.length > 0 && (this.dataAddInvoice.invoiceType == EContractType.ODC || this.dataAddInvoice.invoiceType == EContractType.TNM)) {
                    await this.$store.dispatch({
                        type: 'invoice/savePeopleInChangre',
                        data: param
                    })
                }
                this.$Message.info(this.$t('common.saved'));
                this.$emit("showAdd",false)
            } 
            else if(((!this.employee || !this.position || !this.rate || !this.manMonth) && (this.dataAddInvoice.invoiceType == EContractType.ODC || this.dataAddInvoice.invoiceType == EContractType.TNM))) {
                this.$Message.error('Invoice type ODC/ TNM can be empty!');
            }
            else if((this.dataAddInvoice.invoiceType != EContractType.ODC || this.dataAddInvoice.invoiceType != EContractType.TNM)) {
                this.dataAddInvoice.time = new Date(new Date(this.invoiceDate).setHours(0, 0, 0, 0)).toISOString()
                const dataInvoice = await this.$store.dispatch({
                    type: 'invoice/saveInvoice',
                    data: this.dataAddInvoice
                })
                
                this.$Message.info(this.$t('common.saved'));
                this.$emit("showAdd",false)
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