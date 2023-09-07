<template>
  <div>
    <Card>
      <div class="page-body" @click="closeOption">
        <PageHeading>
          <template #header>{{ $t('task.taskManage') }}</template>
          <template #button>
            <Button name="task_add" class="button btn-add" @click="isShowAddTask = true">{{ $t('task.addTask') }}</Button>
          </template>
        </PageHeading>
        <Modal 
          v-model="isShowAddTask" 
          :footer-hide=true 
          width="50%">
          <div slot="header" align="center">
            <h1>{{ $t('task.addTask') }}</h1>
          </div>
          <AddTask :showAdd="showAdd" @showAdd="showAdd" v-if="isShowAddTask" />
          <div slot="footer" class="button-zone" align="center">
          </div>
        </Modal>
        <Form ref="queryForm" class="form" v-model="formData">
          <FormItem>
            <Row type="flex" justify="space-between" class="code-row-bg">
              <Col span="7">
              <Input 
                name="taskName_filter" 
                search 
                @on-change="remoteMethod_name(formData.param.searchText)"
                v-model="formData.param.searchText" 
                @on-search="filterSearch()"
                :placeholder="$t('common.placeholderSelect', { field: $t('task.assignmentName') })" 
              />
              <span class="option">
                <div 
                  class="element-option" 
                  v-for="(item, index) of assignmentList" 
                  :key="index" 
                  :value="item"
                >
                  <span @click="editName(item)">{{ item }}</span>
                </div>
              </span>
              </Col>
              <Col span="7">
                <DatePicker name="taskStartDate_filter" style="width: 100%" v-model="formData.startDate"
                  @on-change="filterSearch()" type="date" :placeholder="$t('project.startTime')"></DatePicker>
              </Col>
              <Col span="7">
              <Input 
                name="taskUser_filter" 
                search 
                @on-change="remoteMethod_user(formData.searchUser)"
                v-model="formData.searchUser" 
                @on-search="fecthData" 
                :placeholder="$t('task.user')" 
              />
              <span class="option">
                <div 
                  class="element-option" 
                  v-for="(item, index) of userList" 
                  :key="index" 
                  :value="item"
                >
                  <span @click="selectUser(item)">{{ item }}</span>
                </div>
              </span>
              </Col>
            </Row>
            <Row type="flex" justify="space-between" class="code-row-bg space-row">
              <Col span="7">
              <Select 
                name="taskPriority_filter" 
                class="max-width" 
                v-model="formData.priority" 
                filterable 
                clearable
                @on-change="fecthData()" 
                :placeholder="$t('common.placeholderSelect', { field: $t('task.priority') })"
              >
                <Option value="" :placeholder="$t('common.all')">{{ $t('common.all') }}</Option>
                <Option 
                  class="max-width" 
                  v-for="(item, index) of priorityList" 
                  :key="index" 
                  :value="item.id"
                >{{ item.name }}</Option>
              </Select>
              </Col>
              <Col span="7">
                <DatePicker name="taskEndDate_filter" style="width: 100%" v-model="formData.endDate"
                  @on-change="filterSearch()" type="date" :placeholder="$t('project.endTime')"></DatePicker>
              </Col>

              <Col span="7">
              <!-- <Input search   @on-change="remoteMethod_entity(formData.searchEntity)"
                    v-model="formData.searchEntity"
                    @on-search="fecthData"
                    :placeholder="$t('task.entityName')"/>
                <span class="option">
                  <div  class="element-option" v-for="(item,index) of entityList" :key="index" :value="item">
                  <span @click="selectEntity(item)">{{item}}</span>
                  </div>
                </span> -->
              <Select 
                name="taskEntityName_filter" 
                style="width:100%" 
                v-model="formData.searchEntity" 
                filterable
                clearable 
                @on-change="fecthData()"
                :placeholder="$t('common.placeholderSelect', { field: $t('task.entityName') })"
              >
                <Option 
                  style="width:100%" 
                  v-for="(item, index) of entityList" 
                  :key="index" 
                  :value="item">{{ item }}</Option>
              </Select>
              </Col>
            </Row>
            <Row type="flex" justify="start" class="code-row-bg space-row">

              <Col span="7">
              <Select 
                name="taskEntityType_filter" 
                class="max-width" 
                v-model="formData.entityType" 
                filterable 
                clearable
                @on-change="fecthData()"
                :placeholder="$t('common.placeholderSelect', { field: $t('task.entityType') })"
              >
                <Option value="" :placeholder="$t('common.all')">{{ $t('common.all') }}</Option>
                <Option 
                  class="max-width" 
                  v-for="(item, index) of entityTypeList" 
                  :key="index" 
                  :value="item.id"
                >{{ item.name }}</Option>
              </Select>
              </Col>
            </Row>
          </FormItem>
        </Form>
        <div class="dragable-list">
          <!-- <span class="label">{{$t('task.dragableList')}}</span>
          <i-switch v-model="isViewDrag" @on-change="changeViewList" /> -->
        </div>
        <div class="list-task" v-if="!isViewDrag">

          <Table name="task_table" :columns="columns" :data="dataTasks">
            <template slot-scope="{row}" slot="name">
              <span>
                <a @click="viewDetail(row.id)">{{ row.name }}</a>
              </span>
            </template>
            <template slot-scope="{ row }" slot="time">
              <p>{{ convertedDate(row.deadline) }}</p>
            </template>
            <template slot-scope="{ row }" slot="action">
              <Button 
                name="task_edit" 
                class="btn-edit" 
                style="margin-right: 5px" 
                @click="openEditTask(row.id)"
              >{{ $t('common.edit') }}</Button>
            </template>
          </Table>
          <Page show-sizer class-name="fengpage" :current="currentPage" :total="totalPage" class="margin-top-10"
            @on-page-size-change="pSize" @on-change="pNumber" :page-size-opts="pageSizeOpts"></Page>
        </div>
        <!-- <DragableListTask :dataTaskProp="dataTasks" v-if="isViewDrag" /> -->
      </div>
    </Card>
  </div>
</template>
<script lang="ts">
import { Component, Vue, Prop } from "vue-property-decorator";
import Util from "@/lib/util";
import AbpBase from "@/lib/abpbase";
import PageHeading from "../../../components/heading/heading.vue";
import moment from 'moment';
import AddTask from "./addTask.vue";
import { EPriorityList, EEntityTypeList } from '../../../store/entities/task';

import { ListType, PageSizeOpts, ICommonList, IAssignee } from '@/store/entities/project'
import { EProject, IFilterItems } from "../../../store/entities/project";
const propertyName = {
  name: "name",
}
@Component({
  components: { PageHeading, AddTask },
})
export default class ListTasks extends AbpBase {
  pageSizeOpts: Array<number> = PageSizeOpts
  private isViewDrag: any = false;
  isShowAddTask: boolean = false;
  private dataTasks: any = [];
  private dataAllTasks: any = []
  private columns: any = [];
  currentPage: number = 1
  totalPage: number = 0
  pageSize: number = 10

  formData: any = {
    searchUser: "",
    searchEntity: "",
    param: {
      filterItems: [],
      searchText: "",
      maxResultCount: this.pageSize,
      skipCount: 0

    }
  }
  private dataAll: any = []
  private assignmentList: any = []
  private userOrEntityList: any = []
  private entityList: any = []
  private userList: any = []
  private priorityList: any = EPriorityList
  private entityTypeList: any = EEntityTypeList
  private filterItems: any = []

  typeList = ListType
  statusList = []

  showAdd(show) {
    this.isShowAddTask = show
    this.fecthData()
  }
  convertedDate(value: string) {
    return moment(value).format('DD/MM/YYYY')
  }
  async created() {
    await this.fecthData();
    await this.getAllData();
    this.columns = [
      {
        title: this.$t("task.name").toString(),
        key: "name",
        slot: "name"
      },
      {
        title: this.$t("task.time").toString(),
        slot: "time"
      },
      {
        title: this.$t("task.assignors").toString(),
        key: "creatorUserName"
      },
      {
        title: this.$t("task.user").toString(),
        key: "fullName",
      },
      {
        title: this.$t("task.setting").toString(),
        slot: "action",
      },
    ];
  }
  selectEntity(search) {
    this.formData.searchEntity = search
    this.entityList = []
    this.fecthData()

  }
  selectUser(search) {
    this.formData.searchUser = search
    this.userList = []
    this.fecthData()

  }
  async fecthData() {
    this.formData.param.maxResultCount = this.pageSize
    
    let currentpage = parseInt(localStorage.getItem("currentPage"))
    if(currentpage) this.currentPage = currentpage;

    this.formData.param.skipCount = (this.pageSize * (this.currentPage - 1))

    let response = await this.$store.dispatch({
      type: "task/getAllPaging",
      data: this.formData
    });
    // let entityList = await this.$store.dispatch({
    //   type: "task/getAllEntity",
    // });
    // this.entityList = entityList
    this.dataTasks = response.result.items;
    this.totalPage = response.result.totalCount;

    localStorage.setItem("currentPage", "")
  }

  changeViewList() {
    this.$router.push({
      name: "taskDrag",
      params: { isViewDrag: this.isViewDrag }
    }).catch(() => { })
  }

  viewDetail(rowValue: any) {
    //this.$router.push({ name: 'taskDetail', params: { EntityType: rowValue.entityType, EntityId: rowValue.entityId }}).catch(()=>{}); 
    this.$router.push({ name: 'taskDetail', params: { Id: rowValue } }).catch(() => { })
  }

  private openEditTask(rowValue: any) {
    localStorage.setItem("currentPage", JSON.stringify(this.currentPage))
    localStorage.setItem("filterSearch", JSON.stringify(this.filterItems))
    this.$router.push({ name: 'editTask', params: { Id: rowValue } }).catch(() => { })
  }

  private pNumber(n?: number) {
    this.currentPage = n
    const skipCount = (this.currentPage - 1) * this.pageSize
    this.dataTasks = this.dataAllTasks.slice(skipCount, skipCount + this.pageSize)
    this.fecthData()
  }

  pSize(s) {
    this.pageSize = s
    this.pNumber(1)
  }

  async getAllData() {
    let paramsSubmit: any = {
      param: {
        filterItems: [],
        maxResultCount: this.totalPage,
        skipCount: 0
      }
    }
    let response = await this.$store.dispatch({
      type: "task/getAllPaging",
      data: paramsSubmit
    });
    this.dataAllTasks = response.result.items
    this.dataAll = response.result.items;
    let filte = [...new Set(this.dataAll.map(item => item.entityName))];
    this.entityList = filte
  }
  // getListNameAssignment() {
  //     let filte = [...new Map(this.dataAll.map(item => [item['name'], item])).values()];
  //     this.assignmentList = filte
  // }

  remoteMethod_name(query: string) {
    if (query !== '') {
      const filter = this.dataAllTasks.filter(item => item.name.toLowerCase().indexOf(query.toLowerCase()) > -1);
      let nameFilter = [...new Set(filter.map(item => item.name))];   // distinct
      this.assignmentList = nameFilter
    }
    else {
      this.assignmentList = []
    }
  }

  remoteMethod_entity(query: string) {
    if (query !== "") {
      const filter = this.dataAll.filter(item => item.entityName.toLowerCase().indexOf(query.toLowerCase()) > -1);
      let filte = [...new Set(filter.map(item => item.entityName))];
      this.entityList = filte
    }
    else {
      let filte = [...new Set(this.dataAll.map(item => item.entityName))];
      this.entityList = filte
    }
  }
  remoteMethod_user(query: string) {
    if (query !== "") {
      const filter = this.dataAll.filter(item => item.fullName.toLowerCase().indexOf(query.toLowerCase()) > -1);
      let filte = [...new Set(filter.map(item => item.fullName))];
      this.userList = filte
    }
    else this.userList = []
  }
  private editName(name: any) {
    // this.searchParam.name = name
    this.formData.param.searchText = name;
    this.assignmentList = []
    this.filterSearch()
  }
  private editOwnerName(name: any) {
    // this.searchParam.ownerName = name
    // this.ownerNameList = []
    this.filterSearch()
  }
  private editClientName(name: any) {
    // this.searchParam.clientName = name
    // this.clientNameList = []
    this.filterSearch()
  }
  spliceExistFound(query) {
    let find = this.filterItems.findIndex((item) => {
      if (item.propertyName === query) return item
    })
    if (find !== -1) this.filterItems.splice(find, 1);
  }

  filterSearch() {
    this.closeOption()
    this.filterItems = []
    // for (let key in this.searchParam) {
    //   if (key !== "status") {
    //     if (this.searchParam[key] && this.searchParam[key] !== 'All') {
    //       this.filterItems.push({
    //         propertyName: key,
    //         value: this.searchParam[key].toLowerCase(),
    //         comparison: 6
    //       })
    //     }
    //   } else {
    //     if (this.searchParam[key] !== null && this.searchParam[key] !== 'All') {
    //       this.filterItems.push({
    //         propertyName: key,
    //         value: Number(this.searchParam[key])
    //       })
    //     }
    //   }
    // }

    this.currentPage = 1
    this.fecthData()
  }
  private closeOption() {
    this.assignmentList = []
    // this.ownerNameList = []
    // this.clientNameList = []
  }
}
</script>
<style lang="scss" scoped>
.dragable-list {
  margin: 20px;

  .label {
    margin-right: 10px;
  }
}

.max-width {
  max-width: 100%;
}

a:hover {
  color: blue;
  border-bottom: 1px solid blue;
}

.space-row {
  margin-top: 15px;
}
</style>
<style lang="scss">
.page-body {
  .ivu-modal {
    top: 50px !important;
  }
}

.option {
  /* min-width: 100px;
        min-height: 0px; */
  max-height: 200px;
  overflow: auto;
  top: 35px;
  max-width: 100%;
  min-width: 100%;
  right: 0%;
  /* margin: 5px 0; */
  background-color: #fff;
  box-sizing: border-box;
  border-radius: 4px;
  box-shadow: 0 1px 6px rgba(0, 0, 0, 0.2);
  position: absolute;
  z-index: 900;

  .element-option {
    padding-left: 5px !important;
  }
}

.form {
  border: 1px solid gainsboro;
  padding: 10px;
}
</style>
