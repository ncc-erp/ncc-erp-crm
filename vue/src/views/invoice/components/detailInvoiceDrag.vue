<template>
  <div class="invoice-detail" v-if="!isLoading">
    <h2>{{invoiceDetail.projectName}}</h2>
    <Row class="invoice-detail-row">
      <Col span="7">
        <span>{{$t('common.status')}}:</span>
      </Col>

      <Col span="17">
        <span>{{converInvoiceStatus(invoiceDetail.status)}}</span>
      </Col>
    </Row>
    <Row class="invoice-detail-row">
      <Col span="7">
        <span>{{$t('invoice.customerName')}}:</span>
      </Col>

      <Col span="17">
        <span><a @click="viewDetail(invoiceDetail.customerId)">{{invoiceDetail.customerName}}</a></span>
      </Col>
    </Row>
    <Row class="invoice-detail-row">
      <Col span="7">
        <span>{{$t('common.type')}}:</span>
      </Col>

      <Col span="17">
        <span>{{convertInvoiceType(invoiceDetail.type)}}</span>
      </Col>
    </Row>

    <Row class="invoice-detail-row">
      <Col span="7">
        <span>{{$t('project.value')}}:</span>
      </Col>

      <Col span="17">
        <span>{{invoiceDetail.value}}</span>
      </Col>
    </Row>
    <Row class="invoice-detail-row">
      <Col span="7">
        <span>{{$t('project.currency')}}:</span>
      </Col>

      <Col span="17">
        <span>{{curencyTypes}}</span>
      </Col>
    </Row>
    <Row class="invoice-detail-row">
      <Col span="7">
        <span>{{$t('invoice.assignee')}}:</span>
      </Col>

      <Col span="17">
        <Select
          class="select-assign"
          @on-change="onChangeAssignee"
          v-model="invoiceDetail.assignee"
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
    <Row class="invoice-detail-row">
      <Col span="7">
        <span>{{$t('invoice.invoiceDate')}}:</span>
      </Col>

      <Col span="17">
        <span>{{invoiceDetail.invoiceDate | convertedDate}}</span>
      </Col>
    </Row>
    <Row class="invoice-detail-row">
      <Col span="7">
        <span>{{$t('common.contract')}}:</span>
      </Col>

      <Col span="17">
        <span>{{invoiceDetail.contractName}}</span>
      </Col>
    </Row>
    <Row class="invoice-detail-row" v-if="invoiceDetail.type === EInvoiceType.FixPriced">
      <Col span="7">
        <span>{{$t('common.milestone')}}:</span>
      </Col>

      <Col span="17">
        <Table name="detailInvoice_tbl"
          :columns="milestoneColumns"
          :data="invoiceDetail.mileStoneDetail"
          border
          width="auto"
        >
          <template slot-scope="{ row }" slot="milestoneTime">
            <span>{{ row.milestoneTime | convertedDate }}</span>
          </template>
        </Table>
      </Col>
    </Row>

    <Row class="invoice-detail-row">
      <Col span="7">
        <span>{{$t('invoice.comment')}}:</span>
      </Col>

      <Col span="17">
        <Button
          v-if="!isShowCommentInput"
          @click="showCommentInput"
          size="small"
        >{{$t('invoice.addComment')}}</Button>
      </Col>
    </Row>

    <Row class="invoice-detail-row" v-if="isShowCommentInput">
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
    <Row class="invoice-detail-row" v-if="isShowCommentInput">
      <Col span="24">
        <Button
          size="small"
          @click="showCommentInput(); isClickedSubmitComment = false"
        >{{$t('common.cancel')}}</Button>
        <Button size="small" @click="submitComment">{{$t('invoice.comment')}}</Button>
      </Col>
    </Row>

    <div
      class="comment-row"
      v-for="(comment, index) in invoiceDetail.comments"
      :key="index"
      @click="showCommentDetail(index)"
    >
      <div>
        <Icon :type="comment.isShowDetail ? 'ios-arrow-down' : 'ios-arrow-forward'" />
        <span class="bolder-text">{{comment.userName}}</span>
        <span class>{{$t('invoice.commented')}}</span>
        <span>{{comment.createdDate | convertedDate}}</span>
      </div>
      <p v-if="comment.isShowDetail">{{comment.comment}}</p>
    </div>
  </div>
</template>
<script lang="ts">
import { Component, Vue, Prop, Watch } from "vue-property-decorator";
import Util from "@/lib/util";
import AbpBase from "@/lib/abpbase";
import moment from "moment";
import {ICommonList, ListType, ListStatus, IInvoiceUserDetail, IDataAddInvoice, IdataSavePeopleInCharge, EProjectType, CurrencyType, IAssignee, ISaveProject} from '@/store/entities/project'
import { EInvoiceType } from "../../../store/entities/invoice-type";
import { EInvoiceStatus } from "../../../store/entities/status";

@Component({
  filters: {
    convertedDate(value: string) {
      return moment(value).format("DD/MM/YYYY");
    },
  },
})
export default class DetailInvoiceDrag extends AbpBase {
  private invoiceDetail: any = [];
  private milestoneColumns: any = [];
  private newComment: string = "";
  private isLoading: boolean = false;
  private isShowCommentInput: boolean = false;
  private EInvoiceType = EInvoiceType;
  private EInvoiceStatus = EInvoiceStatus;
  private isClickedSubmitComment: boolean = false;
  private userList: any = [];
  get curencyTypes() {
    for(let item of CurrencyType) {
      if(item.id == this.invoiceDetail.currency) return item.name
    } return ''
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
    if (this.$route.params.invoiceId) {
      this.getAllUser();
      let response = await this.$store.dispatch({
        type: "invoice/getInvoiceDetail",
        invoiceId: this.$route.params.invoiceId,
      });
      response.result.mileStoneDetail = [response.result.mileStoneDetail];
      this.invoiceDetail = response.result;
      await this.getInvoiceComment();
    }
  }

  private viewDetail(id?: any) {
    if(id) {
      let routeData = this.$router.resolve({ name: 'detailCustomer', params: { customerId: id } }); 
      window.open(routeData.href, '_blank');
    }
  }
  async getAllUser() {
    let response = await this.$store.dispatch("invoice/getUserForInvoiceUser");
    this.userList = response.result;
  }

  private openDetailCustomer(data?: any) {
    this.$router.push({
      name: "detailCustomer",
      params: { customerId: data.id },
    });
  }

  async getInvoiceComment() {
    await this.$store
      .dispatch({
        type: "invoice/getInvoiceComment",
        data: {
          invoiceId: this.$route.params.invoiceId,
          skipCount: 0,
          maxResultCount: 5,
        },
      })
      .then((res) => {
        this.invoiceDetail.comments = res.result.items.map((item) => {
          item.isShowDetail = false;
          return item;
        });
      });
  }

  async onChangeAssignee() {
    if (this.invoiceDetail) {
      await this.$store.dispatch({
        type: "invoice/saveInvoice",
        data: {
          invoiceName: this.invoiceDetail.name,
          status: this.invoiceDetail.status,
          assignee: this.invoiceDetail.assignee,
          id: this.invoiceDetail.id
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

  converInvoiceStatus(value: number) {
    switch (value) {
      case EInvoiceStatus.Pending:
        return this.$t("common.pending");
      case EInvoiceStatus.Chasing:
        return this.$t("common.chasing");
      case EInvoiceStatus.Fail:
        return this.$t("common.fail");
      case EInvoiceStatus.Paid:
        return this.$t("common.paid");
    }
  }

  @Watch("$route")
  getInvoice() {
    this.fetchData();
  }

  showCommentDetail(index: number) {
    const comments = [...this.invoiceDetail.comments];
    comments[index].isShowDetail = !comments[index].isShowDetail;
    this.$forceUpdate();
  }

  showCommentInput() {
    this.isShowCommentInput = !this.isShowCommentInput;
  }

  async submitComment() {
    if (this.newComment) {
      await this.$store.dispatch({
        type: "invoice/submitComment",
        data: {
          userId: this.$store.state.session.user.id,
          invoiceId: this.invoiceDetail.id,
          comment: this.newComment,
          id: 0,
        },
      });
      this.newComment = "";
      await this.getInvoiceComment();
      this.$forceUpdate();
    } else {
      this.isClickedSubmitComment = true;
    }
  }
}
</script>
<style lang="scss" scoped>
.invoice-detail {
  .invoice-detail-row {
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
.invoice-detail {
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
