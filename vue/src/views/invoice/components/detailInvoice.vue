<template>
  <div class="invoice-detail" v-if="!isLoading">
    <h2>{{$t('invoice.invoiceDetail')}}</h2>
    <Row>
      <Col span="24">
        <Row class="invoice-detail-row">
          <Col span="4">
            <span>{{$t('invoice.projectName')}}:</span>
          </Col>
            <Button name="invoice_download" @click="downloadFile()" class="button btn-add" >{{$t('invoice.exportExcel')}}</Button>
          <Col span="16">
            <span>{{invoiceDetail.projectName}}</span>
          </Col>
          <Col span="4">
          </Col>
        </Row>
        <Row class="invoice-detail-row">
          <Col span="4">
            <span>{{$t('common.type')}}:</span>
          </Col>

          <Col span="20">
            <span>{{convertInvoiceType(invoiceDetail.type)}}</span>
          </Col>
        </Row>
        <Row class="invoice-detail-row">
          <Col span="4">
            <span>{{$t('common.contract')}}:</span>
          </Col>

          <Col span="20">
            <span>{{invoiceDetail.contractName}}</span>
          </Col>
        </Row>
        <Row class="invoice-detail-row">
          <Col span="4">
            <span>{{$t('invoice.invoiceDate')}}:</span>
          </Col>

          <Col span="20">
            <span>{{invoiceDetail.invoiceDate | convertedDate}}</span>
          </Col>
        </Row>
        <Row class="invoice-detail-row">
          <Col span="4">
            <span>{{$t('common.status')}}:</span>
          </Col>

          <Col span="20">
            <span>{{converInvoiceStatus(invoiceDetail.status)}}</span>
          </Col>
        </Row>
        <Row class="invoice-detail-row">
          <Col span="4">
            <span>{{$t('invoice.customerName')}}:</span>
          </Col>

          <Col span="20">
            <span>{{invoiceDetail.customerName}}</span>
          </Col>
        </Row>
        <Row class="invoice-detail-row">
          <Col span="4">
            <span>{{$t('invoice.assignee')}}:</span>
          </Col>

          <Col span="20">
            <span>{{invoiceDetail.assigneeName}}</span>
          </Col>
        </Row>
        
        <Row class="code-row-bg" align="middle">
            <UploadFile :entity="entity" :entityId="this.$route.params.invoiceId" @handleInputFile="upload"></UploadFile>
            <Table name="invoiceFile-tbl" border :columns="columnsFileTable" :data="dataFileTable" :no-data-text="$t('common.nodatas')" style="width: 100%">
                    <template slot-scope="{ row }" slot="fileName">
                        <a @click="openFile(row.fileUrl)">{{row.fileName}}</a>
                    </template> 
                    <template slot-scope="{ row }" slot="type">
                        {{row.typeString}}
                    </template>
                    <template slot-scope="{ row }" slot="action">
                        <Button
                          name="invoiceFile_delete"
                            class="btn-edit"
                            style="margin-right: 5px"
                            @click="deleteFile(row.id)"
                        >{{$t('common.delete')}}</Button>
                    </template>
            </Table>
        </Row>
        <Row class="code-row-bg" align="middle" v-if="showTable">
            <p class="titleTable">{{$t('invoice.invoice')}}</p>
            <Table name="invoice_tbl" border :columns="columns" :data="dataTable" style="width: 100%">
              <template slot-scope="{ row }" slot="employee">
                  {{ row.employee }}
              </template>
              <template slot-scope="{ row }" slot="position" style="width: 100%">
                  {{row.position}}
              </template>
              <template slot-scope="{ row }" slot="rate" style="width: 100%">
                  {{row.rate}}
              </template>
              <template slot-scope="{ row }" slot="manMonth" style="width: 100%">
                  {{row.manMonth}}
              </template>
            </Table>
        </Row>

        <Row class="invoice-detail-row" v-if="invoiceDetail.type === EInvoiceType.FixPriced">
          <p>
            <span>{{$t('common.milestone')}}:</span>
          </p>

            <Table name="mineStone_tbl"
              :columns="milestoneColumns"
              :data="invoiceDetail.mileStoneDetail"
              border
              width="auto"
            >
              <template slot-scope="{ row }" slot="milestoneTime">
                <span>{{ row.milestoneTime | convertedDate }}</span>
              </template>
            </Table>
        </Row>

        <Row class="invoice-detail-row">
          <Col span="4">
            <span>{{$t('invoice.comment')}}:</span>
          </Col>

          <Col span="20">
            <Button
              name="invoiceComment_add"
              v-if="!isShowCommentInput"
              @click="showCommentInput"
              size="small"
            >{{$t('invoice.addComment')}}</Button>
          </Col>
        </Row>

        <Row class="invoice-detail-row" v-if="isShowCommentInput">
          <Col span="24">
            <Input
              name="invoiceComment_input"
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
            <Button name="invoiceComment_cancel" size="small" @click="showCommentInput(); isClickedSubmitComment = false">{{$t('common.cancel')}}</Button>
            <Button name="invoiceComment_save" size="small" @click="submitComment">{{$t('invoice.comment')}}</Button>
          </Col>
        </Row>
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
  import { Component, Vue, Prop } from 'vue-property-decorator';
  import Util from '@/lib/util';
  import AbpBase from '@/lib/abpbase';    
  import {ICommonList, ListType, ListStatus, IInvoiceUserDetail, IDataAddInvoice, IdataSavePeopleInCharge, EProjectType, CurrencyType, IAssignee, ISaveProject, IFileDetail} from '@/store/entities/project'
  import moment from 'moment';
  import { EInvoiceType } from '../../../store/entities/invoice-type';
  import { EInvoiceStatus } from '../../../store/entities/status';
  import { saveAs } from 'file-saver';
  import UploadFile from '../../../components/upload-file/upload-file.vue'
  import appconst from '../../../lib/appconst'
  import { EntityDefault } from '@/store/entities/entity';
  import ScheduleItem from "../../../components/schedule/schedule-item.vue";
  import AddAssignment from "../../../components/schedule/add-assignment.vue";

  @Component({
    components: {
      UploadFile,
      ScheduleItem,
      AddAssignment
      },
    filters: {
      convertedDate(value: string) {
        return moment(value).format('DD/MM/YYYY')
      },
    }
  })
  export default class DetailInvoice extends AbpBase {
    private invoiceDetail: any = [];
    private milestoneColumns: any = [];
    private newComment: string = '';
    private isLoading: boolean = false;
    private isShowCommentInput: boolean = false;
    private EInvoiceType = EInvoiceType;
    private EInvoiceStatus = EInvoiceStatus;
    private isClickedSubmitComment: boolean = false
    private assigneeList: any = []
    private arrowIcon: string = 'ios-arrow-down';
    private scheduleIcon: string = 'ios-arrow-down';
    private isShowScheduleList: boolean = false;
    private isAddAssignment: boolean = false
    private entityId: number = null
    columns: any = []
    columnsFileTable: any = []
    dataFileTable: IFileDetail[] = []
    dataTable: IInvoiceUserDetail[] = []
    showTable:boolean = false
    employee: number = null
    manMonth: number = null
    invoiceUserId: number = null
    entity = EntityDefault.Invoice
    rate: number = null
    position: string = ""
    async created() {
      this.entityId = Number(this.$route.params.invoiceId)
      this.isLoading = true;
      this.columns = [
        {
          title:this.$t('invoice.peopleInCharges').toString(),
          align: 'center',   
          children: [
              {
                  title: this.$t('project.employee').toString(),
                  slot: 'employee',
                  width: 200,
                  align: 'center',
              },
              {
                  title: this.$t('project.position').toString(),
                  slot: 'position',
                  align: 'center',
                  resizable: true,
              },
              {
                  title: this.$t('project.rate').toString(),
                  align: 'center',
                  slot: 'rate',
                  resizable: true,
              },
              {
                  title: this.$t('project.manmonth').toString(),
                  align: 'center',
                  slot: 'manMonth',
                  resizable: true,
              }
          ]
        }
      ]
      this.columnsFileTable = [
          {
              title:this.$t('invoice.attachment').toString(),
              align: 'center',   
              children: [
                  {
                      title: this.$t('common.name').toString(),
                      align: 'center',
                      slot: 'fileName',
                      resizable: true,
                  },
                  {
                      title: this.$t('common.fileType').toString(),
                      align: 'center',
                      slot: 'type',
                      resizable: true,
                  },
                  {
                      title: this.$t("projectManagement.action").toString(),
                      align: 'center',
                      slot: "action"
                  }
              ]
          }
      ]
      this.milestoneColumns = [
        {
          title: this.$t('project.mileStone'),
          key: 'mileStoneName',
          align: 'center',
        },
        {
          title: this.$t('project.description'),
          key: 'mileStoneDescription',
          align: 'center',
          ellipsis: true,
          width: 300
        },
        {
          title: this.$t('project.time'),
          slot: 'milestoneTime',
          key: 'milestoneTime',
          align: 'center',
        },
        {
          title: this.$t('project.percentage'),
          key: 'percentage',
          align: 'center',
        }
      ]
      
      this.allAssignee()
      if (this.$route.params.invoiceId) {
        let response = await this.$store.dispatch({
          type: 'invoice/getInvoiceDetail',
          invoiceId: this.$route.params.invoiceId
        })
        if(response.result.invoiceUserDetail.length > 0){
          this.dataTable = response.result.invoiceUserDetail.map(element => {
                        let data = element
                        this.assigneeList.map(item => {
                          if (item.userId === element.userId) {
                            data.employee = item.userName
                          }
                        });
                        return data
                    })
        }
        response.result.mileStoneDetail = [response.result.mileStoneDetail]
        this.invoiceDetail = response.result
        await this.getInvoiceComment()
        this.typeChanged(response.result.type)
        this.getFiles(this.$route.params.invoiceId)
      }
      this.isLoading = false;
    }
    
    typeChanged(value) {
        if(value==EProjectType.ODC || value == EProjectType.TNM) this.showTable = true
        else this.showTable = false
    }
    async getFiles(invoiceId) {
        let data = await this.$store.dispatch({
            type: 'invoice/getFileByInvoice',
            invoiceId: invoiceId
        })  
        this.dataFileTable = data.result
    }
    async upload(e) {
        if(e){
            await this.getFiles(this.$route.params.invoiceId);
        }
    }
    async deleteFile(invoiceFileId) {
        let data = await this.$store.dispatch({
            type: 'invoice/deleteInvoiceFile',
            invoiceFileId: invoiceFileId
        })
        await this.getFiles(this.$route.params.invoiceId);
    }
    openFile(url){
        window.open(appconst.remoteServiceBaseUrl + url)
    }
    async getInvoiceComment() {
      await this.$store.dispatch({
        type: 'invoice/getInvoiceComment',
        data: {
          invoiceId: this.$route.params.invoiceId,
          skipCount: 0,
          maxResultCount: 1000
        }
      }).then(res => {
        this.invoiceDetail.comments = res.result.items.map(item => {
          item.isShowDetail = false;
          return item
        })
      })

    }

    convertInvoiceType(value: any) {
      switch (value) {
        case EInvoiceType.ODC:
          return this.$t('common.odc');
        case EInvoiceType.FixPriced:
          return this.$t('common.fixedPrice');
        case EInvoiceType.TNM:
          return this.$t('common.tnm');
        case EInvoiceType.Internal:
          return this.$t('common.internal');
      }
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

    showCommentDetail(index: number) {
      const comments = [...this.invoiceDetail.comments];
      comments[index].isShowDetail = !comments[index].isShowDetail
      this.$forceUpdate();
    }

    showCommentInput() {
      this.isShowCommentInput = !this.isShowCommentInput
    }

    async submitComment() {
      if (this.newComment) {
        await this.$store.dispatch({
          type: 'invoice/submitComment',
          data: {
            userId: this.$store.state.session.user.id,
            invoiceId: this.invoiceDetail.id,
            comment: this.newComment,
            id: 0
          }
        })
        this.newComment = ''
        await this.getInvoiceComment()
        this.$forceUpdate();
      }
      else {
        this.isClickedSubmitComment = true
      }
    }

    async downloadFile() {
      await this.$store.dispatch({
        type: 'invoice/exportInvoice',
        invoiceId: this.$route.params.invoiceId
      }).then((res) => {
        const byteCharacters = atob(res.base64);
        const byteNumbers = new Array(byteCharacters.length);
        for (var i = 0; i < byteCharacters.length; i++) {
          byteNumbers[i] = byteCharacters.charCodeAt(i);
        }
        const byteArray = new Uint8Array(byteNumbers);
        const blob = new Blob([byteArray], { type: res.fileType });
        var FileSaver = require('file-saver');
        FileSaver.saveAs(blob, res.fileName);
      })
    } 
    async allAssignee() {
        let data = await this.$store.dispatch({
            type: 'invoice/getUserForInvoiceUser'
        })
        this.assigneeList = data.result
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
</style>
<style lang="scss">
.invoice-detail {
  .error-input {
    textarea {
      border-color: red;
    }
  }
}
</style>
