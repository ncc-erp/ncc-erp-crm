<template>
  <div class="dragble-list-invoice">
    <Split
      class="split-detail"
      :style="{height:heightList *200+'px'}"
      v-model="splitRatio"
      v-if="isViewDetail"
    >
      <div slot="left">
        <div class="left-list">
          <div class="pool-list-item" :style="{height:heightList *150+'px'}">
            <List
              v-for="(data, name) in dataInvoicesMapped"
              :key="name"
              :header="name | upperCase"
              class="list-item"
            >
              <template slot="header">
                <div class="header-list">{{name | upperCase}}</div>
              </template>
              <div
                :class="coverList[name]"
                :style="{height:heightList *150+'px'}"
                @dragend="(event)=>dragEnd(event, name)"
                @drop="(event)=>drop(event, name)"
                @dragover="(event)=>allowDrop(event , name)"
                @dragleave="((event)=>dragLeave(event , name))"
              >
                <div class="chilld-div">
                  <ListItem v-for="item in data" :key="item.name">
                    <div
                      @click="selectInvoice(item)"
                      class="drop-item"
                      draggable="true"
                      @dragstart="(event)=> drag(item, event, name)"
                    >
                      <p>{{$t('project.project')}} : <a @click="viewDetail(item.projectId)">{{item.projectName}}</a></p>
                      <p>{{$t('projectManagement.clientName')}}: {{ item.clientName }}</p>
                      <p>{{$t('projectManagement.type')}}: {{ item.typeName }}</p>
                      <p>{{$t('projectManagement.assignee')}}: {{ item.assigneeName }}</p>
                      <p>{{$t('invoice.time')}}: {{ item.time | moment("YYYY-MM-DD") }}</p>
                    </div>
                  </ListItem>
                </div>
              </div>
            </List>
          </div>
        </div>
      </div>
      <div slot="right" class="right-detail-invoice">
        <DetailInvoiceDrag  v-if="isViewDetail" />
      </div>
    </Split>
    <div v-else class="grid-list-invoice">
      <div class="pool-list-item">
        <List
          v-for="(data, name) in dataInvoicesMapped"
          :key="name"
          :header="name | upperCase"
          class="list-item"
        >
          <template slot="header">
            <div class="header-list">{{name | upperCase}}</div>
          </template>
          <div
            :class="coverList[name]"
            :style="{height:heightList *150+'px'}"
            @dragend="(event)=>dragEnd(event, name)"
            @drop="(event)=>drop(event, name)"
            @dragover="(event)=>allowDrop(event , name)"
            @dragleave="((event)=>dragLeave(event , name))"
          >
            <div class="chilld-div">
              <ListItem v-for="item in data" :key="item.name">
                <div
                  @click="selectInvoice(item)"
                  class="drop-item"
                  draggable="true"
                  @dragstart="(event)=> drag(item, event, name)"
                >
                  <p>{{$t('project.project')}} : <a @click="viewDetail(item.projectId)">{{item.projectName}}</a></p>
                  <p>{{$t('projectManagement.clientName')}}: {{ item.clientName }}</p>
                  <p>{{$t('projectManagement.type')}}: {{ item.typeName }}</p>
                  <p>{{$t('projectManagement.assignee')}}: {{ item.assigneeName }}</p>
                  <p>{{$t('invoice.time')}}: {{ item.time | moment("YYYY-MM-DD") }}</p>
                </div>
              </ListItem>
            </div>
          </div>
        </List>
      </div>
    </div>
  </div>
</template>
<script lang="ts">
import { Component, Vue, Prop, Watch } from "vue-property-decorator";
import Util from "@/lib/util";
import AbpBase from "@/lib/abpbase";
import DetailInvoiceDrag from "./detailInvoiceDrag.vue";
import { EInvoiceStatusList } from "../../../store/entities/status";
@Component({
  components: { DetailInvoiceDrag },
  filters: {
    upperCase(value: any) {
      return value.toUpperCase();
    },
  },
})
export default class DragableListInvoices extends AbpBase {
  @Prop() private dataInvoicesProp;
  private dataInvoices: any = [];
  private coverList: any = {
    waiting: {},
  };
  private splitRatio = 0.6;
  private typeAllowDrop: any = "";
  private isViewDetail = false;
  private dataInvoicesMapped: any = {};
  private heightList = 4;
  hadIdProject = false;
  private listStatus = [
    { id: 0, name: EInvoiceStatusList.Pending },
    { id: 1, name: EInvoiceStatusList.Chasing },
    { id: 2, name: EInvoiceStatusList.Fail },
    { id: 3, name: EInvoiceStatusList.Paid },
  ];
  mounted() {
    this.dataInvoicesMapped;
  }

  @Watch("$route")
  getInvoice() {
    this.fetchData(true);
  }

  viewDetail(rowValue: any) {
    this.hadIdProject = true
    if(rowValue){
      let routeData = this.$router.resolve({ name: 'projectDetail', params: { projectId: rowValue } }); 
      window.open(routeData.href, '_blank');
    }
  }
  created() {
    this.fetchData(false);
  }

  async fetchData(isCall) {
    if (this.$route.params.invoiceId) {
      this.isViewDetail = true;
    } else {
      this.isViewDetail = false;
    }
    if (!isCall) {
      this.dataInvoices = this.dataInvoicesProp;
    } else {
      let response = await this.$store.dispatch({
        type: "invoice/getAllInvoices",
        data: {},
      });
      this.dataInvoices = response;
    }
    this.listStatus.forEach((el) => {
      this.dataInvoicesMapped[el.name] = [];
    });
    if (this.dataInvoices) {
      this.dataInvoices.forEach((el) => {
        el.statusName = this.listStatus.find((ele) => ele.id == el.status).name;
        this.dataInvoicesMapped[el.statusName].push(el);
      });
    }
     let maxLength = 1;
      for (let key in this.dataInvoicesMapped) {
        if (this.dataInvoicesMapped[key].length > maxLength) {
          maxLength = this.dataInvoicesMapped[key].length;
          this.heightList = this.dataInvoicesMapped[key].length;
        }
      }
    this.$forceUpdate()
  }

  allowDrop(ev, name) {
    if (this.typeAllowDrop.includes(name)) {
      ev.preventDefault();
      if (this.coverList[name] && this.coverList[name]["outer-item"])
        this.coverList[name] = {
          "hover-outer-item": true,
          "outer-item": false,
          "none-div": true,
        };
    }
  }

  dragEnd(ev, name) {
    this.coverList = {};
  }

  dragLeave(ev, name) {
    if (this.coverList[name] && this.coverList[name]["hover-outer-item"])
      this.coverList[name] = {
        "hover-outer-item": false,
        "outer-item": true,
        "none-div": true,
      };
  }

  drag(data, ev, type) {
    if (data.changeFromChasing) {
      this.coverList = {
        [EInvoiceStatusList.Pending]: { "outer-item": true, "none-div": true },
        [EInvoiceStatusList.Fail]: { "outer-item": true, "none-div": true },
        [EInvoiceStatusList.Paid]: { "outer-item": true, "none-div": true },
      };
      this.typeAllowDrop = [
        EInvoiceStatusList.Fail,
        EInvoiceStatusList.Pending,
        EInvoiceStatusList.Paid,
      ];
    } else {
      this.coverList = {
        [EInvoiceStatusList.Chasing]: { "outer-item": true, "none-div": true },
      };
      this.typeAllowDrop = [EInvoiceStatusList.Chasing];
    }
    ev.dataTransfer.setData("rowData", data.id);
    ev.dataTransfer.setData("type", type);
  }

  async drop(ev, type) {
    ev.preventDefault();
    let rowData = ev.dataTransfer.getData("rowData");
    let typeDropped = ev.dataTransfer.getData("type");
    if (type != typeDropped) {
      let invoiceDropped = this.dataInvoicesMapped[typeDropped].find(
        (el) => el.id == rowData
      );
      let response = await this.$store.dispatch({
        type: "invoice/changeStatusInvoice",
        data: {
          id: invoiceDropped.id,
          status: this.listStatus.find((el) => el.name == type).id,
        },
      });
     await this.fetchData(true);

    }
  }

  selectInvoice(item) {
    if(!this.hadIdProject)
    this.$router.push({
      name: "invoiceDragDetail",
      params: { invoiceId: item.id, isViewDrag: "true" },
    });
  }
}
</script>
<style lang="scss">
.dragble-list-invoice {
  .ivu-split-trigger-con {
    min-height: 1200px;
  }
  .list-status {
    display: flex;
  }
  .drop-item {
    width: 100%;
    height: 115px;
    min-height: 25px;
    background: white;
    &:hover {
      background: #deebff;
    }
    padding: 7px;
  }
  .outer-item {
    border: 1px dashed black;
    background: #fbeded;
  }
  .hover-outer-item {
    border: 1px dashed black;
    background: #ffc7c7;
  }
  .list-item {
    width: 23%;
    float: left;
    margin-right: 10px;
    background: #f4f5f7;
    padding: 10px;
  }
  .none-div {
    .chilld-div {
      display: none;
    }
  }
  .header-list {
    font-weight: bold;
  }
  .right-detail-invoice {
    z-index: 99;
    right: 0px;
    background: white;
    height: 1000px;
    border-left: 1px solid black;
    padding:30px;
  }
  .pool-list-item {
    z-index: 1;
    display: flex;
    min-width: 1000px;
  }
  .split-detail {
    z-index: 99;
  }
  .left-list {
    overflow: scroll;
  }
}
</style>

