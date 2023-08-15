<template>
  <div>
    <Modal v-model="isShow" width="80" class="edit-contract-popup" @on-visible-change="handleClose">
      <h2>{{$t('project.editInvoice')}}</h2>
      <Row class="info-row">
        <Table name="editInvoice_tbl"
          :columns="invoiceColumns"
          :data="listInvoice.newPeopleInCharges"
          border
          :resizable="true"
        >
          <template slot-scope="{ row, index }" slot="userId">
            <Select
              :value="row.userId"
              @on-change="(event) => onChangeValue(event, 'userId', index)"
              width="100"
            >
              <Option
                v-for="item in userList"
                :value="item.userId"
                :key="item.userId"
              >{{ item.userName }}</Option>
            </Select>
          </template>
          <template slot-scope="{ row, index }" slot="position">
            <Input
              @on-change="(event) => onChangeValue(event, 'position', index)"
              :value="row.position"
            />
          </template>
          <template slot-scope="{ row, index }" slot="rate">
            <Input @on-change="(event) => onChangeValue(event, 'rate', index)" :value="row.rate" />
          </template>
          <template slot-scope="{ row, index }" slot="manMonth">
            <Input
              @on-change="(event) => onChangeValue(event, 'manMonth', index)"
              :value="row.manMonth"
            />
          </template>
          <template slot-scope="{ row, index }" slot="action">
            <Checkbox
              @on-change="(event) => onChangeValue(event, 'isAllMonth', index)"
              v-if="row.invoiceUserId !== 0"
              :value="row.isAllMonth"
            >All month</Checkbox>
            <Button type="error" size="default" @click="deleteRow(index)">X</Button>
          </template>
        </Table>
      </Row>
      <Row>
        <Button type="primary" size="default" style="float: right" @click="addMoreRow">+</Button>
      </Row>
      <Row slot="footer" class="button-zone">
        <Button
          type="error"
          size="default"
          :loading="submit_loading"
          @click="submitInvoice"
        >{{$t('common.save')}}</Button>
        <Button type="default" size="default" @click="handleCancel">{{$t('common.cancel')}}</Button>
      </Row>
    </Modal>
  </div>
</template>
<script lang="ts">
import { Component, Vue, Inject, Prop, Watch, Emit } from "vue-property-decorator";
import Util from "@/lib/util";
import AbpBase from "@/lib/abpbase";
import PageRequest from "@/store/entities/page-request";
import ProjectInfo from "./projectInfo.vue";
import ContractItem from "./contract-item.vue";
import { EContractType } from "../../../../store/entities/contract-type";
import { cloneDeep } from "lodash";
@Component({
  components: {
  },
})
export default class EditContractPopup extends AbpBase {
  @Prop() data: any
  @Prop({ type: Boolean, default: true }) show: boolean
  listInvoice: any = {}
  invoiceColumns: any = []
  EContractType = EContractType
  contractType: any = [
    {
      value: EContractType.ODC,
      label: 'ODC'
    },
    {
      value: EContractType.FixPriced,
      label: 'Fixed Price'
    },
    {
      value: EContractType.TNM,
      label: 'TNM'
    },
    {
      value: EContractType.Internal,
      label: 'Internal'
    },
  ]
  userList: any = []
  submit_loading: boolean = false
  isShow: boolean = false

  mounted() {
    this.isShow = this.show ? true : false
    this.listInvoice = {
      ...this.data,
      newPeopleInCharges: []
    }
    this.data.peopleInCharges.map(person => {
      person.isAllMonth = false
      this.listInvoice.newPeopleInCharges.push(cloneDeep(person))
    })
  }

  @Emit('onChangeValue') onChangeValue(event: any, field: string, index: number) {
    if (field === 'isAllMonth' || field === 'userId') {
      this.listInvoice.newPeopleInCharges[index][field] = event
    }
    else {
      this.listInvoice.newPeopleInCharges[index][field] = event.target.value
    }
    return this.listInvoice
  }
  created() {
    this.invoiceColumns = [
      {
        title: this.$t('project.employee'),
        slot: "userId",
        key: 'userId',
        align: 'center',
      },
      {
        title: this.$t('project.position'),
        slot: "position",
        key: 'position',
        align: 'center',
        width: '200'
      },
      {
        title: this.$t('project.rate'),
        slot: "rate",
        key: 'rate',
        align: 'center',
        width: '100'
      },
      {
        title: this.$t('project.manmonth'),
        slot: 'manMonth',
        key: 'manMonth',
        align: 'center',
        width: '200'
      },
      {
        title: this.$t('project.action'),
        slot: "action",
        key: 'invoiceStatus',
        className: 'action-cell',
        width: '200'
      }
    ]
    this.getAllUser()
  }

  async getAllUser() {
    let response = await this.$store.dispatch('invoice/getUserForInvoiceUser')
    this.userList = response.result
  }

  addMoreRow() {
    this.listInvoice.newPeopleInCharges.push({
      invoiceUserId: 0,
      manMonth: 0,
      position: "",
      rate: 0,
      userName: "",
      isAllMonth: false
    })
  }

  deleteRow(index: number) {
    this.listInvoice.newPeopleInCharges.splice(index, 1)
  }

  submitInvoice() {
    this.submit_loading = true;
    let payload = {
      invoiceId: this.listInvoice.invoiceId,
      people: this.listInvoice.newPeopleInCharges ? this.listInvoice.newPeopleInCharges : []
    }
    this.$store.dispatch({
      type: 'invoice/savePeopleInChangre',
      data: payload
    }).then(res => {
      if (res.success) {
        this.handleCancel()
      }
    }).catch((err) => {
      this.submit_loading = false;
    })
  }

  @Emit('onCancel') handleCancel() {
    return
  }

  handleClose() {
    if (!this.isShow) {
      this.$emit('onCancel', null);
    }
  }

}
</script>
<style lang="scss" scoped>
.edit-contract-popup {
  .info-row {
    margin: 10px 0;
  }
  .action-cell {
    display: flex;
    justify-content: space-around;
  }
}
.ivu-table-wrapper {
  overflow: visible;
}
</style>
<style lang="scss">
.edit-contract-popup {
  .action-cell {
    .ivu-table-cell {
      .ivu-table-cell-slot {
        width: 100%;
        display: flex;
        justify-content: space-around;
        align-items: center;
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
}
</style>