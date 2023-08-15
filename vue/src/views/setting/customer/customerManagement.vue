<template>
  <div>
    <Card>
      <div class="page-body" @click="closeOption">
        <PageHeading>
          <template #header>{{$t('customerManagement.customerManager')}}</template>
          <template #button>
            <Button
              name="customer_add"
              class="button btn-add"
              @click="openCancelModalAdd"
            >{{$t('customerManagement.addCustomer')}}</Button>
          </template>
        </PageHeading>
        <Modal
          v-model="isShow"
          :footer-hide=true
          width="65%">
          <div slot="header" align="center"><h1>{{$t('customerManagement.addCustomer')}}</h1></div>
          <addCustomer @onSubmit="openCancelModalAdd" v-if="isShow"/>
          <div slot="footer" class="button-zone" align="center">
          </div>
        </Modal>
        <!-- <addCustomer v-if="isShow" @onSubmit="openCancelModalAdd" /> -->
        <Form ref="queryForm" class="form">
          <FormItem>
            <Row type="flex" justify="space-around" class="code-row-bg">
              <Col span="4">
                <Input
                  name="customerName_filter"
                  search
                  @on-change="remoteMethod_clientName(searchParam.name)"
                  v-model="searchParam.name"
                  @on-search="filterSearch"
                  :placeholder="$t('customerManagement.clientName')"
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
              <Col span="4">
                <Input
                  name="customerMail_filter"
                  search
                  @on-change="remoteMethod_email(searchParam.mail)"
                  v-model="searchParam.mail"
                  @on-search="filterSearch"
                  :placeholder="$t('common.email')"
                />
                <span class="option">
                  <div
                    class="element-option"
                    v-for="(item,index) of emailList"
                    :key="index"
                    :value="item"
                  >
                    <span @click="editEmail(item)">{{item}}</span>
                  </div>
                </span>
              </Col>
              <Col span="4">
                <Select
                  name="customerStatus_filter"
                  v-model="searchParam.status"
                  key="searchStatus"
                  @on-change="filterSearch"
                  :loading="loading"
                  :placeholder="$t('projectManagement.status')"
                >
                  <Option value="All" :placeholder="$t('common.all')">{{$t('common.all')}}</Option>
                  <Option
                    v-for="(item,index) of statusList"
                    :key="index"
                    :value="item.id"
                  >{{item.name}}</Option>
                </Select>
              </Col>
              <Col span="4">
                <Select
                  name="customerType_filter"
                  v-model="searchParam.type"
                  key="searchType"
                  @on-change="filterSearch"
                  :loading="loading"
                  :placeholder="$t('projectManagement.type')"
                >
                  <Option value="All" :placeholder="$t('common.all')">{{$t('common.all')}}</Option>
                  <Option
                    v-for="(item,index) of typeList"
                    :key="index"
                    :value="item.id"
                  >{{item.name}}</Option>
                </Select>
              </Col>
            </Row>
          </FormItem>
        </Form>

        <!-- Table View -->

        <div v-if="!isViewDrag" class="margin-top-10">
          <Table name="customer_tbl"
            :loading="loading"
            :columns="columns"
            :data="data"
            :no-data-text="$t('common.nodatas')"
            border
          >
            <template slot-scope="{row}" slot="clientName">
              <span>
                <a @click="openDetailCustomer(row)">{{row.name}}</a>
              </span>
            </template>
            <template slot-scope="{row}" slot="setting">
              <Button name="customer_edit" class="btn-edit" @click="openEditCustomer(row.id)">{{$t('common.edit')}}</Button>
            </template>
          </Table>
        </div>
        <div v-if="!isViewDrag" class="page-select margin-top-10">
          <Page
            :page-size="pageSize"
            :current="currentPage"
            @on-change="onChangePage"
            :total="totalCount"
          />
          <Select @on-change="onChangeSize" v-model="pageSize" class="select-size">
            <Option
              v-for="item in pageSizeOpts"
              :value="item.value"
              :key="item.value"
            >{{ item.label }}</Option>
          </Select>
        </div>

        <!-- Draggable View -->

        <div v-if="isViewDrag">
          <DraggableViewList :data="allStatusList" @onRefresh="getDataAll" />
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
import PageHeading from "../../../components/heading/heading.vue";
import AddCustomer from "./components/addCustomer.vue";
import { ICommonList, ListTypeCustomer, PageSizeOpts, ListStatusCustomer,  IAssignee, ISaveProject, IDataList } from '@/store/entities/project'
import { EInvoiceStatus, EListStatusCustomer } from '../../../store/entities/status';
import DraggableViewList from '../../../components/draggable-columns/draggable-view-list.vue'

class PageRoleRequest extends PageRequest {
  keyword: string = "";
}
@Component({
  name: 'customerManagement',
  components: {
    PageHeading,
    AddCustomer,
    DraggableViewList
  }
})
export default class Roles extends AbpBase {
  private currentPage: number = 1
  private totalCount: number = 1
  private pageSize: number = 10
  private nameList: any = []
  private emailList: any = []
  private filterItems: any = []
  private dataAll: any = []
  private EInvoiceStatus = EInvoiceStatus;
  typeList = ListTypeCustomer
  statusList = ListStatusCustomer
  private searchParam: any = {
    name: '',
    mail: '',
    type: null,
    status: null
  }
  pageSizeOpts = [
    {
      value: 10,
      label: '10/page'
    },
    {
      value: 20,
      label: '20/page'
    },
    {
      value: 50,
      label: '50/page'
    },
    {
      value: 100,
      label: '100/page'
    },
  ]
  isViewDrag: boolean = false;

  get edit() {
    return this.editModalShow = true;
  }
  year: number = new Date().getFullYear();
  yearCollect: any = []
  pagerequest: PageRoleRequest = new PageRoleRequest();
  columns: any
  isShow: boolean = false;
  createModalShow: boolean = false;
  editModalShow: boolean = false;
  loading: boolean = false;
  check: boolean = false;
  pendingList: any = {
    name: 'Pending',
    list: [],
    status: 0
  };
  chasingList: any = {
    name: 'Chasing',
    list: [],
    status: 1
  };
  failList: any = {
    name: 'Fail',
    list: [],
    status: 2
  };
  paidList: any = {
    name: 'Paid',
    list: [],
    status: 3
  };
  allStatusList: any = []


  remoteMethod_clientName(query) {
    if (query !== '') {
      const filter = this.dataAll.filter(item => item.name.toLowerCase().indexOf(query.toLowerCase()) > -1);
      let filte = [...new Set(filter.map(item => item.name))]; //distinct
      this.nameList = filte
    } else this.nameList = []
  }

  remoteMethod_email(query) {
    if (query !== '') {
      const filter = this.dataAll.filter(item => item.mail.toLowerCase().indexOf(query.toLowerCase()) > -1);
      let filte = [...new Set(filter.map(item => item.mail))]; //distinct
      this.emailList = filte
    } else this.emailList = []
  }

  async getDataAll() {
    this.pendingList.list = []
    this.chasingList.list = []
    this.failList.list = []
    this.paidList.list = []
    let paramsData: any = {}
    paramsData.maxResultCount = 10000
    paramsData.skipCount = 0
    if (this.filterItems) {
      paramsData.filterItems = this.filterItems
    }
    const response = await this.$store.dispatch({
      type: "customer/getAllPaging",
      params: paramsData
    })
    this.dataAll = response.items
    this.dataAll.map(data => {
      if (data.status === EInvoiceStatus.Pending) {
        this.pendingList.list.push(data)
      }
      else if (data.status === EInvoiceStatus.Chasing) {
        this.chasingList.list.push(data)
      }
      else if (data.status === EInvoiceStatus.Fail) {
        this.failList.list.push(data)
      }
      else if (data.status === EInvoiceStatus.Paid) {
        this.paidList.list.push(data)
      }
    })
    this.allStatusList = [this.pendingList, this.chasingList, this.failList, this.paidList]
  }

  converInvoiceType(value: number) {
    let type = ''
    this.typeList.forEach(el => {
      if (el.id === value) {
        type = el.name
      }
    })
    return type
  }

  converInvoiceStatus(value: number) {
    switch (value) {
      case EListStatusCustomer.New:
        return this.$t('customerManagement.New');
      case EListStatusCustomer.RegularContact:
        return this.$t('customerManagement.RegularContact');
      case EListStatusCustomer.InactiveContact:
        return this.$t('customerManagement.InactiveContact');
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

  handleAddCustomer() {
    this.$Message.info(this.$t('common.saved'));
    this.isShow = false
  }
  branch = [
    {
      name: "Hà Nội",
      id: 1,
    },
    {
      name: "Đà Nẵng",
      id: 1,
    }
  ]
  data = []
  created() {
    this.columns = [
      {
        title: this.$t("customerManagement.no").toString(),
        key: "no",
        width: 100,
        align: 'center',
      },
      {
        title: this.$t("customerManagement.clientName").toString(),
        slot: "clientName",
        align: 'center',
      },
      {
        title: this.$t("customerManagement.email").toString(),
        key: "mail",
        align: 'center',
      },
      {
        title: this.$t("customerManagement.type").toString(),
        key: "type",
        align: 'center',
      },
      {
        title: this.$t("customerManagement.status").toString(),
        key: "status",
        align: 'center',
      },
      {
        title: this.$t("customerManagement.setting").toString(),
        slot: "setting",
        align: 'center',
      },
    ];
    for (let i = 0;i <= 50;i++) {
      this.yearCollect.push(this.year)
      this.year--
    }
    this.getAllPaging()
    this.getDataAll()
  }

  advance() {
    return this.check = !this.check;
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
      if (item.propertyName == "mail") this.searchParam.mail = item.value
      if (item.propertyName == "type") this.searchParam.type = item.value
      if(item.propertyName == "status") this.searchParam.status = item.value 
    })
    if (this.filterItems) {
      paramsData.filterItems = this.filterItems
    }
    const response = await this.$store.dispatch({
      type: "customer/getAllPaging",
      params: paramsData
    })
    this.data = response.items.map((element: any) => {
      let dataElement = element
      dataElement.no = (countNo + (this.pageSize * (this.currentPage - 1)))
      countNo++
      dataElement.status = this.converInvoiceStatus(element.status)
      dataElement.type = this.converInvoiceType(element.type)
      return dataElement
    })
    this.totalCount = response.totalCount
    localStorage.setItem("currentPage", "")
    localStorage.setItem("filterSearch", "")
  }

  private onChangePage(page) {
    this.currentPage = page
    this.getAllPaging()
  }

  private onChangeSize() {
    this.currentPage = 1
    this.getAllPaging()
  }

  private openCancelModalAdd() {
    this.isShow = !this.isShow
    this.getAllPaging()
  }

  private openDetailCustomer(data?: any) {
    let routeData = this.$router.resolve({ name: 'detailCustomer', params: { customerId: data.id } }); 
    window.open(routeData.href, '_blank');
  }

  private openEditCustomer(id: any) {
    localStorage.setItem("currentPage", JSON.stringify(this.currentPage))
    localStorage.setItem("filterSearch", JSON.stringify(this.filterItems))
    this.$router.push({ name: 'editCustomer', params: { customerId: id } }).catch(()=>{})
  }

  private editEmail(mail: any) {
    this.searchParam.mail = mail
    this.emailList = []
    this.filterSearch()
  }

  private editName(name: any) {
    this.searchParam.name = name
    this.nameList = []
    this.filterSearch()
  }

  private closeOption() {
    this.nameList = []
    this.emailList = []
  }
}
</script>

<style lang="scss" scoped>
.form {
  border: 1px solid gainsboro;
  padding: 10px;
}
.page-select {
  width: 100%;
  display: flex;
  flex-wrap: wrap;
}
.select-size {
  width: 87px;
  margin-left: 30px;
}
a:hover {
  color: blue;
  border-bottom: 1px solid blue;
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
<style lang="scss" scoped>
.customer-management {
  .switch-view {
    margin: 10px 0;
    display: flex;
    align-items: center;
    span {
      margin-right: 10px;
    }
  }
}
</style>