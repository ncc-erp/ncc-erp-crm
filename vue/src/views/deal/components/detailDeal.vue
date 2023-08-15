<template>
    <div class="detail-deal">
        <PageHeading>
            <template #header>{{$t('deal.dealDetail')}}   
            </template>
             <template #button>
              <Button
                class="btn-edit"
                @click="openEditDeal(formData.id)"
                >{{$t('common.edit')}}
              </Button>
              <Button type="default"
                 @click="onBack"
                 >{{$t('deal.back')}}
              </Button>
            </template>
            
        </PageHeading>
        <Row>
          <Col :lg="10" :md="24" :sm="24">
            <Row type="flex" class="code-row-bg " align="middle">
                <Col span="6">{{$t('deal.name')}}:</Col>
                <Col span="18"><p>{{formData.name}}</p></Col>
            </Row>
            <Row type="flex" class="code-row-bg" align="middle">
                <Col span="6">{{$t('deal.status')}}:</Col>
                <Col span="18">
                    <p>{{converDealStatus(formData.status)}}</p>
                </Col>
            </Row>
            <Row type="flex" class="code-row-bg" align="middle">
                <Col span="6">{{$t('task.priority')}}:</Col>
                <Col span="18">
                    <p>{{converDealPriority(formData.priority)}}</p>
                </Col>
            </Row>
            <Row type="flex" class="code-row-bg" align="middle">
            <Col span="6">{{$t('deal.client')}}:</Col>
            <Col span="18">
                    <span>
                    <a @click="detailClient(formData.clientId)">{{formData.clientName}}</a>
                    </span>
            </Col>
            </Row>
            <Row type="flex" class="code-row-bg" align="middle">
                <Col span="6">{{$t('deal.owner')}}:</Col>
                <Col span="18"><p>{{formData.ownerName}}</p></Col>
            </Row>
            <Row type="flex" class="code-row-bg" align="middle">
                <Col span="6">{{$t('deal.amount')}}:</Col>
                <Col span="18"><p>{{formData.amount}}</p></Col>
            </Row>
            <Row type="flex" class="code-row-bg" align="middle">
                <Col span="6">{{$t('deal.dealStartDate')}}:</Col>
                <Col span="18"><p>{{convertedDate(formData.dealStartDate)}}</p></Col>
            </Row>
            <Row type="flex" class="code-row-bg" align="middle">
                <Col span="6">{{$t('deal.dealLastFollow')}}:</Col>
                <Col span="18"><p>{{convertedDate(formData.dealLastFollow)}}</p></Col>
            </Row>
          </Col>
          <Col :lg="14" :md="24" :sm="24">
            <Row type="flex" class="code-row-bg" align="middle">
                <Col span="6">{{$t('deal.closeDate')}}:</Col>
                <Col span="18"><p>{{convertedDate(formData.closingDate)}}</p></Col>
            </Row>
            <Row type="flex" class="code-row-bg" align="middle">
                <Col span="6">{{$t('deal.description')}}:</Col>
                <Col span="18"><p  v-html="xuongDong(formData.description)"></p></Col>
            </Row>
            <Row type="flex" class="code-row-bg" align="middle">
            <Col span="8">{{$t('project.project')}}:</Col>
            <Col span="16">
                    <span>
                    <a @click="detailProject(formData.project.id)" v-if="formData.project">{{formData.project.name}}</a>
                    </span>
            </Col>
            </Row>
            <!---lí do thắng thỏa thuận--->
            <Row type="flex" class="code-row-bg" align="middle">
                <Col span="6">{{$t('deal.winReason')}}:</Col>
                <Col span="18"><p>{{formData.winReason}}</p></Col>
            </Row>
            <!---lí do thắng thỏa thuận--->
            <Row type="flex" class="code-row-bg" align="middle">
                <Col span="6">{{$t('deal.loseReason')}}:</Col>
                <Col span="18"><p>{{formData.loseReason}}</p></Col>
            </Row>
            <Row type="flex" class="code-row-bg">
                <Col span="6">{{$t('deal.employeeFuture')}}:</Col>
                <Col span="18">
                  <Row v-for="(item,index) in formData.dealDetails" :key="index">
                    <p>{{item.skillName}} - {{item.levelName}} - {{item.quantity}}</p>
                  </Row>
                </Col>
            </Row>
          </Col>
        </Row>
        <Row class="code-row-bg attached-file" align="middle">
            <!-- <UploadFile :entity="entity" :entityId="this.$route.params.dealId" @handleInputFile="upload"></UploadFile> -->
              <Table name="detailDeal_tbl" border :columns="columnsFileTable" :data="dataFileTable" :no-data-text="$t('common.nodatas')" style="width: 100%">
                      <template slot-scope="{ row }" slot="fileUrl">
                          <a @click="openFile(row.fileUrl)">{{row.fileName}}</a>
                      </template> 
                      <template slot-scope="{ row }" slot="action">
                        <Icon type="md-close-circle" size="30"  @click="deleteFile(row.id)" />
                      </template>
              </Table>
          </Row>
        <Row class="deal-detail-row comment-title">
          <Col span="4">
            <span>{{$t('deal.comment')}}:</span>
          </Col>

          <Col span="20">
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
              name="inputComment"
              v-model="newComment"
              :class="{ 'error-input': (isClickedSubmitComment && !newComment)}"
              type="textarea"
              :rows="4"
              placeholder="Nhập bình luận..."
            />
          </Col>
        </Row>
        <Row class="deal-detail-row" v-if="isShowCommentInput" >
          <Col span="24">
            <Button size="small" @click="showCommentInput(); isClickedSubmitComment = false">{{$t('common.cancel')}}</Button>
            <Button size="small" @click="submitComment">{{$t('deal.comment')}}</Button>
          </Col>
        </Row>

      <div v-for="(comment, index) in listComments" :key="index">
        <details>
          <summary @click="isShowCommentInputReply = false" style="display: block">
            <div class="comment-row">
              <p style="float: right; margin-left: 20px" v-if="!isShowCommentInputParent">
                <Button size="small" @click="showCommentInputParent(comment.id); isClickedSubmitComment = false">
                  {{ $t('deal.reply') }}
                </Button>
               </p>
              <p style="word-wrap: break-word;"><strong>{{ comment.userName }}</strong> lúc
                <strong>{{ convertedDateTime(comment.commentTime) }}</strong>
              </p>
              <MoreLess :value="comment.content" :limit="100">
              </MoreLess>
              <a v-if="comment.childComments.length >0" style="float: right">{{ comment.childComments.length }} <i
                  style="margin-right: 0px" class="ivu-icon ivu-icon-md-return-left"></i>{{$t('deal.feedback')}}</a>
              <div v-if="dealCommentId === comment.id">
                <Row class="deal-detail-row" v-if="isShowCommentInputParent">
                  <Col span="24">
                    <Input
                        v-model="newCommentReply"
                        :class="{ 'error-input': (isClickedSubmitCommentParent && !newCommentReply)}"
                        type="textarea"
                        :rows="2"
                        placeholder="Nhập bình luận..."
                    />
                  </Col>
                </Row>
                <Row class="deal-detail-row" v-if="isShowCommentInputParent">
                  <Col span="24">
                    <Button size="small"
                            @click="showCommentInputParent(comment.id); isClickedSubmitCommentReply = false">
                      {{ $t('common.cancel') }}
                    </Button>
                    <Button size="small" @click="submitCommentReply">{{ $t('deal.reply') }}</Button>
                  </Col>
                </Row>
              </div>
            </div>
          </summary>
          <div v-for="(commentReply, index) in comment.childComments" :key="index">
            <div class="comment-row" style="margin-left: 50px">
              <p style="float: right; margin-left: 20px" v-if="!isShowCommentInputReply">
                <Button size="small" @click="showCommentInputReply(commentReply.id); isClickedSubmitComment = false">
                  {{ $t('deal.reply') }}
                </Button>
              </p>
              <p style="word-wrap: break-word;"><strong>{{ commentReply.userName }}</strong> lúc
                <strong>{{ convertedDateTime(commentReply.commentTime) }}</strong>
              </p>
              <MoreLess :value="commentReply.content" :limit="100" :name="getNameParentComment(commentReply.parentId)">
              </MoreLess>
              <div v-if="dealCommentId === commentReply.id">
                <Row class="deal-detail-row" v-if="isShowCommentInputReply">
                  <Col span="24">
                    <Input
                        v-model="newCommentReply"
                        :class="{ 'error-input': (isClickedSubmitCommentReply && !newCommentReply)}"
                        type="textarea"
                        :rows="2"
                        placeholder="Nhập bình luận..."
                    />
                  </Col>
                </Row>
                <Row class="deal-detail-row" v-if="isShowCommentInputReply">
                  <Col span="24">
                    <Button size="small"
                            @click="showCommentInputReply(comment.id); isClickedSubmitCommentReply = false">
                      {{ $t('common.cancel') }}
                    </Button>
                    <Button size="small" @click="submitCommentReply">{{ $t('deal.reply') }}</Button>
                  </Col>
                </Row>
              </div>
            </div>
          </div>
        </details>
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
    import { Component, Vue, Prop, Emit, Watch } from 'vue-property-decorator';
    import  MoreLess  from '../components/more-less.vue'
    import Util from '@/lib/util'
    import moment from 'moment';
    import AbpBase from '@/lib/abpbase'
    import PageHeading from "@/components/heading/heading.vue";
    import {ICommonList, ListType, ListStatus, ListStatusDeal, IAssignee, ISaveDeal, IFileDetail} from '@/store/entities/project'
    import { saveAs } from 'file-saver';
    import UploadFile from '../../../components/upload-file/upload-file.vue'
    import appconst from '../../../lib/appconst'
    import { EntityDefault } from '@/store/entities/entity';
    import ScheduleItem from "../../../components/schedule/schedule-item.vue";
    import AddAssignment from "../../../components/schedule/add-assignment.vue";
    import { EPriorityList } from '@/store/entities/task';
    @Component({
        name: 'detailDeal',
        components: {
          PageHeading, UploadFile,
          ScheduleItem,
          AddAssignment,
          MoreLess
        }
  
    })
export default class detailDeal extends AbpBase{
    private dataSubmit = {} as ISaveDeal
    private formData:any = {}
    private isClickedSubmitComment: boolean = false
    private isClickedSubmitCommentReply: boolean = false
      private isClickedSubmitCommentParent: boolean = false
    private newComment: string = '';
    private newCommentReply: string = '';
    dealDetail: any = []
    isShowCommentInput: boolean = false
    isShowCommentInputReply: boolean = false
      isShowCommentInputParent: boolean = false
    isEdit: boolean = false
    isProjectWin: boolean = false
    isProjectLose: boolean = false
    typeList = ListType
    statusList = []
    columnsFileTable: any = []
    dataFileTable: IFileDetail[] = []
    entity = EntityDefault.Deal
    priorityList = EPriorityList
    private arrowIcon: string = 'ios-arrow-down';
    private scheduleIcon: string = 'ios-arrow-down';
    private isShowScheduleList: boolean = true;
    private isAddAssignment: boolean = false
    private entityId: number = null
      private dealCommentId: number = null
      listComments = [];

    @Watch("$route")
    getEvent() {
        this.fetchData();
    }
    xuongDong(value: string) {
    if (value) {
      return value.split('\n').join('<br>');
    }
    }
    openEditDeal(id) {
      this.$router.push({ name: 'editDeal', params: { dealId: id } }).catch(()=>{})
    }
    detailClient(clientId) {
        this.$router.push({ name: 'detailCustomer', params: { customerId: clientId } }).catch(()=>{})
    }
    detailProject(id: any) {
        this.$router.push({ name: 'projectDetail', params: { projectId: id } })
    }
    editReason() {
        this.isEdit = !this.isEdit
    }
    convertedDate(value: string) {
        if(value) return moment(value).format('YYYY/MM/DD')
        else return ""
      }
      convertedDateTime(value: string) {
        if (value) return moment(value).format('HH:mm DD/MM/YYYY')
        else return ""
      }
    created() {
        this.columnsFileTable = [
            {
                title:this.$t('deal.attachment').toString(),
                align: 'center',   
                children: [
                    {
                        title: this.$t('common.name').toString(),
                        align: 'center',
                        slot: 'fileUrl',
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
    }
    private async fetchData() {
        this.entityId = Number(this.$route.params.dealId)
        this.formData = {}
        this.dataSubmit = {
            ownerId: null,
            description: "",
            clientId: null,
            status: null,
            priority: null,
            amount: null,
            name: "",
            id: null,
            winReason: "",
            loseReason: "",
            dealStartDate: new Date(),
            dealLastFollow: null,
            dealDetail: null
        }
       let respone = await this.$store.dispatch({
              type: "deal/getAllStatus",
              entityName: 'deal'
         });
        this.statusList = respone.listStatus.map((el) => {
              const _el = el;
              _el.id = el.status
              _el.name = el.statusName
              return _el
        });
        if(this.$route.params.dealId) {
            let response = await this.$store.dispatch({
            type: "deal/getDeal",
            id: this.$route.params.dealId
            });
            this.formData = response
            for(let key in this.dataSubmit) {
                if(key)
                this.dataSubmit[key] = response[key]
            }
          await this.getDealComment()
          this.selectStatus(this.dataSubmit.status)
          await this.getFiles(this.$route.params.dealId)
          this.getListComments()
        }
    }
    getListComments(){
      this.listComments = [];
      let places = this.dealDetail.comments
      places = places.sort((i, j) => i.parentId - j.parentId);
      let result = [];
      
      places.reduce((acc, place) => {
        let plc = { id: place.id, userName: place.userName,content: place.content,commentTime:place.commentTime,parentId:place.parentId, childComments: [] };
        if (place.parentId) {
          acc[place.parentId].childComments.push(plc);
        } else {
          result.push(plc);
        }
        acc[place.id] = plc;
        return acc;
      }, []);

      let results = [];
      const convertArrToObj = (arr) => {
        arr.forEach((e) => {
          if (e.childComments) {
            results.push({
              id: e.id,
              userName: e.userName,
              content: e.content,
              commentTime: e.commentTime,
              parentId: e.parentId,
              childComments: []
            });
            convertArrToObj(e.childComments);
          } else results.push(e);
        });
        return results;
      };
      result.reduce((acc, r) => {
        let plc = { id: r.id, userName: r.userName,content: r.content,commentTime:r.commentTime,parentId:r.parentId, childComments: [] };
        this.listComments.push(plc);
        acc[r.id] = plc;
        results=[];
        acc[r.id].childComments = convertArrToObj(r.childComments);
        return acc;
      }, []);
    }
    async getFiles(dealId) {
        if(dealId){
        let data = await this.$store.dispatch({
            type: 'deal/getFileByDeal',
            dealId: dealId
        })  
        this.dataFileTable = data.result
        this.dataFileTable.forEach((item:any)=>{
            item.fileName = item.fileUrl.replace("deal_file/","")
        })}
    }
    async upload(e) {
        if(e){
            await this.getFiles(this.$route.params.dealId);
        }
    }
    async deleteFile(dealFileId) {
        // let data = await this.$store.dispatch({
        //     type: 'deal/deleteDealFile',
        //     dealFileId: dealFileId
        // })
        // await this.getFiles(this.$route.params.dealId);
    }
    openFile(url){
        window.open(appconst.remoteServiceBaseUrl + url)
    }
    async onSubmit() {
        await this.$store.dispatch({
            type: 'deal/saveDeal',
            data: this.dataSubmit
        }) 
        this.$Message.info(this.$t('common.saved'));
        this.fetchData()
        this.editReason()
    }
    selectStatus(statusId: number) {
        if(statusId == this.statusList[3].id) {this.isProjectWin = true; this.isProjectLose = false}
        else if(statusId == this.statusList[4].id || statusId == this.statusList[5].id) {this.isProjectLose = true; this.isProjectWin = false}
        else {this.isProjectWin = false; this.isProjectLose = false}
    }
    beforeMount() {
        this.fetchData()
    }
    private onBack() {
        this.$router.push({ name: 'deal'}).catch(()=>{})
    }
    converDealStatus(value: number) {
        let type = ''
        this.statusList.forEach(el => {
            if (el.id === value) {
            type = el.name
            }
        })
        return type
    }
     converDealPriority(value: number) {
        let type = ''
        this.priorityList.forEach(el => {
            if (el.id === value) {
            type = el.name
            }
        })
        return type
    }
    async getDealComment() {
      await this.$store.dispatch({
        type: 'deal/getDealComment',
        data: {
          dealId: this.$route.params.dealId
        }
      }).then(res => {
        this.dealDetail.comments = res.result;
      })
    }
    // showCommentDetail(index: number) {
    //   const comments = [...this.dealDetail.comments];
    //   comments[index].isShowDetail = !comments[index].isShowDetail
    //   this.$forceUpdate();
    // }

    showCommentInput() {
      this.isShowCommentInput = !this.isShowCommentInput
    }
    showCommentInputReply(dealCommentId: number) {
      this.dealCommentId = dealCommentId;
      this.isShowCommentInputReply = !this.isShowCommentInputReply;
    }
      showCommentInputParent(dealCommentId: number) {
        this.dealCommentId = dealCommentId;
        this.isShowCommentInputParent = !this.isShowCommentInputParent;
      }

    async submitComment() {
      if (this.newComment) {
        await this.$store.dispatch({
          type: 'deal/submitComment',
          data: {
            dealId: this.$route.params.dealId,
            content: this.newComment
          }
        })
        this.newComment = ''
        this.isShowCommentInput = false;
        await this.getDealComment()
        await this.getListComments()
        this.$forceUpdate();
      }
      else {
        this.isClickedSubmitComment = true
      }
    }
      async submitCommentReply() {
        if (this.newCommentReply) {
          await this.$store.dispatch({
            type: 'deal/submitCommentReply',
            data: {
              dealId: this.$route.params.dealId,
              content: this.newCommentReply,
              id: this.dealCommentId
            }
          })
          this.newCommentReply = ''
          this.isShowCommentInputReply = false;
          this.isShowCommentInputParent = false;
          await this.getDealComment()
          await this.getListComments()
          this.$forceUpdate();
        }
        else {
          this.isClickedSubmitComment = true
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
  getNameParentComment(dealCommentId: number) {
        let userName = '';
        this.dealDetail.comments.forEach(el => {
          if (el.id === dealCommentId) {
            userName = el.userName 
          }
        })
        return userName
      }
    }
</script>
<style lang="scss" scoped>
 
    .code-row-bg{
        margin-top: 10px;
    }
    .detail-deal{
        padding: 20px;
    }
    .button-back{
      margin-top: 20px;
    }
    .deal-detail-row {
        margin: 10px 0;
        button {
            float: right;
            margin-left: 15px;
        }
    }
  .comment-row {
    width: 50%;
    margin: 10px 10px;
    padding: 10px 20px 20px 10px;
    background-color: #f7f7f7;
    border-radius: 15px;
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
  .error-input {
    textarea {
      border-color: red;
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
  }
   Button {
        margin-right: 5px;
    }
    .pageheading ::v-deep .ivu-col-span-18 {
      text-align: center;
      cursor: pointer;
      text-transform: uppercase;
      background-color: #d76767;
      color: white;
    }
    .attached-file {
      margin-bottom: 60px;
    }
    .comment-title ::v-deep .ivu-col-span-4 {
      text-align: center;
      text-transform: uppercase;
      background-color: #d76767;
      color: white;
      font-weight: bold;
      padding: 10px 0;
      font-size: 15px;
      line-height: 18px;
      margin: 10px 0;
    }
    .contract-list {
      margin-top: 60px;
    }
</style>
