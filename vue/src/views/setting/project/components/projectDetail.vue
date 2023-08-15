<template>
  <div class="project-detail">
    <ProjectInfo :data="project" />
    <div class="contract-list">
      <div class="contract-list-header">
        <p @click="showContractList">
          {{$t('project.contract')}}
          <Icon :type="arrowIcon"></Icon>
          <span @click="handleAddContract(currentContract)">+</span>
        </p>
      </div>
      <div class="contract-list-content" v-if="isShowContractList">
        <div v-for="(contract, index) in project.projectContractDetails" :key="index">
          <ContractItem
            :data="contract"
            @onEditContract="handleEditContract"
            @onEditInvoice="handleEditInvoice"
            @onRefresh="fetchData"
          />
        </div>
      </div>

      <!-- Popup edit Contract -->

      <EditContractPopup
        v-if="isShowEditContract"
        :data="currentContract"
        @onCancel="handleCancel()"
      />

      <!-- Popup edit Invoice -->

      <EditInvoicePopup
        v-if="isShowEditInvoice"
        :data="currentInvoice"
        @onCancel="handleCancel()"
      />
    </div>
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
import { Component, Vue, Inject, Prop, Watch } from "vue-property-decorator";
import Util from "@/lib/util";
import AbpBase from "@/lib/abpbase";
import PageRequest from "@/store/entities/page-request";
import ProjectInfo from "./projectInfo.vue";
import ContractItem from "./contract-item.vue";
import EditContractPopup from "./editContractPopup.vue";
import EditInvoicePopup from "./editInvoicePopup.vue";
import ScheduleItem from "../../../../components/schedule/schedule-item.vue";
import AddAssignment from "../../../../components/schedule/add-assignment.vue";

@Component({
  components: {
    ProjectInfo,
    ContractItem,
    EditContractPopup,
    EditInvoicePopup,
    ScheduleItem,
    AddAssignment
  },
})

export default class ProjectDetail extends AbpBase {
  private isShowContractList: boolean = false;
  private arrowIcon: string = 'ios-arrow-down';
  private project: any = {};
  private isShowEditContract: boolean = false;
  private isShowEditInvoice: boolean = false;
  private currentInvoice: any = null;
  private currentContract: any = null;
  private submit_loading: boolean = false;
  private EditContractPopup: any = null;
  private scheduleIcon: string = 'ios-arrow-down';
  private isShowScheduleList: boolean = false;
  private isAddAssignment: boolean = false
  private entityId: number = null

  async fetchData() {
    this.entityId = Number(this.$route.params.projectId)
    this.currentContract = {
      projectId: this.$route.params.projectId,
      contractStatus: 0
    }
    let response = await this.$store.dispatch({
      type: 'project/getProjectById',
      projectId: this.$route.params.projectId
    })
    this.project = response.result
  }

  async created() {
    this.currentContract = {
      projectId: this.$route.params.projectId,
      contractStatus: 0
    }
    this.fetchData()
  }

  mounted() {}
  
  handleAddContract(contract: any) {
    this.currentContract = contract
    this.isShowEditContract = true
    this.isShowContractList = !this.isShowContractList
  }

  handleEditContract(contract: any) {
    this.currentContract = contract
    this.isShowEditContract = true
  }

  handleEditInvoice(invoice: any) {
    this.currentInvoice = invoice
    this.isShowEditInvoice = true
  }

  showContractList() {
    this.isShowContractList = !this.isShowContractList
    if (this.isShowContractList) {
      this.arrowIcon = 'ios-arrow-up'
      return
    }
    this.arrowIcon = 'ios-arrow-down'
  }

  async handleCancel() {
    await this.fetchData()
    this.isShowEditContract = false;
    this.isShowEditInvoice = false;
    this.currentContract = {
      projectId: this.$route.params.projectId,
      contractStatus: 0
    }
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
    this.fetchData()
    this.isAddAssignment = !this.isAddAssignment
    this.showScheduleList()
  }

}
</script>
<style lang="scss" scoped>
.project-detail {
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
    .contract-list-content {
    }
  }
}
</style>