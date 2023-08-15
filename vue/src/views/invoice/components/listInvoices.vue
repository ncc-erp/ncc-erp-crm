<template>
  <div>
    <Card>
      <div class="page-body">
        <PageHeading>
          <template #header>{{$t('invoice.invoiceManage')}}</template>
          <template #button>
            <Button name="invoice_add" class="button btn-add" @click="isShowAddInvoice=true">{{$t('invoice.addInvoice')}}</Button>
          </template>
        </PageHeading>
        <Modal
          v-model="isShowAddInvoice"
          :footer-hide=true
          width="50%">
          <div slot="header" align="center"><h1>{{$t('invoice.addInvoice')}}</h1></div>
          <AddInvoice class="addInvoicePopup" :showAdd="showAdd" @showAdd="showAdd" v-if="isShowAddInvoice"/>
          <div slot="footer" class="button-zone" align="center">
          </div>
        </Modal>
        <div class="dragable-list">
          <span name="invoice_dragableList" class="label">{{$t('invoice.dragableList')}}</span>
        <i-switch v-model="isViewDrag" @on-change="changeViewList" />
        </div>
        <div class="list-invoice" v-if="!isViewDrag">
        <Form ref="queryForm" class="form">
          <FormItem>
            <Row type="flex" justify="space-around" class="code-row-bg">
              <Col span="4">
                <Select
                  name="invoiceProjectId_filter"
                  class="max-width"
                  v-model="searchParam.projectId"
                  filterable
                  clearable
                  key="searchProject"
                  @on-change="fecthData()"
                  :placeholder="$t('common.placeholderSelect',{field: $t('invoice.projectName')})"
                >
                  <Option value="" :placeholder="$t('common.all')">{{$t('common.all')}}</Option>
                  <Option
                    class="max-width" 
                    v-for="(item,index) of projectList"
                    :key="index"
                    :value="item.id"
                  >{{item.name}}</Option>
                </Select>
              </Col>
              <Col span="4">
                <Select
                  name="invoiceStatus_filter"
                  class="max-width"
                  v-model="searchParam.status"
                  key="searchStatus"
                  @on-change="fecthData()"
                  :placeholder="$t('projectManagement.status')"
                >
                  <Option value="" :placeholder="$t('common.all')">{{$t('common.all')}}</Option>
                  <Option
                    v-for="(item,index) of statusList"
                    :key="index"
                    :value="item.id"
                  >{{item.name}}</Option>
                </Select>
              </Col>
              <Col span="4">
                <Select
                  name="invoiceType_filter"
                  class="max-width"
                  v-model="searchParam.type"
                  key="searchType"
                  @on-change="fecthData()"
                  :placeholder="$t('projectManagement.type')"
                >
                  <Option value="" :placeholder="$t('common.all')">{{$t('common.all')}}</Option>
                  <Option
                    v-for="(item,index) of typeList"
                    :key="index"
                    :value="item.id"
                  >{{item.name}}</Option>
                </Select>
              </Col>
            </Row>
            <Row type="flex" justify="space-around" class="code-row-bg space-row">
              <Col span="4">
                <DatePicker name="invoiceStartTime_filter" class="max-width"  v-model="searchParam.startTime" @on-change="fecthData()" type="date" :placeholder="$t('project.startTime')" ></DatePicker>
              </Col>
              <Col span="4">
                <DatePicker name="invoiceEndTime_filter" class="max-width"  v-model="searchParam.endTime" @on-change="fecthData()" type="date" :placeholder="$t('project.endTime')" ></DatePicker>
              </Col>
              <Col span="4">
                <Select
                  name="invoiceAssigneeId_filter"
                  class="max-width"
                  v-model="searchParam.assigne"
                  filterable
                  clearable
                  key="searchProject"
                  @on-change="fecthData()"
                  :placeholder="$t('common.assignee')"
                >
                  <Option value="" :placeholder="$t('common.all')">{{$t('common.all')}}</Option>
                  <Option
                    class="max-width" 
                    v-for="item of assigneeList"
                    :key="item.userId"
                    :value="item.userId"
                  >{{item.userName}}</Option>
                </Select>
              </Col>
            </Row>
          </FormItem>
        </Form>

          <Table name="listInvoice_tbl" :columns="columns" :data="dataInvoices">
            <template slot-scope="{row}" slot="invoiceName">
              <span>
                <a @click="viewDetail(row)">{{row.invoiceName}}</a>
              </span>
            </template>
            <template slot-scope="{row}" slot="clientName">
              <span>
                <a @click="viewDetailCustomer(row)">{{row.clientName}}</a>
              </span>
            </template>
            <template slot-scope="{ row }" slot="action">
              <Button
                name="invoice_edit"
                class="btn-edit"
                style="margin-right: 5px"
                @click="openEditInvoice(row)"
              >{{$t('common.edit')}}</Button>
            </template>
             <template slot-scope="{ row }" slot="time">
           <p>{{row.time | convertedDate}}</p>
        </template>
          <template slot-scope="{ row }" slot="status">
           <p>{{converInvoiceStatus(row.status)}}</p>
        </template>
          </Table>
          <Page show-sizer class-name="fengpage" :current="currentPage" :total="totalPage" class="margin-top-10" @on-page-size-change="pSize" @on-change="pNumber" :page-size-opts="pageSizeOpts"></Page>
        </div>
        <DragableListInvoices :dataInvoicesProp="dataInvoices" v-if="isViewDrag" />
      </div>
    </Card>
  </div>
</template>
<script lang="ts">
import { Component, Vue, Prop } from "vue-property-decorator";
import Util from "@/lib/util";
import AbpBase from "@/lib/abpbase";
import DragableListInvoices from "./dragableList.vue";
import PageHeading from "../../../components/heading/heading.vue";
import moment from 'moment';
import AddInvoice from "./addInvoice.vue";
import { EInvoiceStatus } from '../../../store/entities/status';
import { ListType, ListStatus, PageSizeOpts, ICommonList, IAssignee } from '@/store/entities/project'
@Component({
  components: { DragableListInvoices, PageHeading, AddInvoice },
   filters: {
    convertedDate(value: string) {
      return moment(value).format('DD/MM/YYYY')
    },
  }
})
export default class ListInvoices extends AbpBase {
  pageSizeOpts: Array<number> = PageSizeOpts
  private isViewDrag: any = false;
  isShowAddInvoice: boolean = false;
  private dataInvoices: any = [];
  private dataAllInvoices:any = []
  private columns: any = [];
  private EInvoiceStatus = EInvoiceStatus;
  private searchParam: any = {
    type: null,
    status: null,
    projectId: null,
    assigne: null,
    startTime: '',
    endTime: ''
  }
  typeList = ListType
  statusList = ListStatus
  private projectList: any = []
  assigneeList: IAssignee[] = []
  currentPage: number = 1
  totalPage: number = 0
  pageSize: number = 10
  showAdd(show) {
    this.isShowAddInvoice = show
    this.fecthData()
  }
  async fecthData() {
    if (this.searchParam.startTime) {
      this.searchParam.startTime = new Date(new Date(this.searchParam.startTime).setHours(7, 0, 0, 0)).toISOString()
    }
    if (this.searchParam.endTime) {
      this.searchParam.endTime = new Date(new Date(this.searchParam.endTime).setHours(30, 59, 59, 0)).toISOString()
    }
    let currentpage = parseInt(localStorage.getItem("currentPage"))
    if(currentpage) this.currentPage = currentpage;
    
    let itemSearch = localStorage.getItem("searchParam");
    if(itemSearch) this.searchParam = JSON.parse(itemSearch)
    let response = await this.$store.dispatch({
      type: "invoice/getAllInvoices",
      data: this.searchParam
    });
    this.dataInvoices = response;
    this.dataAllInvoices = response
    if (this.$route.params.isViewDrag == 'true'){
     this.isViewDrag = true;
    }
    else this.isViewDrag = false;
    this.totalPage = response.length
    localStorage.setItem("currentPage", "")
    localStorage.setItem("searchParam", "")
    currentpage?this.pNumber(this.currentPage):this.pNumber(1)
  }

  changeViewList() {
    this.$router.push({
      name: "invoiceDrag",
      params: { isViewDrag: this.isViewDrag }
    });
  }

  converInvoiceStatus(value: number) {
    switch (value) {
      case EInvoiceStatus.Pending:
        return this.$t('common.pending');
      case EInvoiceStatus.Chasing:
        return this.$t('common.chasing');
      case EInvoiceStatus.Fail:
        return this.$t('common.fail');
      case EInvoiceStatus.Paid:
        return this.$t('common.paid');
    }
  }

  created() {
    this.fecthData();
    this.allProject()
    this.allAssignee()
    this.columns = [
      {
        title: this.$t("invoice.invoiceName").toString(),
        slot: "invoiceName"
      },
      {
        title: this.$t("invoice.customerName").toString(),
        slot: "clientName"
      },
      {
        title: this.$t("common.invoiceDate").toString(),
        key: "time",
        slot: "time"
      },
      {
        title: this.$t("common.type").toString(),
        key: "typeName"
      },
      {
        title: this.$t("common.status").toString(),
        key: "status",
        slot: "status"
      },
      {
        title: this.$t("common.assignee").toString(),
        key: "assigneeName"
      },
      {
        title: this.$t("projectManagement.action").toString(),
        slot: "action"
      }
    ];
  }

  viewDetail(rowValue: any) {
    let routeData = this.$router.resolve({ name: 'invoiceDetail', params: { invoiceId: rowValue.id } }); 
    window.open(routeData.href, '_blank');
  }

  viewDetailCustomer(rowValue: any) {
    let routeData = this.$router.resolve({ name: 'detailCustomer', params: { customerId: rowValue.clientId } }); 
    window.open(routeData.href, '_blank');
  }

  async allProject() {
        let param = {"maxResultCount": 10000}
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

    private openEditInvoice(rowValue: any) {
      localStorage.setItem("currentPage", JSON.stringify(this.currentPage))
      localStorage.setItem("searchParam", JSON.stringify(this.searchParam))
      this.$router.push({ name: 'editInvoice', params: { invoiceId: rowValue.id } })
  }

  private pNumber(n?: number) {
    this.currentPage = n
    const skipCount = (this.currentPage-1)*this.pageSize
    this.dataInvoices = this.dataAllInvoices.slice(skipCount, skipCount + this.pageSize)
  }

  pSize(s) {
    this.pageSize = s
    this.pNumber(1)
  }
}
</script>
<style lang="scss" scoped>  
.dragable-list {
  margin: 20px;
  .label{
    margin-right: 10px;
  }
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
</style>
<style lang="scss">

.page-body{  
  .ivu-modal {
    top: 50px !important;
  }
}
</style>
