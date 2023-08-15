<template>
  <div class="dragble-list-deal">
    <Split
      class="split-detail"
      :style="{height:heightList *200+'px'}"
      v-model="splitRatio"
      v-if="isViewDetail"
    >
      <div slot="left">
        <div class="left-list">
          <div class="pool-list-item" :style="{height:heightList *200+'px'}">
            <List
              v-for="(data, name) in dataDealMapped"
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
                      @click="selectDeal(item)"
                      class="drop-item"
                      draggable="true"
                      @dragstart="(event)=> drag(item, event, name)"
                    >
                      <p>{{$t('deal.deal')}} : <a @click="viewDetail(item)">{{item.name}}</a></p>
                      <p>{{$t('deal.client')}}: {{ item.clientName }}</p>
                      <p>{{$t('deal.owner')}}: {{ item.owner }}</p>
                      <p>{{$t('deal.time')}}: {{ item.closingDate | moment("YYYY-MM-DD") }}</p>
                    </div>
                  </ListItem>
                </div>
              </div>
            </List>
          </div>
        </div>
      </div>
      <div slot="right" class="right-detail-deal">
        <DetailDealDrag  v-if="isViewDetail" />
      </div>
    </Split>
    <div v-else class="grid-list-deal">
      <div class="pool-list-item">
        <List
          v-for="(data, name) in dataDealMapped"
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
                  @click="selectDeal(item)"
                  class="drop-item"
                  draggable="true"
                  @dragstart="(event)=> drag(item, event, name)"
                >
                  <p>{{$t('deal.deal')}} : <a @click="viewDetail(item)">{{item.name}}</a></p>
                  <p>{{$t('deal.client')}}: {{ item.clientName }}</p>
                  <p>{{$t('deal.owner')}}: {{ item.ownerName }}</p>
                  <p>{{$t('deal.time')}}: {{ item.closingDate | moment("YYYY-MM-DD") }}</p>
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
import DetailDealDrag from "./detailDealDrag.vue";
import { EDealStatusList } from "../../../store/entities/status";
@Component({
  components: { DetailDealDrag },
  filters: {
    upperCase(value: any) {
      return value.toUpperCase();
    },
  },
})
export default class DragableListDeals extends AbpBase {
  @Prop() private dataDealProp;
  private dataDeal: any = [];
  private coverList: any = {
    waiting: {},
  };
  private splitRatio = 0.6;
  private typeAllowDrop: any = "";
  private isViewDetail = false;
  private dataDealMapped: any = {};
  private heightList = 4;
  hadIdProject = false;
  private listStatus = [
    { id: 0, name: EDealStatusList.RequestCome },
    { id: 1, name: EDealStatusList.ProcessingRequest },
    { id: 2, name: EDealStatusList.ProjectInProgress },
    { id: 3, name: EDealStatusList.ProjectWin },
    { id: 4, name: EDealStatusList.ProjectFail },
    { id: 5, name: EDealStatusList.DealLost },
  ];
  mounted() {
    this.dataDealMapped;
  }

  @Watch("$route")
  getDeal() {
    this.fetchData(true);
  }

  viewDetail(rowValue: any) {
    // this.hadIdProject = true
    if(rowValue.id) {
      let routeData = this.$router.resolve({ name: 'dealDetail', params: { dealId: rowValue.id } }); 
      window.open(routeData.href, '_blank');
    }
  }
  created() {
    this.fetchData(false);
  }

  async fetchData(isCall) {
    if (this.$route.params.dealId) {
      this.isViewDetail = true;
    } else {
      this.isViewDetail = false;
    }
    if (!isCall) {
      this.dataDeal = this.dataDealProp;
    } else {
      let response = await this.$store.dispatch({
        type: "deal/getAllPaging",
        data: {},
      });
      this.dataDeal = response.result.items;
    }
    this.listStatus.forEach((el) => {
      this.dataDealMapped[el.name] = [];
    });
    if (this.dataDeal) {
      this.dataDeal.forEach((el) => {
        el.statusName = this.listStatus.find((ele) => ele.id == el.status).name;
        this.dataDealMapped[el.statusName].push(el);
      });
    }
     let maxLength = 1;
      for (let key in this.dataDealMapped) {
        if (this.dataDealMapped[key].length > maxLength) {
          maxLength = this.dataDealMapped[key].length;
          this.heightList = this.dataDealMapped[key].length;
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
    if(data.changeToProcessing && !data.changeFromProcessingLost && !data.changeFromInProgressToWin && !data.changeFromProcessingToInProgress && !data.changeFromInProgressToFail) {
      this.coverList = {
        [EDealStatusList.ProcessingRequest]: { "outer-item": true, "none-div": true },
      }
      this.typeAllowDrop = [
        EDealStatusList.ProcessingRequest,
      ];
    }
    else if(data.changeToProcessing && data.changeFromProcessingLost && data.changeFromProcessingToInProgress) {
      this.coverList = {
        // [EDealStatusList.ProcessingRequest]: { "outer-item": true, "none-div": true },
        [EDealStatusList.DealLost]: { "outer-item": true, "none-div": true },
        [EDealStatusList.ProjectInProgress]: { "outer-item": true, "none-div": true },
      }
      this.typeAllowDrop = [
        // EDealStatusList.ProcessingRequest,
        EDealStatusList.DealLost,
        EDealStatusList.ProjectInProgress,
      ];
    }
    else if(data.changeToProcessing && data.changeFromInProgressToFail && data.changeFromInProgressToWin) {
      this.coverList = {
        [EDealStatusList.ProcessingRequest]: { "outer-item": true, "none-div": true },
        [EDealStatusList.ProjectFail]: { "outer-item": true, "none-div": true },
        [EDealStatusList.ProjectWin]: { "outer-item": true, "none-div": true },
      }
      this.typeAllowDrop = [
        EDealStatusList.ProcessingRequest,
        EDealStatusList.ProjectFail,
        EDealStatusList.ProjectWin,
      ];
    }
    ev.dataTransfer.setData("rowData", data.id);
    ev.dataTransfer.setData("type", type);
  }

  async drop(ev, type) {
    ev.preventDefault();
    let rowData = ev.dataTransfer.getData("rowData");
    let typeDropped = ev.dataTransfer.getData("type");
    if (type != typeDropped) {
      let dealDropped = this.dataDealMapped[typeDropped].find(
        (el) => el.id == rowData
      );      
      let response = await this.$store.dispatch({
        type: "deal/changeStatusDeal",
        data: {
          id: dealDropped.id,
          status: this.listStatus.find((el) => el.name == type).id,
        },
      });
     await this.fetchData(true);

    }
  }

  selectDeal(item) {
    if(!this.hadIdProject)
    this.$router.push({
      name: "dealDragDetail",
      params: { dealId: item.id, isViewDrag: "true" },
    }).catch(()=>{});
  }
}
</script>
<style lang="scss">
.dragble-list-deal {
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
  .right-detail-deal {
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

