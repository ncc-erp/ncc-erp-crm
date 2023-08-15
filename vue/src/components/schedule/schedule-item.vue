<template>
  <div class="contract-item">
    <div class="contract-item-header">
      <Table :columns="columns" :data="tableData" :no-data-text="$t('common.nodatas')" border>
        <template slot-scope="{row}" slot="priority">
          <span>
            <p>{{row.priority | priority}}</p>
          </span>
        </template>
        <template slot-scope="{row}" slot="deadline">
          <span>
            <p>{{row.deadline | moment("YYYY-MM-DD")}}</p>
          </span>
        </template>
        <template slot-scope="{row}" slot="status">
          <span>
            <p>{{row.status | assignmentStatus}}</p>
          </span>
        </template>
        <template slot-scope="{row}" slot="action">
          <Button name="assignment_edit" class="button btn-add" size="small" @click="handleAddSchedule(row)">{{$t('common.edit')}}</Button>
        </template>
      </Table>
    </div>
    <AddAssignment :entity-id="entityId" @showAdd="showAdd()" :row-data="rowData"  v-if="isAddAssignment"/>
  </div>
</template>
<script lang="ts">
import { Component, Vue, Inject, Prop, Watch, Emit } from "vue-property-decorator";
import Util from "@/lib/util";
import AbpBase from "@/lib/abpbase";
import PageRequest from "@/store/entities/page-request";
import moment from "moment";
import { EAssignmentStatus, EPriority } from '@/store/entities/user';
import AddAssignment from "./add-assignment.vue";
@Component({
  filters: {
    priority(value: any) {
      let name = ''
      EPriority.forEach(element => {
        if (element.id === value) {
          name = element.name
        }
      });
      return name
    },

    assignmentStatus(value: any) {
      let name = ''
      EAssignmentStatus.forEach(element => {
        if (element.id === value) {
          name = element.name
        }
      });
      return name
    },
  },
  components: { AddAssignment }
})
export default class ScheduleItem extends AbpBase {
  @Prop() data: any
  @Prop() entityId: any
  private columns: any = []
  private tableData: any = []
  private EPriority = EPriority
  private EAssignmentStatus = EAssignmentStatus
  private isAddAssignment: boolean = false
  private rowData: any = {}

  async created(){
    this.columns = [
    {
      title: this.$t('common.name'),
      key: "name",
      align: 'center',
    },
    {
      title: this.$t('assignment.priority'),
      slot: "priority",
      align: 'center',
    },
    {
      title: this.$t('assignment.deadline'),
      slot: "deadline",
      align: 'center',
    },
    {
      title: this.$t('common.status'),
      slot: "status",
      align: 'center',
    },
    {
      title: this.$t('projectManagement.action'),
      // key: "action",
      slot: "action",
      resizable: true,
      align: 'center',
    },
  ]
  this.fetchData()
  }

  handleAddSchedule(data?: any) {
    this.rowData = data
    this.isAddAssignment = !this.isAddAssignment
  }
  @Emit('showAdd') showAdd() {
    this.isAddAssignment = false
            return
      }
  get entityType() {
        switch (this.$route.name) {
          case 'projectDetail':
            return 2;
          case 'dealDetail':
            return 4;
          case 'invoiceDetail':
            return 0;
          case 'detailCustomer':
            return 1;
        }
      }

  private async fetchData() {
    const respon = await this.$store.dispatch({
          type:'task/getListTasK',
          param: {
            EntityType:  this.entityType,
            EntityId: this.entityId,
          },
        })
    this.tableData = respon
    this.tableData = respon.map(element => {
      let _el = element
      _el.listUserId = []
      _el.listUserId.push(element.userId)
      respon.forEach(el => {
        if (element.id === el.id) {
          _el.listUserId.push(el.userId)
        }
      });
      _el.listUserId = [...new Set(_el.listUserId.map(item => item))]
      return _el
    })
  }

}
</script>
<style lang="scss" scoped>
.contract-item {
  border: 1px solid gray;
  margin: 10px 0;
  padding: 10px 20px;
  .contract-item-header {
    display: flex;
    justify-content: space-between;
    button {
      margin: 0 5px;
    }
  }
  .info-row {
    margin: 10px 0;
    .field-cols {
      display: flex;
      justify-content: flex-end;
      padding: 0 10px;
    }
    .value-cols {
      display: flex;
      padding: 0 10px;
    }
  }
}
.button-zone {
  display: flex;
  justify-content: center;
  button {
    margin: 0 10px;
  }
}
</style>