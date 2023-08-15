<template>
    <div>
        <div class="add-task">
            <Form ref="dataSubmit" :model="dataSubmit" :rules="ruleValidate" :label-width="200" label-position="left">
                <FormItem :label="$t('task.name')" prop="name">                    
                    <Input name="taskName_input" v-model="dataSubmit.name" :placeholder="$t('common.placeholderEnter',{field: $t('task.name')})" />
                </FormItem>
                <FormItem :label="$t('task.description')" prop="description">                    
                    <Input name="taskDescription_input" v-model="dataSubmit.description" :placeholder="$t('common.placeholderEnter',{field: $t('task.description')})" />
                </FormItem>
                <FormItem :label="$t('task.deadline')" prop="deadline">
                    <DatePicker name="taskDeadline_input" style="width: 100%" type="date" v-model="dataSubmit.deadline" :placeholder="$t('common.placeholderSelect',{field: $t('task.deadline')})" ></DatePicker>
                </FormItem>
                <FormItem :label="$t('task.status')" prop="status">
                    <Select name="taskStatus_input" v-model="dataSubmit.status" :placeholder="$t('common.placeholderSelect',{field: $t('task.status')})">
                        <Option v-for="(item, index) of statusList" v-bind:key="index" :value="item.id">{{item.name}}</Option>
                    </Select>
                </FormItem>
                <FormItem :label="$t('task.priority')" prop="priority">
                    <Select name="taskPriority_input" v-model="dataSubmit.priority" :placeholder="$t('common.placeholderSelect',{field: $t('task.priority')})">
                        <Option v-for="(item, index) of priorityList" v-bind:key="index" :value="item.id">{{item.name}}</Option>
                    </Select>
                </FormItem>
                <FormItem :label="$t('task.entityAssignmentDtos')" prop="entityAssignment">
                    <Select name="taskEntityAssignment_input" v-model="dataSubmit.entityAssignment" filterable clearable
                     :placeholder="$t('common.placeholderSelect',{field: $t('task.entityAssignmentDtos')})">
                        <Option v-for="(item, index) of entityList" :key="index" :value="item.id">{{item.name}}</Option>
                    </Select>
                </FormItem>
                <FormItem :label="$t('task.userId')" prop="userIds">
                    <Select name="taskUserId_input" v-model="dataSubmit.userIds"  :placeholder="$t('common.placeholderSelect',{field: $t('task.userId')})">
                        <Option v-for="(item, index) of assigneeList" v-bind:key="index" :value="item.userId">{{item.userName}}</Option>
                    </Select>
                </FormItem>
                <FormItem >                    
                    <Checkbox name="taskIsActive_input" v-model="dataSubmit.isActive">Active</Checkbox>
                </FormItem>
                <Row type="flex" class="code-row-bg" justify="space-around" align="middle">
                    <Col span="11" align="right">
                        <Button 
                            name="task_save"
                            class="button btn-add"
                            size="large"
                            @click="handleAddProject('dataSubmit')"
                            :loading="loading"
                        >
                            {{$t('common.add')}}
                        </Button>
                    </Col>
                    <Col span="12" align="left">
                        <Button 
                            name="task_cancel"
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
    import { EPriorityList, EEntityTypeList, EStatus, IEntityList, ICommonList } from '../../../store/entities/task';
    import AbpBase from '@/lib/abpbase'
import { IAssignee } from '@/store/entities/project';
import moment from 'moment';
    @Component({
        name: 'AddTask'
    })
    export default class AddTask extends AbpBase{
        value:string = ""
        error:boolean = false
        loading:boolean = false
        isShow: boolean = false
        assigneeList: IAssignee[] = []
        priorityList = EPriorityList
        entityList: any[] = []
        // assigneeList = []
        statusList = EStatus
        entityAssignmentDtos: number[] = []
        dataSubmit:any = {
            name: "",
            description: "",
            isActive: false,
            status: null,
            deadline: "",
            priority: null,
            entityAssignment: null,
            userIds: null
        } 
        ruleValidate: any = {}
        async created() {
            this.ruleValidate = {
                name: [
                    { required: true, message: this.$t('task.name') , trigger: 'blur' },
                ],
                // description: [
                //     { required: true, message: this.$t('task.description') , trigger: 'blur' },
                // ],
                deadline: [
                    { required: true, type: 'date', message: this.$t('task.deadline') , trigger: 'change' },
                ],
                status: [
                    { required: true, type: 'number', message: this.$t('task.status') , trigger: 'change' },
                ],
                priority: [
                    { required: true, type: 'number', message: this.$t('task.priority') , trigger: 'change' },
                ],
                entityAssignment: [
                    { required: true, type: 'number', message: this.$t('task.entityAssignmentDtos') , trigger: 'change' },
                ],
                userIds: [
                    { required: true, type: 'number',  message: this.$t('task.userId') , trigger: 'change' },
                ]
            }
            
            let entityList = await this.$store.dispatch({
                type: 'task/getAllEntity'
            })
            this.entityList = entityList.result
            let assigneeList = await this.$store.dispatch({
                type: 'project/getAssigneeList'
            })
            this.assigneeList = assigneeList.result
        }
        cancel () {
            this.$Message.info(this.$t('common.cancel'));
            this.$emit("showAdd",false)
        }
        // getEntity() {    
        //     this.dataSubmit.entityAssignmentDtos = []
        //     this.entityAssignmentDtos.forEach((id:number) =>{
        //         this.entityList.forEach((item:any)=>{
        //             if(id==item.id) {
        //                 let ite:any = {
        //                     entityId: item.id,
        //                     entityType: item.entityType
        //                 }
        //                 this.dataSubmit.entityAssignmentDtos.push(ite)
        //             }
        //         })
        //     })
        // }
        async handleAddProject(name) {            
            const form = await this.$refs[name] as any;
            const item = this.entityList.find(el => el.id === this.dataSubmit.entityAssignment);
            let data = {
                status: this.dataSubmit.status,
                name: this.dataSubmit.name,
                isActive: this.dataSubmit.isActive,
                deadline: moment(this.dataSubmit.deadline).format('MM/DD/YYYY').toString(),
                description: this.dataSubmit.description,
                priority: this.dataSubmit.priority,
                entityAssignmentDtos: [
                          {
                   entityId: item.id,
                   entityType: item.entityType
                  }
                    ],
                userIds: [this.dataSubmit.userIds],
            }
            await form.validate(async(valid: boolean) => {
                if (valid) {
                    await this.$store.dispatch({
                        type: 'task/saveTask',
                        data: data
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
    .add-task{
        padding: 20px;
    }
    .error {
        text-align: center;
        color:red;
    }
</style>
