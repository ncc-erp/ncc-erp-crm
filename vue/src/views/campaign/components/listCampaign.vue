<template>
  <div>
    <Card>
      <div class="page-body" @click="closeOption">
        <PageHeading>
          <template #header>{{$t('emailCampaign.listCampaign')}}</template>
          <template #button>
            <Button
              name="campaign_add"
              class="button btn-add"
              @click="addCampaign"
            >{{$t('common.add')}}</Button>
          </template>
        </PageHeading>
        <div class="margin-top-10">
          <Form ref="queryForm" class="form">
            <FormItem>
              <Row type="flex">
                <Col span="4" align="left">
                  <Input
                    name="campaignName_filter"
                    search
                    @on-change="remoteMethod(searchName)"
                    v-model="searchName"
                    @on-search="filterSearch"
                    :placeholder="$t('emailCampaign.enterSearchName')"
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
                  <Input style="display: none"/>
                </Col>
              </Row>
              </FormItem>
            </Form>
          <Table name="listcampaign_tbl"
            style="margin-top: 10px;"
            :loading="loading"
            :columns="columns"
            :data="searchResult"
            :no-data-text="$t('common.nodatas')"
            border
          >
            <template slot-scope="{ row }" slot="no">
              {{ (currentPage - 1) * 10 + row._index+1 }}
            </template>
            <template slot-scope="{ row }" slot="action">
              <Button
                name="campaign_detail"
                class="button btn-add"
                @click="viewDetail(row.id)"
              >
                {{$t('common.detail')}}
              </Button>
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
import Util from "../../../lib/util";
import AbpBase from "../../../lib/abpbase";
import PageRequest from "../../../store/entities/page-request";
import PageHeading from "../../../components/heading/heading.vue";
import {ICommonList, ListType, PageSizeOpts, ListStatus, IAssignee, ISaveProject, IDataList} from '@/store/entities/project'
class PageRoleRequest extends PageRequest {
  keyword: string = "";
}
@Component({
  name: 'ListCampaign',
  components: {
    PageHeading,
  }
})
export default class ListCampaign extends AbpBase {
  currentPage: number = 1
  private totalCount: number = 1
  private pageSize: number = 10
  totalPage: number = 0
  private emailList: any = []
  private option: any = []
  private campaignName: any = ''
  private filterItems: any = []
  private dataAll: any = []
  private isViewDrag: boolean = false;
  searchName: string = '';
  searchResult: any = [];
  nameList: any = [];
  pageSizeOpts: Array<number> = PageSizeOpts

    get edit() {
        return this.editModalShow = true;
    }
    data = []
    year: number = new Date().getFullYear();
    yearCollect: any = []
    pagerequest: PageRoleRequest = new PageRoleRequest();
    columns: any
    isShow: boolean = false;
    createModalShow: boolean = false;
    editModalShow: boolean = false;
    loading: boolean = false;
    check: boolean = false;
    allStatusList: any = []

    remoteMethod(query) {
      if (query !== '') {
        const filter = this.dataAll.filter(item => item.campaignName.toLowerCase().indexOf(query.toLowerCase()) > -1);
        let filte = [...new Set(filter.map(item => item.campaignName))]; //distinct
        this.nameList = filte
      } else {
        this.nameList = []
      }
    }

    closeOption() {
      this.nameList = []
    }
    
    viewDetail(id:number) {
      this.$router.push({
        name: "detailCampaign",
        params: { campaignId: id.toString( ) }
      }).catch(()=>{})
    }

    private onChangePage(page) {
        this.currentPage = page
        this.getAllPaging()
    }

    async getAllPaging() {
      let paramsData: any = {}
      paramsData.maxResultCount = this.pageSize;

      // get currentpage from localstorage
      let currentPage = parseInt(localStorage.getItem("currentPage"))
      if (currentPage) {
        this.currentPage = currentPage;
      }
      paramsData.skipCount = (this.currentPage-1) * this.pageSize

      // get filterItems from localstorage, set value search = value
      let itemSearch = localStorage.getItem("filterItems");
      if (itemSearch) {
        this.filterItems = JSON.parse(itemSearch);
        if (this.filterItems.length) {
          this.searchName = this.filterItems[0].value
        }
        else {
          this.filterItems = []
        }
      }
    
      if (this.filterItems) {
        paramsData.filterItems = this.filterItems
      }
      let data = await this.$store.dispatch({
        type: 'campaign/getAllPaging',
        params: paramsData
      })
      this.searchResult = data.items
      this.totalPage = data.totalCount
      localStorage.setItem("currentPage", "")
      localStorage.setItem("filterItems", "")
    }

    pSize(s) {
      this.pageSize = s
      this.getAllPaging()
    }

    pNumber(n) {
      this.currentPage = n
      this.getAllPaging()
    }

    filterSearch() {
      this.closeOption()
      this.filterItems = [{
        propertyName: 'campaignName',
        value: this.searchName.toLowerCase(),
        comparison: 6
      }]
      this.currentPage = 1
      this.getAllPaging()
    }

    editName(name: any) {
      this.searchName = name
      this.nameList = []
      this.filterSearch()
    }

    async getDataAll() {
      let data = await this.$store.dispatch({
        type: 'campaign/getAllPaging',
        params: {
          maxResultCount: this.totalPage,
          skipCount: 0
        }
      })
      this.dataAll = data.items
    }

    async getAllPagging() {
        const response = await this.$store.dispatch({
        type: "campaign/getAllCampaign"
        })
        this.data = response.result
    }
    addCampaign() {
        this.$router.push({path: 'campaign/create-campaign'})
    }
    private onChangeSize() {
        this.currentPage = 1
        this.getAllPaging()
    }
    async created() {
      this.columns = [
        {
          title: this.$t("common.no").toString(),
          slot: "no",
          align: 'center',
          width: 100
        },
        {
          title: this.$t("emailCampaign.name").toString(),
          key: "campaignName",
          align: 'left',
          resizable: true,
        },
        {
          title: this.$t("common.setting").toString(),
          slot: "action",
          align: 'center',
          width: 150
        },
      ];
      await this.getAllPaging()
      this.getDataAll()
    }
}
</script>

<style lang="scss" scoped>
.form {
  border: 1px solid gainsboro;
  padding: 10px;
}
.page-select {
  width: 100%;
  display: flex;
  flex-wrap: wrap;
}
.select-size {
  width: 87px;
  margin-left: 30px;
}
a:hover {
  color: blue;
  border-bottom: 1px solid blue;
}
</style>
<style lang="scss" scoped>
  .switch-view {
    margin: 10px 0;
    display: flex;
    align-items: center;
    span {
      margin-right: 10px;
    }
  }
.option {
    max-height: 200px;
    overflow: auto;
    top: 35px;
    width: 200px;
    left: 0%;
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

</style>
