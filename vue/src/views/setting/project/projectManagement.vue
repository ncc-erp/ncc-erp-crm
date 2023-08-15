<template>
    <div>
        <Card>
          <div class="page-body" @click="closeOption">
            <PageHeading>
              <template #header>{{$t('projectManagement.projectManager')}}</template>
              <template #button><Button name="project_add" class="button btn-add" @click="isShowAddProject=true">{{$t('projectManagement.addProject')}}</Button></template>
            </PageHeading>
            <Modal
              v-model="isShowAddProject"
              :footer-hide=true
              width="40%">
              <div slot="header" align="center"><h1>{{$t('projectManagement.addProject')}}</h1></div>
              <AddProject :showAdd="showAdd" @showAdd="showAdd" v-if="isShowAddProject"/>
              <div slot="footer" class="button-zone" align="center">
              </div>
            </Modal>
            <Form ref="queryForm" class="form">
                <FormItem>
                    <Row type="flex" justify="space-around" class="code-row-bg">
                        <Col span="4">
                            <Input
                              name="projectClientName_filter"
                               search
                               @on-change="remoteMethod_client(searchParam.clientName)"
                                v-model="searchParam.clientName"
                                @on-search="filterSearch"
                                :placeholder="$t('projectManagement.client')"
                            />
                            <span class="option">
                              <div
                                class="element-option"
                                v-for="(item,index) of clientList"
                                :key="index"
                                :value="item"
                              >
                             <span @click="editName(item)">{{item}}</span>
                              </div>
                            </span>
                        </Col>
                        <Col span="4" class="setWidthProject">
                            <Input
                              name="projectName_filter"
                              search
                               @on-change="remoteMethod_project(searchParam.name)"
                                v-model="searchParam.name"
                                @on-search="filterSearch"
                                :placeholder="$t('projectManagement.projectName')"
                            />
                            <span class="option">
                              <div
                                class="element-option"
                                v-for="(item,index) of projectList"
                                :key="index"
                                :value="item"
                              >
                             <span @click="editProject(item)">{{item}}</span>
                              </div>
                            </span>
                        </Col>
                        <Col span="4">
                              <Select
                                name="projectStatus_filter"
                                v-model="searchParam.status"
                                key="searchStatus"
                                @on-change="filterSearch"
                                :loading="loading" 
                                :placeholder="$t('projectManagement.status')">
                                <Option value="All" :placeholder="$t('common.all')">{{$t('common.all')}}</Option>
                                <Option v-for="(item,index) of statusList" :key="index" :value="item.id" >{{item.name}}</Option>
                            </Select>
                        </Col>
                        <Col span="4">
                          <Select
                            name="projectType_filter"
                              v-model="searchParam.type"
                              key="searchType"
                              :loading="loading" 
                              @on-change="filterSearch"
                              :placeholder="$t('projectManagement.type')">
                              <Option value="All" :placeholder="$t('common.all')">{{$t('common.all')}}</Option>
                              <Option v-for="(item,index) of typeList" :key="index" :value="item.id" >{{item.name}}</Option>
                          </Select>
                        </Col>
                    </Row>
                </FormItem>
            </Form>
            <div class="margin-top-10">
              <Table name="project_tbl" :loading="loading" :columns="columns" :data="searchResult" :no-data-text="$t('common.nodatas')" border>
                <template slot-scope="{row}" slot="projectName">
                  <span>
                    <a @click="viewDetail(row.id)">{{row.name}}</a>
                  </span>
                </template>
                <template slot-scope="{row}" slot="clientName">
                  <span>
                    <a @click="viewDetailClient(row.id)">{{row.clientName}}</a>
                  </span>
                </template>
                <template slot-scope="{row}" slot="action">
                  <Button name="project_edit" class="btn-edit" @click="openEditProject(row.id)">{{$t('common.edit')}}</Button>
                </template>
              </Table>
              <Page show-sizer class-name="fengpage" :current="currentPage" :total="totalPage" class="margin-top-10" @on-page-size-change="pSize" @on-change="pNumber" :page-size-opts="pageSizeOpts"></Page>
            </div>
          </div>
        </Card>
    </div>
</template>
<script lang="ts">
import { Component, Vue, Inject, Prop, Watch } from "vue-property-decorator";
import Util from "../../../lib/util";
import AbpBase from "../../../lib/abpbase";
import PageRequest from "../../../store/entities/page-request";
import {EProject, IFilterItems} from "../../../store/entities/project";
import {ICommonList, ListType, PageSizeOpts, ListStatus, ListStatusProject, IAssignee, ISaveProject, IDataList} from '@/store/entities/project'
import PageHeading from "../../../components/heading/heading.vue";
import AddProject from "./components/addProject.vue";
import EditProject from "./components/editProject.vue";
const propertyName = {
      name: "name",
      type: "type",
      clientId: "clientId",
      clientName: "clientName",
      status: "status"
    }
@Component({
  name: 'ProjectManagement',
  components: {
    PageHeading,
    AddProject,
    EditProject
  }
})
export default class ProjectManagement extends AbpBase {
  pageSizeOpts: Array<number> = PageSizeOpts
  isShowAddProject: boolean = false;
  loading: boolean = false;
  pageChange: boolean = false;
  columns: any = []
  dataAll: IDataList[] = []
  currentPage: number = 1
  pageSize: number = 10
  searchParam: any = {
    clientName: '',
    type: null,
    status: null,
    name: ''
  }
  param: any = {}
  totalPage: number = 0
  searchResult: IDataList[] = []
  filter: IDataList[] = []
  filterItems: IFilterItems[] = []
  clientList: string[] = []
  projectList: string[] = []
  statusList = ListStatusProject
  typeList = ListType
  showAdd(show) {
    this.isShowAddProject = show
    this.getAllPaging()
  }
  pSize(s) {
    this.pageSize = s
    this.getAllPaging()
  }
  pNumber(n) {
    this.pageChange = true;
    this.currentPage = n
    this.getAllPaging()
  }
  remoteMethod_client(query) {
    if (query!=='') {
      this.filter = this.dataAll.filter(item => item.clientName.toLowerCase().indexOf(query.toLowerCase()) > -1);
      let filter = [...new Set(this.filter.map(item => item.clientName))]; //distinct
      this.clientList = filter
    }
  }
  remoteMethod_project(query) {
    if (query!=='') {
      this.filter = this.dataAll.filter(item => item.name.toLowerCase().indexOf(query.toLowerCase()) > -1);
      let filter = [...new Set(this.filter.map(item => item.name))]; //distinct
      this.projectList = filter
    }
  }
  
  filterSearch() {
    this.closeOption()
    this.filterItems = []
    for (let key in this.searchParam) {
      if (key !== "type" && key !== "status") {
        if (this.searchParam[key] && this.searchParam[key] !== 'All') {
          this.filterItems.push({
            propertyName: key,
            value: this.searchParam[key].toLowerCase(),
            comparison: 6
          })
        }
      } else {
        if (this.searchParam[key] !== null && this.searchParam[key] !== 'All') {
          this.filterItems.push({
            propertyName: key,
            value: Number(this.searchParam[key])
          })
        }
      }
    }
    this.currentPage = 1
    this.getAllPaging()
  }
  
  convertProjectType(value: number) {
    let type = ''
    this.typeList.forEach(el => {
      if (el.id === value) {
        type = el.name
      }
    })
    return type
  }

  convertProjectStatus(value: number) {
    let status = ''
    this.statusList.forEach(el => {
      if (el.id === value) {
        status = el.name
      }
    })
    return status
  }
  private async getAllPaging() {
    let countNo = 1
    let paramsData: any = {}
    paramsData.maxResultCount = this.pageSize
    let currentpage = parseInt(localStorage.getItem("currentPage"))
    if(currentpage) this.currentPage = currentpage;
    paramsData.skipCount = (this.pageSize * (this.currentPage - 1)) 
    let itemSearch = localStorage.getItem("filterSearch");
    if(itemSearch) this.filterItems = JSON.parse(itemSearch)
    this.filterItems.forEach((item:any) => {
      if(item.propertyName == "name") this.searchParam.name = item.value
      if (item.propertyName == "client") this.searchParam.clientName = item.value
      if (item.propertyName == "type") this.searchParam.type = item.value
      if(item.propertyName == "status") this.searchParam.status = item.value 
    })
    if (this.filterItems) {
      paramsData.filterItems = this.filterItems
    }
    const response = await this.$store.dispatch({
      type: "project/getProject",
      data: paramsData
    })
    this.searchResult = response.result.items.map((element: any) => {
      element.no = (countNo + (this.pageSize * (this.currentPage - 1)))
      countNo++
      element.status = this.convertProjectStatus(element.status)
      element.type = this.convertProjectType(element.type)
      return element
    })
    this.totalPage = response.result.totalCount
    localStorage.setItem("currentPage", "")
    localStorage.setItem("filterSearch", "")
  }
  async getNoPagin() {
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
      ListStatusProject.forEach(itemStatus => {
        if(itemData.status == itemStatus.id) {
          itemData.statusName = itemStatus.name
        }
      })  
    })// add field typename folow type
    this.dataAll = data.result.items
  }
  async created(){
    // await this.getNoPagin()
    await this.getAllPaging()
    this.getNoPagin()
    this.columns = [
    {
      title: this.$t('projectManagement.no'),
      key: "no",
      width: 100,
      align: 'center',
    },
    {
      title: this.$t('projectManagement.projectName'),
      // key: "name",
      resizable: true,
      align: 'center',
      slot: "projectName",
    },
    {
      title: this.$t('projectManagement.client'),
      // key: "clientName",
      resizable: true,
      align: 'center',
      slot: 'clientName'
    },
    {
      title: this.$t('projectManagement.type'),
      key: "type",
      resizable: true,
      align: 'center',
    },
    {
      title: this.$t('projectManagement.status'),
      key: "status",
      resizable: true,
      align: 'center',
    },
    {
      title: this.$t('projectManagement.action'),
      // key: "action",
      slot: "action",
      resizable: true,
      align: 'center',
    },
  ]
  }
  viewDetail(rowValue: any) {
    let routeData = this.$router.resolve({ name: 'projectDetail', params: { projectId: rowValue } }); 
    window.open(routeData.href, '_blank');
  }
  viewDetailClient(rowValue: any) {
    let routeData = this.$router.resolve({ name: 'detailCustomer', params: { customerId: rowValue } }); 
    window.open(routeData.href, '_blank');
  }
  private editName(name: any) {
    this.searchParam.clientName = name
    this.clientList = []
    this.filterSearch()
  }

  private editProject(name: any) {
    this.searchParam.name = name
    this.projectList = []
    this.filterSearch()
  }

  private closeOption() {
    this.clientList = []
    this.projectList = []
  }

  private openEditProject(id: any) {
    localStorage.setItem("currentPage", JSON.stringify(this.currentPage))
    localStorage.setItem("filterSearch", JSON.stringify(this.filterItems))
    this.$router.push({ name: 'editProject', params: { projectId: id } })
  }
}
</script>
<style lang="scss">
    .setWidthProject {
      .ivu-select-dropdown{
        max-width: 100% !important;
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
</style>
<style scoped >
    .form{
        border: 1px solid gainsboro;
        padding: 10px;
    }
</style>