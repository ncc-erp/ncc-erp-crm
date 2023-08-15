<template>
    <div>
        <Modal
          v-model="isShowAddCustomer"
          :footer-hide=true
          width="65%">
        <div slot="header" align="center"><h1>{{$t('customerManagement.addCustomer')}}</h1></div>
        <addCustomer v-if="isShowAddCustomer" @onSubmit="openCancelModalAdd"/>
        <div slot="footer" class="button-zone" align="center">
        </div>
        </Modal>
        <div class="add-project">
            <Form ref="dataSubmit" :model="dataSubmit" :rules="ruleValidate" :label-width="200" label-position="left" class="add-invoice">
                <FormItem :label="$t('projectManagement.projectName')" prop="name">
                    <Input name="projectName_input" v-model="dataSubmit.name" :placeholder="$t('common.placeholderEnter',{field: $t('projectManagement.projectName')})" />
                </FormItem>
                <FormItem :label="$t('projectManagement.projectCode')" prop="code">
                    <Input name="projectCode_input" v-model="dataSubmit.code" :placeholder="$t('common.placeholderEnter',{field: $t('projectManagement.projectCode')})" />
                </FormItem>
                <FormItem :label="$t('projectManagement.projectType')" prop="type">                    
                    <Select name="projectType_input" v-model="dataSubmit.type" :placeholder="$t('common.placeholderEnter',{field: $t('projectManagement.projectType')})">
                        <Option v-for="(item, index) in listType" :value="item.id" :key="index">{{ item.name }}</Option>
                    </Select>
                </FormItem>
                <FormItem :label="$t('projectManagement.assignee')" prop="users">                    
                    <Select name="projectAssigneeId_input" v-model="dataSubmit.users" filterable multiple :placeholder="$t('common.placeholderEnter',{field: $t('projectManagement.assignee')})">
                        <Option v-for="item in assigneeList" :value="item.userId" :key="item.userId">{{ item.userName }}</Option>
                    </Select>
                </FormItem>
                <FormItem :label="$t('projectManagement.client')" prop="clientId">
                    <Row type="flex">
                        <Col span="20">
                            <Select name="projectClientId_input" v-model="dataSubmit.clientId" filterable :placeholder="$t('common.placeholderSelect',{field: $t('projectManagement.client')})">
                                <Option v-for="(item, index) of clientData" v-bind:key="index" :value="item.id">{{item.name}}</Option>
                            </Select>
                        </Col>
                        <Col span="4">
                            <Button name="projectCustomer_add" class="addCustomer" @click="openCancelModalAdd"><h3>+</h3></Button>
                        </Col>
                    </Row>
                </FormItem>
                <FormItem :label="$t('projectManagement.status')" prop="status">
                    <Select name="projectStatus_input" v-model="dataSubmit.status" :placeholder="$t('common.placeholderEnter',{field: $t('projectManagement.status')})">
                        <Option v-for="(item, index) of statusList" v-bind:key="index" :value="item.id">{{item.name}}</Option>
                    </Select>
                </FormItem>
                <FormItem :label="$t('projectManagement.description')" prop="description">
                    <Input name="projectDescription_input" v-model="dataSubmit.description" :autosize="{minRows: 5}" :placeholder="$t('common.placeholderEnter',{field: $t('projectManagement.description')})" type="textarea" />
                </FormItem>
                <Row type="flex" class="code-row-bg" justify="space-around" align="middle">
                    <Col span="11" align="right">
                        <Button 
                            name="project_save"
                            type="error"
                            size="large"
                            @click="handleAddProject('dataSubmit')"
                            :loading="loading"
                        >
                            {{$t('common.add')}}
                        </Button>
                    </Col>
                    <Col span="12" align="left">
                        <Button 
                            name="project_cancel"
                            type="default"
                            size="large"
                            @click="cancel"
                        >
                            {{$t('common.cancel')}}
                        </Button>
                    
                    </Col>
                </Row>
            </Form>
        </div>
    </div>
</template>
<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import AbpBase from '@/lib/abpbase'
    import AddCustomer from "../../customer/components/addCustomer.vue";
    import {ICommonList, ListType, ListStatus, ListStatusProject, IAssignee, ISaveProject} from '@/store/entities/project'
    @Component({
        name: 'AddProject',
        components: {
            AddCustomer
        }
    })
    export default class AddProject extends AbpBase{
        value:string = ""
        error:boolean = false
        loading:boolean = false
        isShow: boolean = false
        isShowAddCustomer: boolean = false
        assignee: Number[] = []
        assigneeList: IAssignee[] = []
        statusList: ICommonList[] = ListStatusProject
        clientData: ICommonList[] = []
        listType: ICommonList[] = ListType
        dataSubmit: ISaveProject = {    
            name: "",
            code: "",   
            type: null,
            clientId: null,
            status: null,
            description: "",
            users: []
        }
        ruleValidate: any = {}
        async created() {
            this.ruleValidate = {
                name: [
                    { required: true, type: 'string', message: this.$t('projectManagement.errName'), trigger: 'blur' }
                ],
                code: [
                    { required: true, message: this.$t('projectManagement.errCode') , trigger: 'blur' },
                ],
                type: [
                    { required: true, type: 'number', message: this.$t('projectManagement.errType'), trigger: 'change' }
                ],
                status: [
                    { required: true, type: 'number',message: this.$t('projectManagement.errStatus'), trigger: 'change' }
                ],
                description: [
                    { required: true, message: this.$t('projectManagement.errDescription'), trigger: 'blur' },
                ],
                users: [
                    { required: true, type: 'array', min: 1, message: this.$t('projectManagement.errAssignee'), trigger: 'change' }
                ],
                clientId: [
                    { required: true, type: 'number', message: this.$t('projectManagement.errClient'), trigger: 'change' }
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
        }
        handleAddCustomer() {
            this.$Message.info(this.$t('common.saved'));
            this.isShowAddCustomer=false
        }
        private openCancelModalAdd() {
            this.isShowAddCustomer = !this.isShowAddCustomer
        }
        cancel () {
            this.$Message.info(this.$t('common.cancel'));
            this.$emit("showAdd",false)
        }
        async handleAddProject(name) {
            const form = await this.$refs[name] as any;     
            await form.validate(async(valid: boolean) => {
                if (valid) {
                    await this.$store.dispatch({
                        type: 'project/addProject',
                        data: this.dataSubmit
                    }) 
                    this.$Message.info(this.$t('common.saved'));
                    this.$emit("showAdd",false)
                } else {
                    this.$Message.error('Fail!');
                }
            })
            
        }
    }
</script>
<style scoped>  
    .code-row-bg{
        margin-top: 10px;
    }
    .addCustomer {
        width: 80%;
        margin-left: 20%;
    }
    .add-project{
        padding: 20px;
    }
    .error {
        text-align: center;
        color:red;
    }
</style>
