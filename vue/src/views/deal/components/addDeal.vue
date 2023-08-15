<template>
    <div>
        <div class="add-deal">
             <Row type="flex" class="code-row-bg" justify="space-around" align="middle">
                    <Col span="23" align="right">
                        <Button 
                            name="deal_add"
                            class="button btn-add"
                            @click="handleAddProject('dataSubmit')"
                            :loading="loading"
                        >
                            {{$t('common.save')}}
                        </Button>
                    </Col>

                    <Col span="1" align="right">
                        <Button 
                            name="deal_cancel"
                            type="default"
                            @click="cancel"
                        >
                            {{$t('common.cancel')}}
                        </Button>
                    
                    </Col>
                </Row>
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
                 <FormItem >
                     <Button class="button btn-add" v-if="!isShow" @click="addCustomer()">{{$t('customerManagement.addCustomer')}}</Button>
                     <Icon v-else @click="collapse()" type="md-arrow-dropup"  size="25" />
                    <Modal
                        v-model="isShow"
                        :footer-hide=true
                        width="65%">
                        <div slot="header" align="center"><h1>{{$t('customerManagement.addCustomer')}}</h1></div>
                        <addCustomer @addedClient="updateDropdown" @onSubmit="addCustomer" v-if="isShow"/>
                        <div slot="footer" class="button-zone" align="center">
                        </div>
                    </Modal>


                </FormItem>
                
                <FormItem :label="$t('deal.status')" prop="status">
                    <Select name="dealStatus_input" v-model="dataSubmit.status" :placeholder="$t('common.placeholderEnter',{field: $t('deal.status')})">
                        <Option v-for="(item, index) of statusList" v-bind:key="index" :value="item.id">{{item.name}}</Option>
                    </Select>
                </FormItem>

                <FormItem :label="$t('task.priority')" prop="priority">
                    <Select name="dealPriority_input" v-model="dataSubmit.priority" :placeholder="$t('common.placeholderEnter',{field: $t('task.priority')})">
                        <Option v-for="(item, index) of priorityList" v-bind:key="index" :value="item.id">{{item.name}}</Option>
                    </Select>
                </FormItem>

                <FormItem :label="$t('deal.amount')" prop="amount">
                    <InputNumber name="dealAmount_input" style="width: 100%" v-model="dataSubmit.amount" :min="0" :placeholder="$t('common.placeholderEnter', {field: $t('deal.amount')})"></InputNumber>
                </FormItem>
                <FormItem :label="$t('deal.dealStartDate')" prop="dealStartDate">
                    <Col :md="4">
                        <DatePicker name="dealStartDate_input" v-model="dataSubmit.dealStartDate" :clearable="true"></DatePicker>
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
                            <Select :name="`dealSkillId${index}_input`" v-model="item['skillId']" @change="showData()" :placeholder="$t('common.placeholderEnter', {field: $t('deal.skill')})">
                                <Option v-for="(skill, i) of skills" v-bind:key="i" :value="skill.id">{{skill.skillName}}</Option>
                            </Select>
                        </Col>
                        <Col :md="8">
                            <Row class="">
                                <Col :md="12" class="text-center">
                                    <label>{{$t('deal.level')}}</label>
                                </Col>
                                <Col :md="12">
                                    <Select :name="`dealLevelId${index}_input`" v-model="item['levelId']" :placeholder="$t('common.placeholderEnter', {field: $t('deal.level')})">
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
                            <Icon v-if="dealLevels.length > 1" type="md-close" size="25" @click="removeRowSkill(index)"/>
                        </Col>
                    </Row>
                    <Row class="">
                        <Icon  type="md-add" size="25" @click="addRowSkill()"/>
                    </Row>
                </FormItem>
            </Form>
        </div>
    </div>
</template>
<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import AbpBase from '@/lib/abpbase'
    import {ICommonList, ListType, ListStatusDeal, IAssignee, ISaveDeal} from '@/store/entities/project';
    import AddCustomer from "../../setting/customer/components/addCustomer.vue";
    import { EPriorityList } from '@/store/entities/task';
import moment from 'moment';
    @Component({
         components: { AddCustomer},
        name: 'AddDeal'
    })
    export default class AddDeal extends AbpBase{
        value:string = ""
        error:boolean = false
        loading:boolean = false
        isShow: boolean = false
        assignee: Number[] = []
        assigneeList: IAssignee[] = []
        statusList = []
        clientData: ICommonList[] = []
        dataSubmit = {} as ISaveDeal
        ruleValidate: any = {}
        priorityList = EPriorityList
        dealLevels: any[] = []
        levels: any[] = [];
        skills: any[] = [];
        async created() {
            this.addRowSkill()
            this.dataSubmit =  {
                ownerId: null,
                description: "",
                clientId: null,
                status: null,
                priority: null,
                amount: null,
                name: "",
                dealStartDate: moment(new Date()).format('YYYY/MM/DD'),
                dealLastFollow: null,
                dealDetail: null
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
            this.getClient();
            
            await this.getDropdownLevels();
            await this.getDropdownSkills();

            let assigneeList = await this.$store.dispatch({
                type: 'project/getAssigneeList'
            }) 
            this.assigneeList = assigneeList.result
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
        }
        deactivated() {
            (this.$refs['dataSubmit'] as any ).resetFields();
        }

        activated() {
            (this.$refs['dataSubmit'] as any ).resetFields();
        }

        async getClient() {
            let clientData = await this.$store.dispatch({
            type:'project/getClient'
            })
            this.clientData = clientData.result
        }
        cancel () {
            this.$Message.info(this.$t('common.cancel'));
            this.isShow = false;
            this.$router.push({ name: 'deal' }).catch(() => { })  
        }
        async updateDropdown(clientInfor: any) {
            await this.getClient()
            this.dataSubmit.clientId = clientInfor['id'];
        }
        async handleAddProject(name) {
            const form = await this.$refs[name] as any;
            
            this.dataSubmit.dealDetail = this.cleanData()
            await form.validate(async(valid: boolean) => {
                
                if (valid) {
                    this.dataSubmit.id = 0
                    this.dataSubmit.dealStartDate = this.convertedDate(this.dataSubmit.dealStartDate)
                    this.dataSubmit.dealLastFollow = this.convertedDate(this.dataSubmit.dealLastFollow)
                    await this.$store.dispatch({
                        type: 'deal/saveDeal',
                        data: this.dataSubmit
                    }) 
                    this.$Message.info(this.$t('common.saved'));
                    // this.$emit("showAdd",false)
                    this.$router.push({ name: 'deal'}).catch(()=>{})
                } else {
                    this.$Message.error('Fail!');
                }
            })
        }

        addCustomer() {
            this.isShow = !this.isShow
            this.getClient();
        }
        collapse() {
            this.isShow = !this.isShow
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
<style scoped>  
    .code-row-bg{
        margin-top: 10px;
        margin-bottom: 10px;
    }
    .add-deal{
        padding: 20px;
    }
    .error {
        text-align: center;
        color:red;
    }
    .d-flex{
        display: flex;
    }
    .ivu-date-picker{
        display: block;
    }
    .text-center{
        text-align: center;
    }
</style>
