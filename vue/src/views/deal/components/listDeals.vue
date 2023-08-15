<template>
  <div>
    <Card>
      <div class="page-body" @click="closeOption">
        <PageHeading>
          <template #header>{{$t('deal.dealManage')}}</template>
          <template #button>
            <Button name="deal_add" class="button btn-add" @click="createDeal()" >{{$t('deal.addDeal')}}</Button>
          </template>
        </PageHeading>
        <Modal
          v-model="isShowAddDeal"
          :footer-hide=true
          width="50%">
          <div slot="header" align="center"><h1>{{$t('deal.addDeal')}}</h1></div>
          <AddDeal :showAdd="showAdd" @showAdd="showAdd" v-if="isShowAddDeal"/>
          <div slot="footer" class="button-zone" align="center">
          </div>
        </Modal>
        <Form ref="queryForm" class="form">
            <FormItem>
                <Row type="flex" justify="space-between" class="code-row-bg" >
                    <Col span="7">
                        <Input
                          name="dealName_filter"
                          search
                            @on-change="remoteMethod_name(searchParam.name)"
                            v-model="searchParam.name"
                            @on-search="filterSearch"
                            :placeholder="$t('deal.name')"
                        />
                        <span class="option">
                          <div
                            class="element-option"
                            v-for="(item,index) of nameList"
                            :key="index"
                            :value="item"
                          >
                          <span @click="editName(item)">{{item}}</span>
                          </div>
                        </span>
                    </Col>
                    <Col span="7" class="setWidthProject">
                        <Input
                          name="dealOwnerName_filter"
                          search
                            @on-change="remoteMethod_ownerName(searchParam.ownerName)"
                            v-model="searchParam.ownerName"
                            @on-search="filterSearch"
                            :placeholder="$t('deal.owner')"
                        />
                        <span class="option">
                          <div
                            class="element-option"
                            v-for="(item,index) of ownerNameList"
                            :key="index"
                            :value="item"
                          >
                          <span @click="editOwnerName(item)">{{item}}</span>
                          </div>
                        </span>
                    </Col>
                     <Col span="7">
                      <DatePicker 
                        name="dealStartDate_filter"
                        style="width: 100%;" 
                        v-model="startDate" 
                        @on-change="filterSearch"
                        type="date" 
                        :placeholder="$t('project.startTime')" ></DatePicker>
                     </Col>  
                   
                  </Row>
                  <Row type="flex" justify="space-between" class="code-row-bg space-row" >
                    <Col span="7">
                        <Input
                          name="dealClientName_filter"
                          search
                            @on-change="remoteMethod_clientName(searchParam.clientName)"
                            v-model="searchParam.clientName"
                            @on-search="filterSearch"
                            :placeholder="$t('projectManagement.client')"
                        />
                        <span class="option">
                          <div
                            class="element-option"
                            v-for="(item,index) of clientNameList"
                            :key="index"
                            :value="item"
                          >
                          <span @click="editClientName(item)">{{item}}</span>
                          </div>
                        </span>
                     </Col>
                     <Col span="7">
                          <Select
                            name="dealStatus_filter"
                            v-model="searchParam.status"
                            key="searchStatus"
                            @on-change="filterSearch"
                            :placeholder="$t('projectManagement.status')">
                            <Option value="All" :placeholder="$t('common.all')">{{$t('common.all')}}</Option>
                            <Option v-for="(item,index) of statusList" :key="index" :value="item.id" >{{item.name}}</Option>
                        </Select>
                    </Col>
                     <Col span="7">
                      <DatePicker 
                        name="dealEndDate_filter"
                        style="width: 100%;"
                        v-model="endDate" 
                        @on-change="filterSearch"
                        type="date" 
                        :placeholder="$t('project.endTime')" ></DatePicker>
                     </Col>
                   </Row>
                   <Row type="flex" justify="space-between" class="code-row-bg space-row">
                    <Col span="7">
                     <Select
                      name="dealPriority_filter"
                         style="width: 100%;" 
                         v-model="searchParam.priority"
                          key="searchPriority"
                         @on-change="filterSearch"
                         :placeholder="$t('task.priority')"
                      >
                     <Option value="All" :placeholder="$t('common.all')">{{$t('common.all')}}</Option>
                     <Option
                        class="max-width" 
                        v-for="(item,index) of priorityList"
                        :key="index"
                        :value="item.id"
                      >{{item.name}}</Option>
                      </Select>
                     </Col>
                    </Row>
            </FormItem>
        </Form>
        <!-- <div class="dragable-list">
          <span class="label">{{$t('invoice.dragableList')}}</span>
          <i-switch v-model="isViewDrag" @on-change="changeViewList" />
        </div> -->
        <div v-if="!isViewDrag">
          <div class="list-deal">
          <Table name="listDeal_tbl"
              border 
              :columns="columns" 
              :data="dataDeals" 
              @on-sort-change='changeSort'
              style="overflow-y:visible; overflow-x:visible;">
            <!----Tên thỏa thuận--->
            <template slot-scope="{row}" slot="name">
              <span>
                <a @click="viewDetail(row)">{{row.name}}</a>
              </span>
            </template>
            <!----Mô tả-------------->
            <template slot-scope="{row}" slot="description">
              <span>
                <p class="description" v-if="row.isEdit" v-html="xuongDong(row.description)" v-bind:class="{'expand-description': !row.isExpand}" @click="row.isExpand = !row.isExpand"></p>
                <textarea v-else v-model="row.description" rows="4"></textarea>
              </span>
            </template>
            <!----Người phụ trách---->
            <template slot-scope="{row}" slot="ownerName">
              <span>
                <p v-if="row.isEdit">{{row.ownerName}}</p>
                <Select v-else v-model="row.ownerId">
                      <Option v-for="item in assigneeList" :value="item.userId" :key="item.userId">{{ item.userName }}</Option>
                </Select>
              </span>
            </template>
            <!----Trạng thái--------->
            <template slot-scope="{ row }" slot="status">
              <div v-if="row.isEdit">
                <p :class="tagStatus(row)" >{{converDealStatus(row.status)}}</p>
              </div>
              <Select v-else
                 v-model="row.status">
                 <Option value="All" :placeholder="$t('common.all')">{{$t('common.all')}}</Option>
                 <Option v-for="(item,index) of statusList" :key="index" :value="item.id" >{{item.name}}</Option>
              </Select>
            </template>
            <!---Mức độ ưu tiên---------->
            <template slot-scope="{ row }" slot="priority">
              <div v-if="row.isEdit">
                <p :class="tagPriority(row)" >{{converDealPriority(row.priority)}}</p>
              </div>
              <Select v-else
                v-model="row.priority">
                <Option value="All" :placeholder="$t('common.all')">{{$t('common.all')}}</Option>
                <Option v-for="(item,index) of priorityList" :key="index" :value="item.id" >{{item.name}}</Option>
              </Select>
            </template>
            <!---Ngày deal đến---------->
            <template slot-scope="{ row }" slot="dealStartDate">
              <p >{{convertedDate(row.dealStartDate)}}</p>
              
            </template>
            <!---Ngày theo dõi gần nhất---------->
            <template slot-scope="{ row }" slot="dealLastFollow">
              <p >{{convertedDate(row.dealLastFollow)}}</p>
            </template>
            <!---Dự kiến nhân sự---------->
            <template slot-scope="{ row }" slot="skill">
              <p v-for="(item,index) in row.dealDetails" :key="index">{{item.skillName}} - {{item.levelName}} - {{item.quantity}}</p>
            </template>
            <!---Action--------->
            <template slot-scope="{ row }" slot="action">
              <div v-if="row.isEdit">
                <Icon @click="openEditDeal(row)" type="md-create" size="18" />
                <Icon  @click="openEditLastFollow(row)" type="md-calendar" size="18" />
              </div>
              <div v-else>
                <Icon @click="save(row)" type="md-checkmark" size="20" />
                <Icon @click="closeEditDeal(row)" type="md-close" size="20"/>
              </div>  
              <Modal
                  v-model="row.isShow"
                  title="Sửa ngày follow gần nhất"
                  @on-ok="saveDealLastFollow(row)"
                  @on-cancel="cancelModelDealLastFollow(row)">
                  <Row class="row-middle">
                    <Col :md="8">
                      <label>{{$t('deal.dealLastFollow')}}: </label>
                    </Col>
                    <Col :md="16">
                      <DatePicker class="mdDatePicker" v-model="row.dealLastFollow" :clearable="false"></DatePicker>
                    </Col>
                  </Row>
              </Modal> 
            </template>
          </Table></div>
          <Page show-sizer class-name="fengpage" :current="currentPage" :total="totalPage" class="margin-top-10" @on-page-size-change="pSize" @on-change="pNumber" :page-size-opts="pageSizeOpts"></Page>
        </div>
        <!-- <DragableListDeal :dataDealProp="dataDeals" v-if="isViewDrag" /> -->
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
import AddDeal from "./addDeal.vue";
import DragableListDeal from "./dragableList.vue";
import { EInvoiceStatus } from '../../../store/entities/status';
import { ListType, ListStatusDeal, PageSizeOpts, ICommonList, IAssignee } from '@/store/entities/project'
import {EProject, IFilterItems} from "../../../store/entities/project";
import { EPriorityList } from "@/store/entities/task";
const propertyName = {
      name: "name",
    }
@Component({
  components: {  PageHeading, AddDeal, DragableListDeal },
   filters: {
    convertedDate(value: string) {
      return moment(value).format('DD/MM/YYYY')
    },
  }
})
export default class ListDeals extends AbpBase {
  pageSizeOpts: Array<number> = PageSizeOpts
  private isViewDrag: any = false;
  isShowAddDeal: boolean = false;
  priorityList = EPriorityList
  private dataDeals: any = [];
  private dataAllDeals:any = []
  private columns: any = [];
  private EInvoiceStatus = EInvoiceStatus;
  private searchParam: any = {
    name: '',
    ownerName: '',
    status: null,
    clientName: '',
    priority: null,
  }
  private dataAll: any = []
  private nameList: any = []
  private filterItems: any = []
  private ownerNameList: any = []
  private clientNameList: any = []
  private isEdit = true;
  private dealLastFollowDate: any;
  modelEditDeal: any[] = [];
  assigneeList: IAssignee[] = []
  startDate: Date = null;
  endDate: Date = null;
  typeList = ListType
  statusList = [];
  currentPage: number = 1
  totalPage: number = 0
  pageSize: number = 10
  arrTagStatus = ['requestcome','wbs','contractsent','projectwin','projectfail','deallost','re-contract','dealwon-fix','dealwon-odc','followup']
  arrTagPriority = ['block','trival','minor','major','critical']
  dealStartDateSort: string = ''
  dealLastFollowSort: string = ''
  showAdd(show) {
    this.isShowAddDeal = show
    this.fecthData()
  }

  xuongDong(value: string) {
    if (value) {
      return value.split('\n').join('<br>');
    }
  }

  converDealStatus(value: number) {
    let type = ''
    this.statusList.forEach(el => {
      if (el.id === value) {
        type = el.name
      }
    })
    return type
  }
  converDealPriority(value: number) {
    let type = ''
    this.priorityList.forEach(el => {
      if (el.id === value) {
        type = el.name
      }
    })
    return type
  }
  async fecthData() {
    let param:any = {
      filterItems: [],
      maxResultCount: this.pageSize,
    }
    let currentpage = parseInt(localStorage.getItem("currentPage"))
    if(currentpage) this.currentPage = currentpage;
    param.skipCount = (this.pageSize * (this.currentPage - 1))
    let itemSearch = localStorage.getItem("filterSearch");
    if(itemSearch) this.filterItems = JSON.parse(itemSearch)
    
    this.filterItems.forEach((item:any) => {
      if(item.propertyName == "name") this.searchParam.name = item.value
      if (item.propertyName == "ownerName") this.searchParam.ownerName = item.value
      if (item.propertyName == "status") this.searchParam.status = item.value
      if(item.propertyName == "clientName") this.searchParam.clientName = item.value 
      if(item.propertyName == "priority") this.searchParam.priority = item.value 
    })
    if(this.filterItems) {
      param.filterItems = this.filterItems
    }
    let response = await this.$store.dispatch({
      type: "deal/getAllPaging",
      data: { startDate: this.convertedDate(this.startDate),
              endDate: this.convertedDate(this.endDate),
              dealStartDateSort: this.dealStartDateSort,
              dealLastFollowSort: this.dealLastFollowSort,
              param } 
    });
    this.dataDeals = response.result.items.map((el)=> {
      el.isEdit = true;
      el.isShow = false;
      el.isExpand = false;
      return el;
    });
    this.modelEditDeal = this.dataDeals
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
    this.dataAllDeals = response.result.items
    this.totalPage = response.result.totalCount
    localStorage.setItem("currentPage", "")
    localStorage.setItem("filterSearch", "")
  }
  changeViewList() {
    this.$router.push({
      name: "dealDrag",
      params: { isViewDrag: this.isViewDrag }
    }).catch(()=>{})
  }
  created() {
    this.fecthData();
    //this.getAllData();
    this.columns = [
      {
        title: this.$t("deal.name").toString(),
        key: "name",
        slot: "name",
        width: "200"
      },
      {
        title: this.$t("deal.description").toString(),
        key: "description",
        slot: "description",
      },
      {
        title: this.$t("deal.owner").toString(),
        key: "ownerName",
        slot: "ownerName",
        width: "150"
      },
      {
        title: this.$t("common.status").toString(),
        key: "status",
        slot: "status",
        width: "150"
      },
      {
        title: this.$t("task.priority").toString(),
        key: "priority",
        slot: "priority",
        width: "120"
      },
      {
        title: this.$t("deal.dealStartDate").toString(),
        key: "dealStartDate",
        slot: "dealStartDate",
        sortable: 'custom',
        width: "120"
      },
      {
        title: this.$t("deal.dealLastFollow").toString(),
        key: "dealLastFollow",
        slot: "dealLastFollow",
        sortable: 'custom',
        width: "120"
      },
      {
        title: this.$t("deal.employeeFuture").toString(),
        key: "skill",
        slot: "skill",
        width: "170"
      },
      {
        title: this.$t("projectManagement.action").toString(),
        slot: "action",
        align: 'center',
        width: "100"
      }
    ];
  }
  viewDetail(rowValue: any) {
    this.$router.push({ name: 'dealDetail', params: { dealId: rowValue.id } }).catch(()=>{})
  }

  createDeal() {
    this.$router.push({ name: 'AddDeal'}).catch(()=>{})
  }
  openEditDeal(rowValue: any) {
    rowValue.isEdit = false;
  }
  closeEditDeal(rowValue: any){
    let index = this.modelEditDeal.findIndex(x => x.id == rowValue.id);
    let oldData = this.modelEditDeal[index];
    rowValue.description = oldData.description
    rowValue.ownerName = oldData.ownerName
    rowValue.status = oldData.status
    rowValue.priority = oldData.priority
    rowValue.isEdit = true;
  }
  openEditDeal1(rowValue: any) {
    localStorage.setItem("currentPage", JSON.stringify(this.currentPage))
    localStorage.setItem("filterSearch", JSON.stringify(this.filterItems))
    this.$router.push({ name: 'editDeal', params: { dealId: rowValue.id } }).catch(()=>{})
  }

  openEditLastFollow(rowValue: any){
    this.dealLastFollowDate = rowValue.dealLastFollow;
    if(!rowValue.dealLastFollow){
      let dateTimeNow = new Date()
      rowValue.dealLastFollow = this.convertedDate(dateTimeNow.toString())
    }
    rowValue.isShow = true;
  }
  cancelModelDealLastFollow(rowValue: any){
    rowValue.dealLastFollow = this.dealLastFollowDate
  }
  save(rowValue) {
    const dataSubmit = {
      name: rowValue.name,
      ownerId: rowValue.ownerId,
      clientId: rowValue.clientId,
      amount: rowValue.amount,
      status: rowValue.status,
      priority: rowValue.priority,
      description: rowValue.description,
      id: rowValue.id
    }
    this.edit(dataSubmit)
    rowValue.isEdit = true;
  }
  async edit(dataSubmit) {
        await this.$store.dispatch({
            type: 'deal/quickSaveDeal',
            data: dataSubmit
        }) 
        this.$Message.info(this.$t('common.saved'));
        //this.fecthData();
    }
  private pNumber(n?: number) {
    this.currentPage = n
    const skipCount = (this.currentPage-1)*this.pageSize
    this.dataDeals = this.dataAllDeals.slice(skipCount, skipCount + this.pageSize)
    this.fecthData()
  }
  pSize(s) {
    this.pageSize = s
    this.pNumber(1)
  }
  async getAllData() {
    let param:any = {
      filterItems: [],
      maxResultCount: 1000000,
      skipCount: 0
    }
    let response = await this.$store.dispatch({
      type: "deal/getAllPaging",
      data: { startDate: this.convertedDate(this.startDate),
              endDate: this.convertedDate(this.endDate),
              param } 
    });
    this.dataAll = response.result.items
  }
  remoteMethod_name(query) {
    if (query!=='') {
      const filter = this.dataAll.filter(item => item.name.toLowerCase().indexOf(query.toLowerCase()) > -1);
      let filte = [...new Set(filter.map(item => item.name))]; //distinct
      this.nameList = filte
    }
  }
  remoteMethod_ownerName(query) {
    if (query!=='') {
      const filter = this.dataAll.filter(item => item.ownerName.toLowerCase().indexOf(query.toLowerCase()) > -1);
      let filte = [...new Set(filter.map(item => item.ownerName))]; //distinct
      this.ownerNameList = filte
    }
  }
  remoteMethod_clientName(query) {
    if (query!=='') {
      const filter = this.dataAll.filter(item => item.clientName.toLowerCase().indexOf(query.toLowerCase()) > -1);
      let filte = [...new Set(filter.map(item => item.clientName))]; //distinct
      this.clientNameList = filte
    }
  }
  private editName(name: any) {
    this.searchParam.name = name
    this.nameList = []
    this.filterSearch()
  }
  private editOwnerName(name: any) {
    this.searchParam.ownerName = name
    this.ownerNameList = []
    this.filterSearch()
  }
  private editClientName(name: any) {
    this.searchParam.clientName = name
    this.clientNameList = []
    this.filterSearch()
  }
  spliceExistFound(query) {
    let find = this.filterItems.findIndex((item) => {
      if(item.propertyName === query) return item
    })
    if(find!==-1) this.filterItems.splice(find, 1);
  }
  
  filterSearch() {
    this.closeOption()
    this.filterItems = []
    for (let key in this.searchParam) {
      if (key !== "status" && key !== "priority" ) {
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
    this.fecthData()
  }
  convertedDate(value: string | Date) {
    if(value)
      return moment(value).format('YYYY/MM/DD')
    return null
  }
  private closeOption() {
    this.nameList = []
    this.ownerNameList = []
    this.clientNameList = []
  }
  async saveDealLastFollow(data: any){
    let objUpdateDealLastFollow = {
      dealId: data.id,
      dealLastFollow: this.convertedDate(data.dealLastFollow)
    }
    let respone = await this.$store.dispatch({
      type: "deal/updateDealLastFollow",
      data: objUpdateDealLastFollow
    })
    this.fecthData()
  }
  changeSort(item){
    if(item.key === 'dealLastFollow')
    {
      this.dealLastFollowSort = item.order
    }
    else if(item.key === 'dealStartDate'){
      this.dealStartDateSort = item.order
    }
    this.fecthData()
  }
  tagStatus(row: any){
    return [this.arrTagStatus[row.status%10], 'text-tag'];
  }
  tagPriority(row: any){
    return [this.arrTagPriority[row.priority], 'text-tag']
  }
}
</script>
<style lang="scss" scoped>  
  /* tag status */
  .requestcome{
    background: #00d9ff;
  }
  .wbs{
    background: #FF5D00;
  }
  .contractsent{
    background: #FF8B00;
  }
  .projectwin{
    background: #5CFD00;
  }
  .projectfail{
    background: #c5c8ce;
  }
  .re-contract{
    background: #00fde8;
  }
  .dealwon-fix{
    background: #08fd00;
  }
  .followup{
    background: #0045FD;
  }
  .dealwon-odc{
    background: #8600FD;
  }
  .deallost{
    background: #838081;
  }
  /* end tag status */

  /* tag priority */
  .block{
    background: red;
  }
  .critical{
    background: orangered;
  }
  .minor{
    background: green;
  }
  .major{
    background: blue;
  }
  .trival{
    background: orange;
  }
  /* end tag priority */
  .text-tag{
    text-align: center;
    color: white;
    border-radius: 10px;
    width: fit-content;
    padding: 3px;
    font-weight: 600;
  }
  .ivu-table-cell-slot .ivu-icon{
    margin: 5px;
  }
.list-deal {
  //display: flex;
}
  .dragable-list {
    margin: 20px;
    .label{
      margin-right: 10px;
    }
  }
  textarea {
    width: 100%;
    margin-top:5px; 
    margin-bottom:5px;
    border-color: #c5c8ce;
  }
  .max-width {
    max-width: 210px;
  }
  a:hover {
    color: blue;
    border-bottom: 1px solid blue;
  }
  .space-row {
    margin-top: 15px;
  }
  .description{
    overflow: hidden;
    display: -webkit-box;
    -webkit-box-orient: vertical;
    font-size: 14px;        
    line-height: 2em;       
  }
  .expand-description{
    text-overflow: ellipsis;
    -webkit-line-clamp:2;
  }
  .row-middle{
    display: flex !important;
    align-items: center;
  }
  .row-middle .ivu-date-picker{
    display: block !important;
  }
  ::v-deep col{
    min-width: 100px;
  }
</style>
<style lang="scss">
.page-body{  
    .ivu-modal {
      top: 50px !important;
    }
  }
  .option {
    max-height: 200px;
    overflow: auto;
    top: 35px;
    max-width: 100%;
    min-width: 100%;
    right: 0%;
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
