<template>
    <div class="detail-task">
      <PageHeading>
        <template #header>{{$t('task.editTask')}}</template>
        <template #button>
        <Button name="task_save" class="button btn-add" @click="onSubmit('dataSubmit')">{{$t('common.save')}}</Button>
        <Button name="task_back" type="default" @click="onBack">{{$t('common.backToList')}}</Button>
        <Button name="task_cancel" type="default" @click="openConfirmDeletePopup">{{$t('common.delete')}}</Button>
        </template>
      </PageHeading>
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
                <FormItem :label="$t('task.entityAssignmentDtos')" prop="entityId">
                    <Select name="taskEntityId_input" v-model="dataSubmit.entityId" :placeholder="$t('common.placeholderSelect',{field: $t('task.entityAssignmentDtos')})">
                        <Option v-for="(item, index) of entityList" :key="index" :value="item.id">{{item.name}}</Option>
                    </Select>
                </FormItem>
                <FormItem :label="$t('task.userId')" prop="userId">
                    <Select name="taskUserId_input" v-model="dataSubmit.userId" :placeholder="$t('common.placeholderSelect',{field: $t('task.userId')})">
                        <Option v-for="(item, index) of userIds" v-bind:key="index" :value="item.userId">{{item.userName}}</Option>
                    </Select>
                </FormItem>
                <FormItem >                    
                    <Checkbox name="taskIsActive_input" v-model="dataSubmit.isActive">Active</Checkbox>
                </FormItem>
            </Form>
             <Modal v-model="isShowConfirmDelete" :title="$t('common.notification')">
                <p>{{$t('common.confirmDeleteNotice')}}</p>
                <Row slot="footer" class="button-zone">
                <Button
                    name="task_confirmDelete"
                    class="button btn-add"
                    size="default"
                    @click="deleteTask"
                  >{{$t('common.accept')}}</Button>
                <Button
                    name="task_cancelDelete"
                    type="error"
                    size="default"
                    ghost
                    @click="isShowConfirmDelete=false"
                >{{$t('common.cancel')}}</Button>
              </Row>
            </Modal>
      </div>
    </div>
</template>
<script lang="ts">
    import { Component, Vue, Prop, Emit, Watch } from 'vue-property-decorator';
    import Util from '@/lib/util'
    import AbpBase from '@/lib/abpbase'
    import PageHeading from "@/components/heading/heading.vue";
    import appconst from '../../../lib/appconst'
    import { EPriorityList, EEntityTypeList, EStatus, IEntityList, ICommonList } from '../../../store/entities/task';

    @Component({
        name: 'editTask',
        components: {
          PageHeading
        }
    })
export default class editTask extends AbpBase{
    private isClickSubmit: boolean = false
    userIds: Number[] = []
    priorityList = EPriorityList
    entityList:any = []
    assigneeList = []
    statusList = EStatus
    ruleValidate: any = {}
    isShowConfirmDelete = false
    entityAssignmentDtos: number[] = []
    dataSubmit:any = {
        name: "",
        description: "",
        isActive: false,
        status: null,
        deadline: "",
        priority: null,
        entityId: null,
        id: null,
        userId: null
    } 

    @Watch("$route")
    getEvent() {
        this.fetchData();
    }
    async deleteTask() {
        if(this.dataSubmit.id) {
            await this.$store.dispatch({
            type: "task/deleteTask",
            id: this.dataSubmit.id
            });
        }
        this.$Message.info(this.$t('common.deleted'));
        this.isShowConfirmDelete = false
        this.onBack()
    }
    private async fetchData() {
        this.dataSubmit =  {
            name: "",
            description: "",
            isActive: false,
            status: null,
            deadline: "",
            priority: null,
            entityId: null,
            id: null,
            userId: null
        }
        if (this.$route.params.Id) {
            let param = {
                Id: this.$route.params.Id
            }
            let response = await this.$store.dispatch({
                type: "task/getTask",
                param
            });
            const dataFinal = response
            for(let key in this.dataSubmit) {
                if(key)
                if( key == 'userId') {
                    this.dataSubmit['userId'] = response.user.userId
                }else {
                    this.dataSubmit[key] = dataFinal[key]
                }
            }
            let entityList = await this.$store.dispatch({
                type: 'task/getAllEntity'
            })
            this.entityList = entityList.result
            let assigneeList = await this.$store.dispatch({
                type: 'project/getAssigneeList'
            })
            this.userIds = assigneeList.result
            this.dataSubmit.userId = response.user.userId
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
                entityId: [
                    { required: true, type: 'number', message: this.$t('task.entityAssignmentDtos') , trigger: 'change' },
                ],
                userId: [
                    { required: true, type: 'number', message: this.$t('task.userId') , trigger: 'change' },
                ]
            }
        }
    }
    private beforeMount() {
        this.fetchData()
    }
    private async onSubmit(name) {
    const form = await this.$refs[name] as any;     
    await form.validate(async(valid: boolean) => {
        if (valid) {
            await this.edit()
        } else {
            this.$Message.error('Fail!');
        }
    })
    }
    async edit() {
        const item = this.entityList.find(el => el.id === this.dataSubmit.entityId);
        const data = {
                id: this.dataSubmit.id,
                status: this.dataSubmit.status,
                name: this.dataSubmit.name,
                isActive: this.dataSubmit.isActive,
                deadline: this.dataSubmit.deadline,
                description: this.dataSubmit.description,
                priority: this.dataSubmit.priority,
                entityAssignmentDtos: [
                  {
                   entityId: item.id,
                   entityType: item.entityType
                  }
                    ],
                userIds: [this.dataSubmit.userId],
        } 
        await this.$store.dispatch({
            type: 'task/saveTask',
            data: data
        }) 
        this.$Message.info(this.$t('common.saved'));
        await this.onBack()
    }
    private onBack() {
        this.$router.push({name: 'task'}).catch(()=>{})
    }
    openConfirmDeletePopup() {
    this.isShowConfirmDelete = true
    }

}
</script>
<style lang="scss" scoped>  
    .code-row-bg{
        margin-top: 10px;
    }
    .detail-task{
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
</style>
