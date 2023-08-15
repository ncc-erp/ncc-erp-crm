<template>
  <div>
    <Card>
      <div class="page-body">
        <PageHeading>
          <template #header>{{ $t("assignment.emailTemplate") }}</template>
          <template #button>
            <Button name="emailTemplate_add" class="button btn-add" @click="openCreateEmailTemplate">
              {{$t("emailTemplate.add")}}
            </Button>
          </template>
        </PageHeading>
        <div class="content">
          <Table name="emailtemplate_tbl"
            :columns="columns"
            :data="tableData"
            :no-data-text="$t('common.nodatas')"
            border
          >
          <template slot-scope="{ row }" slot="action">
              <Button
                name="emailTemplate_edit"
                class="btn-edit"
                style="margin-right: 5px"
                @click="openEditEmailTemplate(row)"
              >{{$t('common.edit')}}</Button>
            </template>
          </Table>
          <Page
            show-sizer
            class-name="fengpage"
            :current="currentPage"
            :total="totalPage"
            class="margin-top-10"
            @on-page-size-change="pSize"
            @on-change="pNumber"
            :page-size-opts="pageSizeOpts"
          ></Page>
        </div>
      </div>
    </Card>
  </div>
</template>
<script lang="ts">
import { Component, Vue, Inject, Prop, Watch } from "vue-property-decorator";
import Util from "@/lib/util";
import AbpBase from "@/lib/abpbase";
import PageRequest from "../../store/entities/page-request";
import PageHeading from "../../components/heading/heading.vue";
import {
  ICommonList,
  ListType,
  PageSizeOpts,
  ListStatus,
  IAssignee,
  ISaveProject,
  IDataList,
} from "@/store/entities/project";
@Component({
  name: "EmailTemplate",
  components: {
    PageHeading,
  },
})
export default class EmailTemplate extends AbpBase {
  private columns: any = [];
  private tableData: any = [];
  private allDataEmail: any = [];
  currentPage: number = 1;
  totalPage: number = 0;
  pageSize: number = 10;
  pageSizeOpts: Array<number> = PageSizeOpts;

  @Watch("$route")
    getEvent() {
        this.getEmailTemplate();
    }

  async created() {
    this.getEmailTemplate();
    this.columns = [
      {
        title: this.$t("customerManagement.no"),
        resizable: true,
        align: "center",
        key: "no",
        width: 100,
        //   slot: "name",
      },
      {
        title: this.$t("common.name"),
        key: "name",
        resizable: true,
        align: "center",
      },
      {
        title: this.$t("assignment.subject"),
        key: "subject",
        resizable: true,
        align: "center",
      },
      {
        title: this.$t("projectManagement.action").toString(),
        slot: "action",
        align: "center",
        width: 150,
      }
    ];
  }

  async getEmailTemplate() {
    let No = 0;
    let response = await this.$store.dispatch({
      type: "email/getEmailTemplate",
    });
    this.allDataEmail = response.map((element) => {
      No++;
      let newData: any = element;
      newData.no = No;
      return newData;
    });
    this.totalPage = this.allDataEmail.length;
    this.getTableData();
  }

  getTableData() {
    if (this.allDataEmail && this.allDataEmail.length > 0) {
      this.tableData = this.allDataEmail.slice(
        (this.currentPage - 1) * this.pageSize,
        this.currentPage * this.pageSize
      );
    }
  }

  private pNumber(n?: number) {
    this.currentPage = n;
    this.getTableData();
  }
  pSize(s) {
    this.pageSize = s;
    this.pNumber(1);
  }

  openCreateEmailTemplate() {
    this.$router.push({ name: 'createEmailTemplate'});
  }

  openEditEmailTemplate(rowValue: any) {
    this.$router.push({ name: 'editEmailTemplate', params: { emailId: rowValue.id }});
  }
}
</script>
<style lang="scss" scoped>
.content {
  margin-top: 50px;
}
</style>
