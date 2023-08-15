<template>
  <div class="detail-customer">
    <PageHeading>
      <template #header>{{
        $t("customerManagement.customersDetails")
      }}</template>
      <template #button>
       <Button type="default" @click="onBack">{{$t("customerManagement.back")}}</Button>
      </template>
    </PageHeading>
    <Row type="flex" class="code-row-bg" align="middle">
      <Col span="3">{{ $t("customerManagement.clientName") }}:</Col>
      <Col span="17">
        <p>{{ formData.name }}</p>
      </Col>
    </Row>
    <Row type="flex" class="code-row-bg" align="middle">
      <Col span="3">{{ $t("customerManagement.country") }}:</Col>
      <Col span="17">
        <p>{{ formData.country }}</p>
      </Col>
    </Row>
    <Row type="flex" class="code-row-bg" align="middle">
      <Col span="3">{{ $t("customerManagement.email") }}:</Col>
      <Col span="17">
        <p>{{ formData.mail }}</p>
      </Col>
    </Row>
    <Row type="flex" class="code-row-bg" align="middle">
      <Col span="3">{{ $t("customerManagement.region") }}:</Col>
      <Col span="17">
        <p>{{ formData.regionName }}</p>
      </Col>
    </Row>
    <Row type="flex" class="code-row-bg" align="middle">
      <Col span="3">{{ $t("customerManagement.website") }}:</Col>
      <Col span="17">
        <p>{{ formData.website }}</p>
      </Col>
    </Row>
    <Row type="flex" class="code-row-bg" align="middle">
      <Col span="3">{{ $t("customerManagement.phone") }}:</Col>
      <Col span="17">
        <p>{{ formData.phone }}</p>
      </Col>
    </Row>
    <Row type="flex" class="code-row-bg" align="middle">
      <Col span="3">{{ $t("customerManagement.type") }}:</Col>
      <Col span="17">
        <p>{{ converInvoiceType(formData.type) }}</p>
      </Col>
    </Row>
    <Row type="flex" class="code-row-bg" align="middle">
      <Col span="3">{{ $t("customerManagement.status") }}:</Col>
      <Col span="17">
        <p>{{ converInvoiceStatus(formData.status) }}</p>
      </Col>
    </Row>
    <Row type="flex" class="code-row-bg" align="middle">
      <Col span="3">{{ $t("customerManagement.description") }}:</Col>
      <Col span="17"
        ><p>{{ formData.description }}</p></Col
      >
    </Row>
    <Row type="flex" class="code-row-bg">
      <Col span="3">{{ $t("contact.contact") }}:</Col>
      <Col span="17">
        <Table name="customerContact_tbl"
          :columns="columns"
          :data="searchResult"
          :no-data-text="$t('common.nodatas')"
          border
        >
          <template slot-scope="{ row }" slot="name">
            <span>
              <a @click="openDetailContact(row)">{{ row.name }}</a>
            </span>
          </template>
        </Table>
      </Col>
    </Row>
    <Row type="flex" class="code-row-bg">
      <Col span="3">{{ $t("deal.deal") }}:</Col>
      <Col span="17">
        <Table name="customerDeal_tbl" :columns="columnsDeal" :data="formData.deals" border>
          <template slot-scope="{ row }" slot="name">
            <span>
              <a @click="viewDetail(row)">{{ row.name }}</a>
            </span>
          </template>
          <template slot-scope="{ row }" slot="status">
            <p>{{ converDealStatus(row.status) }}</p>
          </template>
          <template slot-scope="{ row }" slot="closingDate">
            <p>{{ row.closingDate | convertedDate }}</p>
          </template>
        </Table>
      </Col>
    </Row>
    <Row type="flex" class="code-row-bg">
      <Col span="3">{{ $t("project.project") }}:</Col>
      <Col span="17">
        <Table name="customerProject_tbl" :columns="columnsProject" :data="formData.projects" border>
          <template slot-scope="{ row }" slot="projectName">
            <span>
              <a @click="viewDetailproject(row.id)">{{ row.name }}</a>
            </span>
          </template>
        </Table>
      </Col>
    </Row>
    <div class="contract-list">
      <div class="contract-list-header">
        <p @click="showScheduleList">
          {{$t('team.scheduleList')}}
          <Icon :type="scheduleIcon"></Icon>
          <span @click="handleAddSchedule()">+</span>
        </p>
      </div>
      <div class="contract-list-content" v-if="isShowScheduleList">
          <ScheduleItem :entity-id="entityId" v-if="isShowScheduleList" @showAdd="showScheduleList()"/>
      </div>
      <AddAssignment :entity-id="entityId" @showAdd="showAdd"  v-if="isAddAssignment"/>
    </div>
  </div>
</template>
<script lang="ts">
import { Component, Vue, Prop, Emit, Watch } from "vue-property-decorator";
import Util from "@/lib/util";
import AbpBase from "@/lib/abpbase";
import PageHeading from "@/components/heading/heading.vue";
import { ListTypeCustomer, ListStatusCustomer } from "@/store/entities/project";
import { ListStatusDeal } from "@/store/entities/project";
import {
  ICommonList,
  ListType,
  PageSizeOpts,
  ListStatus,
  ListStatusProject,
  IAssignee,
  ISaveProject,
  IDataList,
} from "@/store/entities/project";
import ScheduleItem from "../../../../components/schedule/schedule-item.vue";
import AddAssignment from "../../../../components/schedule/add-assignment.vue";
@Component({
  name: "detailCustomer",
  components: {
    PageHeading,
    ScheduleItem,
    AddAssignment
  },
})
export default class detailCustomer extends AbpBase {
  private formData: any = {};
  typeList = ListTypeCustomer;
  statusList = ListStatusCustomer;
  private statusListDeal = ListStatusDeal;
  private columns: any = [];
  private columnsDeal: any = [];
  private columnsProject: any = [];
  private searchResult: any = [];
  private arrowIcon: string = 'ios-arrow-down';
  private scheduleIcon: string = 'ios-arrow-down';
  private isShowScheduleList: boolean = false;
  private isAddAssignment: boolean = false
  private entityId: number = null
  private listType: any = [
    {
      value: 0,
      label: 0,
    },
    {
      value: 1,
      label: 1,
    },
    {
      value: 2,
      label: 2,
    },
    {
      value: 3,
      label: 3,
    },
  ];

  @Watch("$route")
  getEvent() {
    this.fetchData();
  }

  private async fetchData() {
    this.entityId = Number(this.$route.params.customerId)
    this.formData = {};
    let countNo = 1;
    if (this.$route.params.customerId) {
      let response = await this.$store.dispatch({
        type: "customer/getCustomer",
        id: this.$route.params.customerId,
      });
      response.projects.forEach((itemData) => {
        itemData.no = countNo;
        countNo++;
        ListType.forEach((itemType) => {
          if (itemData.type == itemType.id) {
            itemData.typeName = itemType.name;
          }
        });
        ListStatusProject.forEach((itemStatus) => {
          if (itemData.status == itemStatus.id) {
            itemData.statusName = itemStatus.name;
          }
        });
      });
      this.formData = response;

      let customers = await this.$store.dispatch({
        type: "contact/getByClient",
        clientId: this.$route.params.customerId,
      });
      this.searchResult = customers;
    }
  }

  private beforeMount() {
    this.fetchData();
    this.columns = [
      {
        title: this.$t("contact.name"),
        resizable: true,
        align: "center",
        key: "name",
        slot: "name",
      },
      {
        title: this.$t("contact.email"),
        key: "mail",
        resizable: true,
        align: "center",
      },
      {
        title: this.$t("contact.phone"),
        key: "phone",
        resizable: true,
        align: "center",
      },
      {
        title: this.$t("contact.role"),
        key: "role",
        resizable: true,
        align: "center",
      },
      {
        title: this.$t("contact.description"),
        key: "description",
        resizable: true,
        align: "center",
      },
    ];

    this.columnsDeal = [
      {
        title: this.$t("deal.name").toString(),
        key: "name",
        slot: "name",
      },
      {
        title: this.$t("deal.owner").toString(),
        key: "ownerName",
      },
      {
        title: this.$t("common.status").toString(),
        key: "status",
        slot: "status",
      },
      {
        title: this.$t("deal.closeDate").toString(),
        key: "closingDate",
        slot: "closingDate",
      },
    ];
    this.columnsProject = [
      {
        title: this.$t("projectManagement.no"),
        key: "no",
        width: 100,
        align: "center",
      },
      {
        title: this.$t("projectManagement.projectName"),
        // key: "name",
        resizable: true,
        align: "center",
        slot: "projectName",
      },
      {
        title: this.$t("projectManagement.type"),
        key: "typeName",
        resizable: true,
        align: "center",
      },
      {
        title: this.$t("projectManagement.status"),
        key: "statusName",
        resizable: true,
        align: "center",
      },
    ];
  }
  private onBack() {
    this.$router.push({ name: "customer" });
  }

  converInvoiceType(value: number) {
    let type = "";
    this.typeList.forEach((el) => {
      if (el.id === value) {
        type = el.name;
      }
    });
    return type;
  }

  converInvoiceStatus(value: number) {
    let type = "";
    this.statusList.forEach((el) => {
      if (el.id === value) {
        type = el.name;
      }
    });
    return type;
  }

  converDealStatus(value: number) {
    let type = "";
    this.statusListDeal.forEach((el) => {
      if (el.id === value) {
        type = el.name;
      }
    });
    return type;
  }

  viewDetail(rowValue: any) {
    if (rowValue.id) {
      let routeData = this.$router.resolve({
        name: "dealDetail",
        params: { dealId: rowValue.id },
      });
      window.open(routeData.href, "_blank");
    }
    // this.$router.push({ name: 'dealDetail', params: { dealId: rowValue.id } });
  }

  viewDetailproject(rowValue: any) {
    let routeData = this.$router.resolve({
      name: "projectDetail",
      params: { projectId: rowValue },
    });
    window.open(routeData.href, "_blank");
    // this.$router.push({ name: 'projectDetail', params: { projectId: rowValue } })
  }
  private openDetailContact(rowValue: any) {
    let routeData = this.$router.resolve({
      name: "detailContact",
      params: { contactId: rowValue.id },
    });
    window.open(routeData.href, "_blank");
  }

  async showScheduleList() {
    this.isShowScheduleList = !this.isShowScheduleList
    if (this.isShowScheduleList) {
      this.scheduleIcon = 'ios-arrow-up'
      return
    }
    this.scheduleIcon = 'ios-arrow-down'
  }

  handleAddSchedule(data?: any) {
    this.isShowScheduleList = !this.isShowScheduleList
    this.isAddAssignment = !this.isAddAssignment
  }

  showAdd() {
    this.isAddAssignment = !this.isAddAssignment
    this.showScheduleList()
  }
}
</script>
<style lang="scss" scoped>
.code-row-bg {
  margin-top: 10px;
}
.detail-customer {
  padding: 20px;
}
.button-back {
  margin-top: 20px;
}
.contract-list {
    width: 100%;
    .contract-list-header {
      p {
        text-align: center;
        cursor: pointer;
        text-transform: uppercase;
        background-color: #d76767;
        color: white;
        font-weight: bold;
        padding: 10px 0;
        font-size: 15px;
        line-height: 18px;
        margin: 10px 0;
      }
      i {
        float: right;
        padding-right: 10px;
      }
      span {
        float: right;
        padding-right: 10px;
        padding: 0 10px;
        display: inline-block;
      }
    }
  }
</style>
