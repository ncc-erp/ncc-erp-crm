<template>
  <div>
    <Card dis-hover>
      <div class="page-body">
        <Form ref="queryForm" :label-width="80" label-position="left" inline>
          <Row :gutter="16">
            <Col span="6">
              <FormItem :label="L('Keyword')+':'" style="width:100%">
                <Input name="userKeyword_filter" v-model="pagerequest.keyword" :placeholder="L('UserName')+'/'+L('Name')"></Input>
              </FormItem>
            </Col>
            <Col span="6">
              <FormItem :label="L('IsActive')+':'" style="width:100%">
                <!--Select should not set :value="'All'" it may not trigger on-change when first select 'NoActive'(or 'Actived') then select 'All'-->
                <Select name="userIsActive_filter" :placeholder="L('Select')" @on-change="isActiveChange">
                  <Option value="All">{{L('All')}}</Option>
                  <Option value="Actived">{{L('Actived')}}</Option>
                  <Option value="NoActive">{{L('NoActive')}}</Option>
                </Select>
              </FormItem>
            </Col>
            <Col span="6">
              <FormItem :label="'Creattion Time: '" style="width:100%">
                <DatePicker
                  name="userCreationTime_filter"
                  v-model="creationTime"
                  type="datetimerange"
                  format="dd/MM/yyyy"
                  style="width:100%"
                  @on-clear="clearDate"
                  placement="bottom-end"
                  :placeholder="L('SelectDate')"
                ></DatePicker>
              </FormItem>
            </Col>
          </Row>
          <Row>
            <Button name="user_add" @click="create" icon="android-add" type="primary" size="large">{{L('Add')}}</Button>
            <Button
              name="user_find"
              icon="ios-search"
              type="primary"
              size="large"
              @click="getpage"
              class="toolbar-btn"
            >{{L('Find')}}</Button>
          </Row>
        </Form>
        <div class="margin-top-10">
          <Table name ="user_tbl"
            :loading="loading"
            :columns="columns"
            :no-data-text="L('NoDatas')"
            border
            :data="list"
          ></Table>
          <Page
            show-sizer
            class-name="fengpage"
            :total="totalCount"
            class="margin-top-10"
            @on-change="pageChange"
            @on-page-size-change="pagesizeChange"
            :page-size="pageSize"
            :current="currentPage"
          ></Page>
        </div>
      </div>
    </Card>
    <create-user v-model="createModalShow" @save-success="getpage"></create-user>
    <edit-user v-model="editModalShow" @save-success="getpage"></edit-user>
  </div>
</template>
<script lang="ts">
import { Component, Vue, Inject, Prop, Watch } from 'vue-property-decorator';
import Util from '@/lib/util'
import AbpBase from '@/lib/abpbase'
import PageRequest from '@/store/entities/page-request'
import CreateUser from './create-user.vue'
import EditUser from './edit-user.vue'
import moment from 'moment';
class PageUserRequest extends PageRequest {
  keyword: string;
  isActive: boolean = null;//nullable
  from: Date | string;
  to: Date | string;
}

@Component({
  components: { CreateUser, EditUser }
})
export default class Users extends AbpBase {
  edit() {
    this.editModalShow = true;
  }
  //filters
  pagerequest: PageUserRequest = new PageUserRequest();
  creationTime: Date[] = [];

  createModalShow: boolean = false;
  editModalShow: boolean = false;
  get list() {
    return this.$store.state.user.list;
  };
  get loading() {
    return this.$store.state.user.loading;
  }
  create() {
    this.createModalShow = true;
  }
  convertedDate(value: string | Date) {
    if(value)
      return moment(value).format('DD/MM/YYYY')
    return null
  }
  async clearDate() {
    this.pagerequest.from = null;
    this.pagerequest.to = null;
    await this.$store.dispatch({
      type: 'user/getAll',
      data: this.pagerequest
    })
  }
  isActiveChange(val: string) {
    if (val === 'Actived') {
      this.pagerequest.isActive = true;
    } else if (val === 'NoActive') {
      this.pagerequest.isActive = false;
    } else {
      this.pagerequest.isActive = null;
    }
  }
  pageChange(page: number) {
    this.$store.commit('user/setCurrentPage', page);
    this.getpage();
  }
  pagesizeChange(pagesize: number) {
    this.$store.commit('user/setPageSize', pagesize);
    this.getpage();
  }
  async getpage() {
    this.pagerequest.maxResultCount = this.pageSize;
    this.pagerequest.skipCount = (this.currentPage - 1) * this.pageSize;
    //filters

    if (this.creationTime.length != 0) {
      if (this.creationTime[0].toString() != "") {
        this.pagerequest.from = moment(this.creationTime[0]).format('YYYY/MM/DD');
      }
      if (this.creationTime[1].toString() != "") {
        this.pagerequest.to = moment(this.creationTime[1]).format('YYYY/MM/DD');
      }
    }

    await this.$store.dispatch({
      type: 'user/getAll',
      data: this.pagerequest
    })
  }
  get pageSize() {
    return this.$store.state.user.pageSize;
  }
  get totalCount() {
    return this.$store.state.user.totalCount;
  }
  get currentPage() {
    return this.$store.state.user.currentPage;
  }
  columns = [{
    title: this.L('UserName'),
    key: 'userName'
  }, {
    title: this.L('Name'),
    key: 'name'
  }, {
    title: this.L('IsActive'),
    render: (h: any, params: any) => {
      return h('span', params.row.isActive ? this.L('Yes') : this.L('No'))
    }
  }, {
    title: this.L('CreationTime'),
    key: 'creationTime',
    render: (h: any, params: any) => {
      return h('span', this.convertedDate(params.row.creationTime))
    }
  }, {
    title: this.L('LastLoginTime'),
    render: (h: any, params: any) => {
      return params.row.lastLoginTime!=null? h('span', new Date(params.row.lastLoginTime).toLocaleString()):h('span','Chưa từng ghi nhận đăng nhập')
    }
  }, {
    title: this.L('Actions'),
    key: 'Actions',
    width: 150,
    render: (h: any, params: any) => {
      return h('div', [
        h('Button', {
          props: {
            type: 'primary',
            size: 'small'
          },
          style: {
            marginRight: '5px'
          },
          on: {
            click: () => {
              this.$store.commit('user/edit', params.row);
              this.edit();
            }
          },
        }, this.L('Edit')),
        h('Button', {
          props: {
            type: 'error',
            size: 'small'
          },
          on: {
            click: async () => {
              this.$Modal.confirm({
                title: this.L('Tips'),
                content: this.L('DeleteUserConfirm'),
                okText: this.L('Yes'),
                cancelText: this.L('No'),
                onOk: async () => {
                  await this.$store.dispatch({
                    type: 'user/delete',
                    data: params.row
                  })
                  await this.getpage();
                }
              })
            }
          }
        }, this.L('Delete'))
      ])
    }
  }]
  async created() {
    this.getpage();
    await this.$store.dispatch({
      type: 'user/getRoles'
    })
  }
}
</script>