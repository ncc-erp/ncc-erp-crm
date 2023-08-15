<template>
    <div>
        <Card>
          <div class="select-date">
            {{$t('dashboard.statistics')}} {{$t('dashboard.from')}}
            <DatePicker name="dashboardStartDate_filter" @on-change="getLatestInformation" type="date" v-model="param.startDate" placeholder="Select date" style="width: 200px"></DatePicker>
            {{$t('dashboard.to')}}
            <DatePicker name="dashboardEndDate_filter" @on-change="getLatestInformation" type="date" v-model="param.endDate" placeholder="Select date" style="width: 200px"></DatePicker>
          </div>
          <Row>
            <Col span="6">
              <div class="information-zone color-clients">
                <div class="icon-clients"><i class="icon ivu-icon ivu-icon-md-people"></i></div>
                <div class="total-count">
                  <h3>{{$t('dashboard.clientsNumber')}}</h3>
                  <h1 v-if="informationData">{{informationData.clientsTotal}}</h1>
                </div>
              </div>
            </Col>
            <Col span="6">
              <div class="information-zone color-invoices">
                <div class="icon-invoices"><i class="icon ivu-icon ivu-icon-ios-document"></i></div>
                <div class="total-count">
                  <h3>{{$t('dashboard.dealsNumber')}}</h3>
                  <h1 v-if="informationData">{{informationData.invoicesTotal}}</h1>
                </div>
              </div>
            </Col>
            <Col span="6">
              <div class="information-zone color-projects">
                <div class="icon-projects"><i class="icon ivu-icon ivu-icon-ios-list-box"></i></div>
                <div class="total-count">
                  <h3>{{$t('dashboard.projectsNumber')}}</h3>
                  <h1 v-if="informationData">{{informationData.projectsTotal}}</h1>
                </div>
              </div>
            </Col>
            <Col span="6">
              <div class="information-zone color-accounts">
                <div class="icon-accounts"><i class="icon ivu-icon ivu-icon-ios-contact"></i></div>
                <div class="total-count">
                  <h3>{{$t('dashboard.accountsNumber')}}</h3>
                  <h1 v-if="informationData">{{informationData.accountsTotal}}</h1>
                </div>
              </div>
            </Col>
          </Row>
        </Card>
    </div>
</template>
<script lang="ts">
import { Component, Vue, Inject, Prop, Watch } from "vue-property-decorator";
import Util from "@/lib/util";
import AbpBase from "@/lib/abpbase";
import PageRequest from "../../../store/entities/page-request";
import moment from 'moment';
import { Row } from "view-design";
@Component({
  name: 'DashboardManagement',
  components: {
  }
})
export default class DashboardManagement extends AbpBase {
  private informationData: any = {
    clientsTotal: 0,
    projectsTotal: 0,
    invoicesTotal: 0,
    accountsTotal: 0,
  }
  private param: any = {
    startDate: '',
    endDate: ''
  }

  async created() {
    this.param.startDate = new Date(moment(new Date).format('MM/1/YYYY'))
    this.param.endDate = new Date(moment(new Date).add(1, 'months').format('MM/1/YYYY'))
    this.getLatestInformation()
  }

  private async getLatestInformation() {    
    if(this.param.startDate && this.param.endDate) {
      let param = {
        startDate: moment(this.param.startDate).format('MM/DD/YYYY').toString(),
        endDate:  moment(this.param.endDate).format('MM/DD/YYYY').toString(),
      }
      let data = await this.$store.dispatch({
        type:'dashboard/getLatestInformation',
        params: param
      })
      this.informationData.clientsTotal = data.clients.reduce((total, current) => {return total + current.total }, 0);
      this.informationData.invoicesTotal = data.invoices.reduce((total, current) => {return total + current.total }, 0);
      this.informationData.projectsTotal = data.projects.reduce((total, current) => {return total + current.total }, 0);
      this.informationData.accountsTotal = data.accountsCount.filter(x => x.value == true).reduce((total, current) => { return total + current.total }, 0);
    }
  }
}
</script>
<style lang="scss" scoped >
.color-clients {
   background-color:rgb(245, 81, 109);
 }
 .color-invoices {
   background-color:rgb(79, 223, 223);
 }
 .color-projects {
   background-color:rgb(159, 218, 70);
 }
 .color-accounts {
   background-color: rgb(164, 66, 221);
 }
 .information-zone {
   height: 90px;
   margin: 0px 10px;
   display: flex;
   color:white;
   .icon-clients {
     width: 90px;
     height: 100%;
     background-color:rgb(221, 36, 67);
     size: 50px;
   }
   .icon-invoices {
     width: 90px;
     height: 100%;
     background-color:rgb(35, 197, 197);
   }
   .icon-projects {
     width: 90px;
     height: 100%;
     background-color:rgb(127, 194, 26);
   }
    .icon-accounts {
      width: 90px;
      height: 100%;
      background-color: rgb(138, 35, 197);
    }
   .icon {
     position: relative;
     top: 50%;
     left: 50%;
     font-size: 70px;
     color: white;
     transform: translate(-50%, -50%);
   }
   .total-count {
     width: calc(100% - 90px);
     padding-top: 15px;
     padding-left: 20px;
   }
 }
 .select-date {
   padding: 10px 0px 20px 15px;
 }
</style>