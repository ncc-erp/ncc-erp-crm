<template>
    <div>
        <Card>
          <div class="page-body">
            <PageHeading>
              <template #header>{{$t('contact.contactManagement')}}</template>
              <template #button><Button name="contact_add" class="button btn-add" @click="isShowAddContact=true">{{$t('contact.addContact')}}</Button></template>
            </PageHeading>
            <Modal
              v-model="isShowAddContact"
              width="50%">
              <div slot="header" align="center"><h1>{{$t('contact.addContact')}}</h1></div>
              <addContact class="addInvoicePopup" @closeModal="collapse" @createCompleted="updateContacts" v-if="isShowAddContact"/>
              <div slot="footer" class="button-zone" align="center">
              </div>
            </Modal>
            <Form ref="queryForm" class="form">
              <FormItem>
                <Row type="flex">
                  <Col span="4" align="left">
                    <Input
                      name="contactName_filter"
                      search
                      @on-change="remoteMethod_name(searchName)"
                      v-model="searchName"
                      @on-search="filterSearch"
                      :placeholder="$t('contact.enterSearchName')"
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
            <div class="margin-top-10">
              <Table name="contactList_tbl" :columns="columns" :data="searchResult" :no-data-text="$t('common.nodatas')" border>
                <template slot-scope="{row}" slot="name">
                  <span>
                    <a @click="viewDetail(row)">{{row.name}}</a>
                  </span>
                </template>
                <template slot-scope="{ row }" slot="action">
                <Button
                  name="contact_edit"
                  class="btn-edit"
                  style="margin-right: 5px"
                  @click="openEditContact(row)"
                >{{$t('common.edit')}}</Button>
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
import addContact from "./components/add-contact.vue";
import {ICommonList, ListType, PageSizeOpts, ListStatus, IAssignee, ISaveProject, IDataList} from '@/store/entities/project'
@Component({
  name: 'ContactManagement',
  components: {
    PageHeading,
    addContact
  }
})
export default class ContactManagement extends AbpBase {
  private columns: any = []
  private searchResult: any = []
  private currentPage: number = 1
  private totalPage: number = 0
  pageSizeOpts: Array<number> = PageSizeOpts
  public isShowAddContact: boolean = false
  pageSize: number = 10
  private dataAll: any = []
  private nameList: any = []
  private searchName: string = ''
  private filterItems: any = []

  async created(){
    this.columns = [
    {
      title: this.$t('contact.name'),
      resizable: true,
      align: 'center',
      // key: "name",
      slot: "name",
    },
    {
      title: this.$t('contact.email'),
      key: "mail",
      resizable: true,
      align: 'center',
    },
    {
      title: this.$t('contact.phone'),
      key: "phone",
      resizable: true,
      align: 'center',
    },
    {
      title: this.$t('contact.role'),
      key: "role",
      resizable: true,
      align: 'center',
    },
    {
      title: this.$t('contact.description'),
      key: "description",
      resizable: true,
      align: 'center',
    },
    {
      title: this.$t("projectManagement.action").toString(),
      align: 'center',
      slot: "action"
    }
  ]
  await this.getAllPagging()
  this.getDataAll()
  }

  collapse(value: boolean = false) {
    this.isShowAddContact = value
    this.getAllPagging()
  }

  async getAllPagging() {
    
    let paramsData: any = {}
    
    paramsData.maxResultCount = this.pageSize
    let currentpage = parseInt(localStorage.getItem("currentPage"))
    if(currentpage) this.currentPage = currentpage;
    //get currentpage from localstorage
    paramsData.skipCount = (this.currentPage-1)*this.pageSize

    let itemSearch = localStorage.getItem("filterItems");
    if(itemSearch) {
      this.filterItems = JSON.parse(itemSearch)
      if(this.filterItems.length) this.searchName = this.filterItems[0].value
      else this.filterItems = []
    }
    //get filterItems from localstorage, set value search = value

    if (this.filterItems) {
      paramsData.filterItems = this.filterItems
    }
    let data = await this.$store.dispatch({
      type:'contact/getAllPagging',
      params: paramsData
    })
    this.searchResult = data.items
    this.totalPage = data.totalCount
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
    this.$router.push({ name: 'editContact', params: { contactId: rowValue.id } })
  }

  private viewDetail(rowValue: any) {
    let routeData = this.$router.resolve({ name: 'detailContact', params: { contactId: rowValue.id } }); 
    window.open(routeData.href, '_blank');
  }

  private remoteMethod_name(query) {
    if (query !== '') {
      const filter = this.dataAll.filter(item => item.name.toLowerCase().indexOf(query.toLowerCase()) > -1);
      let filte = [...new Set(filter.map(item => item.name))]; //distinct
      this.nameList = filte
    } else this.nameList = []
  }

   async getDataAll() {
     let data = await this.$store.dispatch({
      type:'contact/getAllPagging',
      params: {
          maxResultCount: this.totalPage,
          skipCount: 0
      }
    })
    this.dataAll = data.items
  }

  filterSearch() {
    this.closeOption()
    this.filterItems = [{
      propertyName: 'name',
      value: this.searchName.toLowerCase(),
      comparison: 6
    }]
    this.currentPage = 1
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
</style>