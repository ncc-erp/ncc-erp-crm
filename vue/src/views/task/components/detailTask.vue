<template>
    <div class="detail-task">
        <PageHeading>
            <template #header>{{$t('task.taskDetail')}}</template>
            <template #button>
               <Button type="default"
                       @click="onBack"
                       >{{$t('task.back')}}</Button>
            </template>
        </PageHeading>
         <Row type="flex" class="code-row-bg" align="middle">
             <Col span="3">{{ $t("task.name") }}:</Col>
             <Col span="17">
             <p>{{ formData.name }}</p>
            </Col>
         </Row>
         <Row type="flex" class="code-row-bg" align="middle">
             <Col span="3">{{ $t("task.description") }}:</Col>
             <Col span="17">
             <p>{{ formData.description }}</p>
            </Col>
         </Row>
         <Row type="flex" class="code-row-bg" align="middle">
             <Col span="3">{{ $t("task.deadline") }}:</Col>
             <Col span="17">
             <p>{{ convertedDate(formData.deadline) }}</p>
            </Col>
         </Row>
        <Row type="flex" class="code-row-bg" align="middle">
             <Col span="3">{{ $t("task.status") }}:</Col>
             <Col span="17">
             <p>{{ formData.status }}</p>
            </Col>
         </Row>
         <Row type="flex" class="code-row-bg" align="middle">
             <Col span="3">{{ $t("task.priority") }}:</Col>
             <Col span="17">
             <p>{{ formData.priority }}</p>
            </Col>
         </Row>
         <Row type="flex" class="code-row-bg" align="middle">
             <Col span="3">{{ $t("task.entityAssignmentDtos") }}:</Col>
             <Col span="17">
             <p>{{ formData.entityName }}</p>
            </Col>
         </Row>
        <Row type="flex" class="code-row-bg" align="middle">
             <Col span="3">{{ $t("task.userId") }}:</Col>
             <Col span="17">
             <p v-if="userName">{{ userName }}</p>
            </Col>
         </Row>
        <div slot="footer" class="button-zone button-back"  align="center">
        </div>
    </div>
</template>
<script lang="ts">
    import { Component, Vue, Prop, Emit, Watch } from 'vue-property-decorator';
    import Util from '@/lib/util'
    import moment from 'moment';
    import AbpBase from '@/lib/abpbase'
    import PageHeading from "@/components/heading/heading.vue";
    import { saveAs } from 'file-saver';
    import UploadFile from '../../../components/upload-file/upload-file.vue'
    import appconst from '../../../lib/appconst'
    import { EStatus, EPriorityList } from '@/store/entities/task';
    @Component({
        name: 'detailTask',
        components: {
          PageHeading, UploadFile
        }

    })
export default class detailTask extends AbpBase{
    // private dataSubmit = {} as ISaveTask
    private formData:any = {}
    private isClickedSubmitComment: boolean = false
    private newComment: string = '';
    dealDetail: any = []
    isShowCommentInput: boolean = false
    isEdit: boolean = false
    isProjectWin: boolean = false
    isProjectLose: boolean = false
    userName: string = ''
    statusList = EStatus
    priorityList = EPriorityList
    columnsFileTable: any = []
        dataSubmit:any = {
            name: "",
            description: "",
            isActive: false,
            status: null,
            deadline: "",
            priority: null,
            entityAssignmentDtos: [],
            userIds: []
        }
    @Watch("$route")
    getEvent() {
        this.fetchData();
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
        if(value) return moment(value).format('DD/MM/YYYY')
        else return ""
    }
    created() {
        this.columnsFileTable = [
            {
                title:this.$t('task.attachment').toString(),
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
      async fetchData() {//
        this.formData = {}
        if (this.$route.params) {
          let param = {
            Id: this.$route.params.Id
          }
          let response = await this.$store.dispatch({
            type: "task/getTask",
            param
          });

          this.formData = response
          const priority = this.priorityList.find(x => x.id == this.formData.priority);
          const status = this.statusList.find(x => x.id == this.formData.status);
          if (priority) { this.formData.priority = priority.name }
          if (status) { this.formData.status = status.name }
          this.userName = response.user.fullName

          // for(let key in this.dataSubmit) {
          //     if(key)
          //     this.dataSubmit[key] = response[key]
          // }
          // this.getTaskComment()
          // this.selectStatus(this.dataSubmit.status)
          // this.getFiles(this.$route.params.dealId)
        }
        // async getFiles(dealId) {
        //     if(dealId){
        //     let data = await this.$store.dispatch({
        //         type: 'task/getFileByTask',
        //         dealId: dealId
        //     })
        //     this.dataFileTable = data.result
        //     this.dataFileTable.forEach((item:any)=>{
        //         item.fileName = item.fileUrl.replace("deal_file/","")
        //     })}
        // }
        // async upload(e) {
        //     if(e){
        //         await this.getFiles(this.$route.params.dealId);
        //     }
        // }
        // async deleteFile(dealFileId) {
        //     let data = await this.$store.dispatch({
        //         type: 'task/deleteTaskFile',
        //         dealFileId: dealFileId
        //     })
        //     await this.getFiles(this.$route.params.dealId);
        // }
        // openFile(url){
        //     window.open(appconst.remoteServiceBaseUrl + url)
        // }
        // async onSubmit() {
        //     await this.$store.dispatch({
        //         type: 'task/saveTask',
        //         data: this.dataSubmit
        //     })
        //     this.$Message.info(this.$t('common.saved'));
        //     this.fetchData()
        //     this.editReason()
        // }
        // selectStatus(statusId: number) {
        //     if(statusId == this.statusList[3].id) {this.isProjectWin = true; this.isProjectLose = false}
        //     else if(statusId == this.statusList[4].id || statusId == this.statusList[5].id) {this.isProjectLose = true; this.isProjectWin = false}
        //     else {this.isProjectWin = false; this.isProjectLose = false}
        // }
      }
    private beforeMount() {
        this.fetchData()
    }
    private onBack() {
        this.$router.push({name: 'task'}).catch(()=>{})
    }
    converTaskStatus(value: number) {
        // let type = ''
        // this.statusList.forEach(el => {
        //     if (el.id === value) {
        //     type = el.name
        //     }
        // })
        // return type
    }

    async getTaskComment() {
      await this.$store.dispatch({
        type: 'task/getTaskComment',
        data: {
          dealId: this.$route.params.dealId
        }
      }).then(res => {
        this.dealDetail.comments = res.result.map(item => {
          item.isShowDetail = false;
          return item
        })
      })
    }
    showCommentDetail(index: number) {
      const comments = [...this.dealDetail.comments];
      comments[index].isShowDetail = !comments[index].isShowDetail
      this.$forceUpdate();
    }

    showCommentInput() {
      this.isShowCommentInput = !this.isShowCommentInput
    }

    async submitComment() {
      if (this.newComment) {
        await this.$store.dispatch({
          type: 'task/submitComment',
          data: {
            dealId: this.$route.params.dealId,
            content: this.newComment
          }
        })
        this.newComment = ''
        await this.getTaskComment()
        this.$forceUpdate();
      }
      else {
        this.isClickedSubmitComment = true
      }
    }
}
</script>
<style lang="scss" scoped>

    .code-row-bg{
        margin-top: 10px;
    }
    .detail-task{
        padding: 20px;
    }
    .button-back{
      margin-top: 20px;
    }
    .task-detail-row {
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
  .error-input {
    textarea {
      border-color: red;
    }
  }
</style>
