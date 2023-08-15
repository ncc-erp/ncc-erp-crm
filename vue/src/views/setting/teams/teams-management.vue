<template>
    <div>
        <Card>
          <div class="page-body">
            <PageHeading>
              <template #header>{{$t('team.teamManagement')}}</template>
              <template #button><Button name="team_add" class="button btn-add" @click="isShowaddTeam=true">{{$t('team.addTeam')}}</Button></template>
            </PageHeading>
            <Modal
              v-model="isShowaddTeam"
              width="40%">
              <div slot="header" align="center"><h1>{{$t('team.addTeam')}}</h1></div>
              <addTeam class="addInvoicePopup" :showAdd="showAdd" @showAdd="showAdd" v-if="isShowaddTeam"/>
              <div slot="footer" class="button-zone" align="center">
              </div>
            </Modal>
            <Form ref="queryForm" class="form">
              <FormItem>
                <Row type="flex">
                  <Col span="4" align="left">
                    <Input
                      name="teamName_filter"
                      search
                      @on-change="remoteMethod_name(searchName)"
                      v-model="searchName"
                      @on-search="filterSearch"
                      :placeholder="$t('team.enterSearchName')"
                    />
                    <span class="option" >
                      <div
                        class="element-option"
                        v-for="(item,index) of nameList"
                        :key="index"
                        :value="item"
                      >
                        <span @click="editName(item)">{{item}}</span>
                      </div>
                    </span>
                  </Col>
                </Row>
              </FormItem>
            </Form>
            <div class="margin-top-10">
              <Table name="team_tbl" :columns="columns" :data="searchResult" :no-data-text="$t('common.nodatas')" border>
                <template slot-scope="{row}" slot="name">
                  <span>
                    <a @click="viewDetail(row)">{{row.name}}</a>
                  </span>
                </template>
                <template slot-scope="{ row, index }" slot="active">
                  <Checkbox :name="`teamIsActive${index}_input`" class="checkbox-active" v-model="row.isActive" size="large" @on-change="teamActive(row.id, row.isActive)"></Checkbox>
                </template>
                <template slot-scope="{ row }" slot="action">
                <Button
                  name="team_edit"
                  class="btn-edit"
                  style="margin-right: 5px"
                  @click="openEditContact(row)"
                >{{$t('common.edit')}}</Button>
                <Button
                  name="team_delete"
                  class="btn-edit"
                  style="margin-right: 5px"
                  @click="deleteTeam(row.id)"
                >{{$t('common.delete')}}</Button>
              </template>
              </Table>
              <Page show-sizer class-name="fengpage" :current="currentPage" :total="totalPage" class="margin-top-10" @on-page-size-change="pSize" @on-change="pNumber" :page-size-opts="pageSizeOpts"></Page>
            </div>
          </div>
        </Card>
    </div>
</template>
<script lang="ts">
import { Component, Vue, Inject, Prop, Watch } from "vue-property-decorator";
import Util from "@/lib/util";
import AbpBase from "@/lib/abpbase";
import PageRequest from "../../../store/entities/page-request";
import PageHeading from "../../../components/heading/heading.vue";
import addTeam from "./components/add-teams.vue";
import {ICommonList, ListType, PageSizeOpts, ListStatus, IAssignee, ISaveProject, IDataList} from '@/store/entities/project'
@Component({
  name: 'TeamsManagement',
  components: {
    PageHeading,
    addTeam
  }
})
export default class TeamsManagement extends AbpBase {
  private single: boolean = true
  private columns: any = []
  private searchResult: any = []
  private currentPage: number = 1
  private totalPage: number = 0
  pageSizeOpts: Array<number> = PageSizeOpts
  public isShowaddTeam: boolean = false
  pageSize: number = 10
  private dataAll: any = []
  private nameList: any = []
  private searchName: string = ''
  private filterItems: any = []

  async created(){
    this.columns = [
    {
      title: this.$t('team.teamName'),
      resizable: true,
      align: 'center',
      slot: "name",
    },
    {
      title: this.$t('team.active'),
      align: 'center',
      width: 150,
      slot: 'active',
    },
    {
      title: this.$t("projectManagement.action").toString(),
      align: 'center',
      slot: "action"
    }
  ]
  await this.getAllPagging()
  }

  showAdd() {
    this.isShowaddTeam = !this.isShowaddTeam
    this.getAllPagging()
  }

  async getAllPagging() {
    
    let paramsData: any = {}
    
    paramsData.maxResultCount = this.pageSize
    let currentpage = parseInt(localStorage.getItem("currentPage"))
    if(currentpage) this.currentPage = currentpage;
    let data = await this.$store.dispatch({
      type:'team/getAllPagging',
    })
    this.dataAll = data
    if (this.filterItems.length < 1) {
      this.searchResult = this.dataAll.slice((this.currentPage-1)*this.pageSize, this.currentPage*this.pageSize)
      this.totalPage = this.dataAll.length
      
    } else {
      this.searchResult = this.filterItems.slice((this.currentPage-1)*this.pageSize, this.currentPage*this.pageSize)
      this.totalPage = this.filterItems.length
    }
    localStorage.setItem("currentPage", "")
    localStorage.setItem("filterItems", "")
  }

  pSize(s) {
    this.pageSize = s
    this.getAllPagging()
  }

  pNumber(n) {
    this.currentPage = n
    this.getAllPagging()
  }

  private openEditContact(rowValue: any) {
    localStorage.setItem("currentPage", JSON.stringify(this.currentPage))
    localStorage.setItem("filterItems", JSON.stringify(this.filterItems))
    this.$router.push({ name: 'editTeams', params: { teamsId: rowValue.id } })
  }

  private viewDetail(rowValue: any) {
    let routeData = this.$router.resolve({ name: 'detailTeams', params: { teamsId: rowValue.id } }); 
    window.open(routeData.href, '_blank');
  }

  private remoteMethod_name(query) {
    if (query !== '') {
      const filter = this.dataAll.filter(item => item.name.toLowerCase().indexOf(query.toLowerCase()) > -1);
      let filte = [...new Set(filter.map(item => item.name))]; //distinct
      this.nameList = filte
    } else this.nameList = []
  }

  filterSearch() {
    this.closeOption()
    this.currentPage = 1
    const filter = this.dataAll.filter(item => item.name.toLowerCase().indexOf(this.searchName.toLowerCase()) > -1);
    this.filterItems = filter
    this.getAllPagging()
  }

  private closeOption() {
    this.nameList = []
  }

  private editName(name: any) {
    this.searchName = name
    this.nameList = []
    this.filterSearch()
  }

  async deleteTeam(teamId: number) {
    await this.$store.dispatch({
    type: "team/delete",
    id: teamId
    });
    this.$Message.info(this.$t('common.deleted'));
    this.getAllPagging()
  }

  private async teamActive(teamId: number, active: boolean) {
    const param = {
      id: teamId
    }
    if (active) {
      await this.$store.dispatch({
      type: "team/active",
      teamId: param
      });
    } else {
      await this.$store.dispatch({
      type: "team/deActive",
      teamId: param
      });
    }
    this.getAllPagging()
  }
}
</script>
<style lang="scss" scoped >
a:hover {
  color: blue;
  border-bottom: 1px solid blue;
}
.form {
  border: 1px solid gainsboro;
  padding: 10px;
}
.option {
  /* min-width: 100px;
      min-height: 0px; */
  max-height: 200px;
  overflow: auto;
  top: 35px;
  max-width: 100%;
  min-width: 100%;
  right: 0%;
  /* margin: 5px 0; */
  background-color: #fff;
  box-sizing: border-box;
  border-radius: 4px;
  box-shadow: 0 1px 6px rgba(0, 0, 0, 0.2);
  position: absolute;
  z-index: 900;
  .element-option {
    padding-left: 5px !important;
  }
}
.checkbox-active {
  size: 50;
}
</style>