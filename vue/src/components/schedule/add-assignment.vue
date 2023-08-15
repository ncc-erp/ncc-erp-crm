<template>
    <div>
      <Modal
        v-model="isShow"
        width="40%"
        @on-visible-change="cancel"
      >
      <h2>{{$t('team.addAssignment')}}</h2>
      <Form ref="formData" :model="fromData" :rules="ruleValidate" :label-width="100" label-position="left" class="add-invoice">
        <FormItem :label="$t('common.name')" prop="name">
          <Input name="assignmentName_input" v-model="fromData.name"  placeholder="Enter something..." />
        </FormItem>
        <FormItem :label="$t('assignment.reporter')" >
          <p>{{reporter}}</p>
        </FormItem>
        <FormItem :label="$t('assignment.assigned')" prop="assigned" >
          <Select name="assignmentAssigneeId_input" v-model="userId" style="width:200px" filterable @on-change="changeAssigned">
            <Option v-for="item in allUser" :value="item.id" :key="item.id">{{ item.fullName }}</Option>
          </Select>
        </FormItem>
        <FormItem :label="$t('assignment.priority')" prop="priority">
          <Select name="assignmentPriority_input" v-model="fromData.priority" style="width:200px" >
            <Option v-for="item in EPriority" :value="item.id" :key="item.id">{{ item.name }}</Option>
          </Select>
        </FormItem>
        <FormItem :label="$t('assignment.deadline')" prop="deadline">
          <Col >
            <DatePicker name="assignmentDeadline_input" type="date" placeholder="Select date" style="width: 200px" v-model="fromData.deadline"></DatePicker>
          </Col>
        </FormItem>
        <FormItem :label="$t('common.description')" >
          <Input name="assignmentDescription_input" v-model="fromData.description" type="textarea" :rows="4" placeholder="Enter something..." />
        </FormItem>
        <FormItem :label="$t('common.status')" prop="status">
          <Select name="assignmentStatus_input" v-model="fromData.status" style="width:200px">
            <Option v-for="item in EAssignmentStatus" :value="item.id" :key="item.id">{{ item.name }}</Option>
          </Select>
        </FormItem>
        <Row type="flex" class="code-row-bg" justify="space-around" align="middle">
            <Col span="11" align="right">
                <Button 
                  name="assignment_save"
                  class="button btn-add"
                  @click="saveAssignment('formData')"
                >
                  {{$t('common.save')}}
                </Button>
            </Col>
            <Col span="12" align="left">
                <Button 
                  name="assignment_cancel"
                  type="default"
                  @click="cancel"
                >
                  {{$t('common.cancel')}}
                </Button>
            </Col>
        </Row>
      </Form>
      <div slot="footer" class="button-zone" align="center">
      </div>
      </Modal>
    </div>
</template>
<script lang="ts">
    import { Component, Vue, Prop, Emit } from 'vue-property-decorator';
    import AbpBase from '@/lib/abpbase'
    import { EAssignmentStatus, EPriority } from '@/store/entities/user';
    import { Form } from '@/types/iview/form';
import moment from 'moment';
    @Component({
        name: 'AddAssignment',
        components: {}
    })
    export default class AddAssignment extends AbpBase{
      @Prop() rowData: any
      @Prop() entityId: any
      isShow: boolean = true
      reporter: string = ''
      EAssignmentStatus = EAssignmentStatus
      EPriority = EPriority
      allUser = []
      userId = null;
      ruleValidate: any = {}
      fromData: any = {
        status: null,
        name: '',
        deadline: '',
        description: '',
        priority: null,
        isActive: true,
        userIds: [],
        entityAssignmentDtos: [],
      }

      beforeMount() {   
        if (this.rowData) {
          this.fromData = this.rowData
          this.userId = this.rowData.users[0].userId
          // this.fromData.userIds = []
          // if (this.rowData.users.length > 0) {
          //   this.rowData.users.forEach(element => {
          //     this.fromData.userIds.push(element.userId)
          //   });
          // }
        }
      }
      async created() {   
        this.ruleValidate = {
          name: [
            { required: true, message: this.$t('task.name'), trigger: 'blur' },
          ],
          // description: [
          //     { required: true, message: this.$t('task.description') , trigger: 'blur' },
          // ],
          deadline: [
            { required: true, type: 'date', message: this.$t('task.deadline'), trigger: 'change' },
          ],
          status: [
            { required: true, type: 'number', message: this.$t('task.status'), trigger: 'change' },
          ],
          priority: [
            { required: true, type: 'number', message: this.$t('task.priority'), trigger: 'change' },
          ],
          assigned: [
            { required: true, type: 'number', message: this.$t('task.entityAssignmentDtos'), trigger: 'change' },
          ],
        }
        this.getpage()
        const userData = await this.$store.dispatch({
          type:'assignment/getCurrentLoginInformations',
        })
        this.reporter = userData.user.userName
        this.fromData.entityAssignmentDtos = []
        this.fromData.entityAssignmentDtos.push({
          entityId: this.entityId,
          entityType: this.entityType
        })
      }

      get entityType() {
        switch (this.$route.name) {
          case 'projectDetail':
            return 2;
          case 'dealDetail':
            return 4;
          case 'invoiceDetail':
            return 0;
          case 'detailCustomer':
            return 1;
        }
      }

      cancel() {
        this.$Message.info(this.$t('common.cancel'));
        this.showAdd()
      }
      @Emit('showAdd') showAdd() {
            return
      }

      async getpage() {
        let pagerequest = {
          skipCount: 0,
          maxResultCount: 1000000,
        }
        const data = await this.$store.dispatch({
          type: 'user/getAll',
          data: pagerequest
        })

        this.allUser = data.items;
      }

      async saveAssignment(name) {
        if (this.userId != null) {
          this.ruleValidate.assigned = true;
        }
        const form = await this.$refs[name] as Form;
        this.fromData.userIds = [this.userId]
        const param = this.fromData
          await form.validate(async (valid: boolean) => {
            if (valid) {
              let data = {
                id: this.rowData == null ? 0 : this.rowData.id,
                status: this.fromData.status,
                name: this.fromData.name,
                isActive: this.fromData.isActive,
                deadline: moment(this.fromData.deadline).format('MM/DD/YYYY').toString(),
                description: this.fromData.description,
                priority: this.fromData.priority,
                entityAssignmentDtos: this.fromData.entityAssignmentDtos,
                userIds: this.fromData.userIds,
            }
              await this.$store.dispatch({
                type: 'task/saveTask',
                data: data
              })
              this.$Message.info(this.$t('common.saved'));
              this.showAdd()
            } else {
              this.$Message.error('Fail!');
            }
          })
      }
      changeAssigned() {
        if (this.userId != null) {
          this.ruleValidate.assigned = true;
        }
      }
    }
</script>
<style lang="scss" scoped>
    .error {
        text-align: center;
        color:red;
    }
</style>
<style lang="scss">
</style>
