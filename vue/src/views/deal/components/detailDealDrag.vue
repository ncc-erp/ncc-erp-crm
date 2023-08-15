<template>
  <div class="deal-detail" v-if="!isLoading">
    <!-- {{ dealDetail }} -->
    <h2>{{dealDetail.name}}</h2>
    <Row class="deal-detail-row">
      <Col span="7">
        <span>{{$t('common.status')}}:</span>
      </Col>

      <Col span="17">
        <span>{{convertInvoiceStatus(dealDetail.status)}}</span>
      </Col>
    </Row>
    <Row class="deal-detail-row">
      <Col span="7">
        <span>{{$t('deal.client')}}:</span>
      </Col>

      <Col span="17">
        <span><a @click="viewDetail(dealDetail)">{{dealDetail.clientName}}</a></span>
      </Col>
    </Row>
    <Row class="deal-detail-row">
      <Col span="7">
        <span>{{$t('common.description')}}:</span>
      </Col>

      <Col span="17">
        <span>{{ dealDetail.description}}</span>
      </Col>
    </Row>

    <Row class="deal-detail-row">
      <Col span="7">
        <span>{{$t('deal.amount')}}:</span>
      </Col>

      <Col span="17">
        <span>{{dealDetail.amount}}</span>
      </Col>
    </Row>
    <Row class="deal-detail-row">
      <Col span="7">
        <span>{{$t('invoice.assignee')}}:</span>
      </Col>

      <Col span="17">
        <Select
          class="select-assign"
          @on-change="onChangeAssignee"
          v-model="dealDetail.ownerId"
          filterable
        >
          <Option
            v-for="item in userList"
            :value="item.userId"
            :key="item.userId"
          >{{ item.userName }}</Option>
        </Select>
      </Col>
    </Row>
    <Row class="deal-detail-row">
      <Col span="7">
        <span>{{$t('deal.closeDate')}}:</span>
      </Col>

      <Col span="17">
        <span>{{dealDetail.closingDate | convertedDate}}</span>
      </Col>
    </Row>

    <Row class="deal-detail-row">
      <Col span="7">
        <span>{{$t('deal.comment')}}:</span>
      </Col>

      <Col span="17">
        <Button
          v-if="!isShowCommentInput"
          @click="showCommentInput"
          size="small"
        >{{$t('deal.addComment')}}</Button>
      </Col>
    </Row>

    <Row class="deal-detail-row" v-if="isShowCommentInput">
      <Col span="24">
        <Input
          v-model="newComment"
          :class="{ 'error-input': (isClickedSubmitComment && !newComment)}"
          type="textarea"
          :rows="4"
          placeholder="Nhập bình luận..."
        />
      </Col>
    </Row>
    <Row class="deal-detail-row" v-if="isShowCommentInput">
      <Col span="24">
        <Button
          size="small"
          @click="showCommentInput(); isClickedSubmitComment = false"
        >{{$t('common.cancel')}}</Button>
        <Button size="small" @click="submitComment">{{$t('deal.comment')}}</Button>
      </Col>
    </Row>
    <Row class="deal-detail-row">
      <Col span="24">
    <div
      class="comment-row"
      v-for="(comment, index) in commentData"
      :key="index"
    >
       <div>
        <Icon :type="comment.isShowDetail ? 'ios-arrow-down' : 'ios-arrow-forward'" />
        <span class="bolder-text">{{comment.content}}</span>
      </div>
      <p v-if="comment.isShowDetail">{{comment.content}}</p> 
    </div> 
      </Col>
    </Row>
  </div>
</template>
<script lang="ts">
import { Component, Vue, Prop, Watch } from "vue-property-decorator";
import Util from "@/lib/util";
import AbpBase from "@/lib/abpbase";
import moment from "moment";
import {ICommonList, ListType, ListStatus, IInvoiceUserDetail, IDataAddInvoice, IdataSavePeopleInCharge, EProjectType, CurrencyType, IAssignee, ISaveProject} from '@/store/entities/project'
import { EInvoiceType } from "../../../store/entities/invoice-type";
import { EDealStatusList } from "../../../store/entities/status";

@Component({
  filters: {
    convertedDate(value: string) {
      return moment(value).format("DD/MM/YYYY");
    },
  },
})
export default class DetailInvoiceDrag extends AbpBase {
  private dealDetail: any = [];
  private commentData: any = [];
  private milestoneColumns: any = [];
  private newComment: string = "";
  private isLoading: boolean = false;
  private isShowCommentInput: boolean = false;
  private EInvoiceType = EInvoiceType;
  private EDealStatusList = EDealStatusList;
  private listStatus = [
    { id: 0, name: EDealStatusList.RequestCome },
    { id: 1, name: EDealStatusList.ProcessingRequest },
    { id: 2, name: EDealStatusList.ProjectInProgress },
    { id: 3, name: EDealStatusList.ProjectWin },
    { id: 4, name: EDealStatusList.ProjectFail },
    { id: 5, name: EDealStatusList.DealLost },
  ];
  private isClickedSubmitComment: boolean = false;
  private userList: any = [];

  @Watch("$route")
  getEvent() {
      this.fetchData();
  }
  get curencyTypes() {
    for(let item of CurrencyType) {
      if(item.id == this.dealDetail.currency) return item.name
    } return ''
  }
  viewDetail(data: any) {
    if(data.clientId){
      let routeData = this.$router.resolve({ name: 'detailCustomer', params: { customerId: data.clientId } }); 
      window.open(routeData.href, '_blank');
    }
  }
  async created() {
    this.isLoading = true;
    this.milestoneColumns = [
      {
        title: this.$t("project.mileStone"),
        key: "mileStoneName",
        align: "center",
      },
      {
        title: this.$t("project.description"),
        key: "mileStoneDescription",
        align: "center",
        ellipsis: true,
      },
      {
        title: this.$t("project.time"),
        slot: "milestoneTime",
        key: "milestoneTime",
        align: "center",
      },
      {
        title: this.$t("project.percentage"),
        key: "percentage",
        align: "center",
      },
    ];
    this.fetchData();
    this.isLoading = false;
  }
  async fetchData() {    
    if (this.$route.params.dealId) {
      this.getAllUser();
      await this.getDealComment();
      let response = await this.$store.dispatch({
        type: "deal/getDeal",
        id: this.$route.params.dealId,
      });      
      this.dealDetail = response;
    }
  }

  async getAllUser() {
    let response = await this.$store.dispatch({
            type: 'project/getAssigneeList'
        })
    this.userList = response.result;
  }

  private openDetailCustomer(data?: any) {
    this.$router.push({
      name: "detailCustomer",
      params: { customerId: data.id },
    });
  }

  async getDealComment() {
    if(this.$route.params.dealId) {
      await this.$store
        .dispatch({
          type: "deal/getDealComment",
          data: {
            dealId: this.$route.params.dealId
          },
        })
        .then((res) => {
          this.commentData = res.result.map((item) => {
            item.isShowDetail = false;
            return item;
        });
      });
    }
  }

  async onChangeAssignee() {
    if (this.dealDetail) {
      await this.$store.dispatch({
        type: "invoice/saveInvoice",
        data: {
          invoiceName: this.dealDetail.name,
          status: this.dealDetail.status,
          assignee: this.dealDetail.assignee,
          id: this.dealDetail.id
        },
      });
    }
  }

  convertInvoiceType(value: any) {
    switch (value) {
      case EInvoiceType.ODC:
        return this.$t("common.odc");
      case EInvoiceType.FixPriced:
        return this.$t("common.fixedPrice");
      case EInvoiceType.TNM:
        return this.$t("common.tnm");
      case EInvoiceType.Internal:
        return this.$t("common.internal");
    }
  }

  convertInvoiceStatus(value: number) {
    switch (value) {
      case this.listStatus[0].id:
        return this.$t(EDealStatusList.RequestCome);
      case this.listStatus[1].id:
        return this.$t(EDealStatusList.ProcessingRequest);
      case this.listStatus[2].id:
        return this.$t(EDealStatusList.ProjectInProgress);
      case this.listStatus[3].id:
        return this.$t(EDealStatusList.ProjectWin);
      case this.listStatus[4].id:
        return this.$t(EDealStatusList.ProjectFail);
      case this.listStatus[5].id:
        return this.$t(EDealStatusList.DealLost);
    }
  }

  // showCommentDetail(index: number) {
  //   const comments = [...this.dealDetail.comments];
  //   comments[index].isShowDetail = !comments[index].isShowDetail;
  //   this.$forceUpdate();
  // }

  showCommentInput() {
    this.isShowCommentInput = !this.isShowCommentInput;
  }

  async submitComment() {
    if (this.newComment) {
      await this.$store.dispatch({
        type: "deal/submitComment",
        data: {
          dealId: this.dealDetail.id,
          content: this.newComment,
        },
      });
      this.newComment = "";
      await this.getDealComment();
      this.$forceUpdate();
    } else {
      this.isClickedSubmitComment = true;
    }
  }
}
</script>
<style lang="scss" scoped>
.deal-detail {
  .deal-detail-row {
    margin: 10px 0;
    button {
      float: right;
      margin-left: 15px;
    }
  }
  .comment-row {
    margin: 10px 0;
    padding: 5px;
    background-color: #f0f2f5;
    border-radius: 3px;
    cursor: pointer;
    i {
      margin-right: 10px;
    }
    span {
      margin: 0 3px;
    }
    .bolder-text {
      font-weight: bold;
    }
    p {
      margin-left: 25px;
    }
  }
}
</style>
<style lang="scss">
.deal-detail {
  .error-input {
    textarea {
      border-color: red;
    }
  }
  .customer-name {
    color: rgb(78, 78, 248);
    &:hover {
      cursor: pointer;
      color: #ff6464;
    }
  }
  .select-assign {
    width: 125px;
  }
}
</style>
