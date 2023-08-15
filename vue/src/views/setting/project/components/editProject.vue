<template>
  <div class="detail-project">
    <PageHeading>
      <template #header>{{$t('projectManagement.editProject')}}</template>
      <template #button>
        <Button
          name="project_delete"
          class="button btn-edit"
          @click="deleteProject"
        >{{$t('common.delete')}}</Button>
      </template>
    </PageHeading>
    
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
            <Select name="projectClientId_input" v-model="dataSubmit.clientId" filterable :placeholder="$t('common.placeholderSelect',{field: $t('projectManagement.client')})">
              <Option v-for="(item, index) of clientData" v-bind:key="index" :value="item.id">{{item.name}}</Option>
            </Select>
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
                @click="handleEditProject('dataSubmit')"
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
                @click="onBack"
              >
                {{$t('common.cancel')}}
              </Button>
            </Col>
        </Row>
    </Form>
  </div>
</template>
<script lang="ts">
import { Component, Vue, Prop, Emit, Watch } from "vue-property-decorator";
import Util from "@/lib/util";
import AbpBase from "@/lib/abpbase";
import PageHeading from "@/components/heading/heading.vue";
import {
  ListType,
  ListStatus,
  ListStatusProject,
  IAssignee,
  ICommonList,
  ISaveProject,
} from "@/store/entities/project";
import AddCustomer from "../../customer/components/addCustomer.vue";

@Component({
  name: "EditProject",
  components: {
    PageHeading,
    AddCustomer,
  },
})
export default class EditProject extends AbpBase {
  value: string = "";
  error: boolean = false;
  loading: boolean = false;
  isShow: boolean = false;
  isShowAddCustomer: boolean = false;
  projectName: string = "";
  projectCode: string = "";
  description: string = "";
  projectType: Number = null;
  client: Number = null;
  status: Number = null;
  assignee: Number[] = [];
  assigneeList: IAssignee[] = [];
  statusList: ICommonList[] = ListStatusProject;
  clientData: ICommonList[] = [];
  listType: ICommonList[] = ListType;
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
  async fetchData() {
    let response = await this.$store.dispatch({
      type: "project/getProjectById",
      projectId: this.$route.params.projectId,
    });    
    if(response.result) {
      this.dataSubmit.id = response.result.id;
      this.dataSubmit.name = response.result.projectName;
      this.dataSubmit.code = response.result.projectCode;
      this.dataSubmit.type = response.result.projectType;
      this.dataSubmit.users = response.result.assigneeId;
      this.dataSubmit.clientId = response.result.clientId;
      this.dataSubmit.status = response.result.projectStatus;
      this.dataSubmit.description = response.result.description;
    }
  }
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
      type: "project/getClient",
    });
    this.clientData = clientData.result;
    let assigneeList = await this.$store.dispatch({
      type: "project/getAssigneeList",
    });
    this.assigneeList = assigneeList.result;
    this.fetchData();
  }
  handleAddCustomer() {
    this.$Message.info(this.$t("common.saved"));
    this.isShowAddCustomer = false;
  }
  private openCancelModalAdd() {
    this.isShowAddCustomer = !this.isShowAddCustomer;
  }
  cancel() {
    this.$Message.info(this.$t("common.cancel"));
    this.$emit("showAdd", false);
  }
  async deleteProject() {
    if(this.$route.params.projectId) {
        await this.$store.dispatch({
          type: "project/deleteProject",
          id: this.$route.params.projectId
        });
      this.$Message.info(this.$t('common.deleted'));
      this.onBack() 
    }
  }
  async handleEditProject(name) {
    const form = await this.$refs[name] as any;     
    await form.validate(async(valid: boolean) => {
      if (valid) {
        await this.edit()
        this.$Message.info(this.$t('common.saved'));
        this.$emit("showAdd",false)
      } else {
        this.$Message.error('Fail!');
      }
    })
  }
  async edit() {
    await this.$store.dispatch({
        type: "project/addProject",
        data: this.dataSubmit,
      });
      await this.onBack();
  }
  private onBack() {
    this.$router.push({ name: "project" });
  }
}
</script>
<style lang="scss" scoped>
.code-row-bg {
  margin-top: 10px;
}
.detail-project {
  padding: 20px;
}
.button-back {
  margin-top: 20px;
  .btn-submit {
    margin-right: 15px;
  }
}
.error-text {
  color: red;
}
</style>
