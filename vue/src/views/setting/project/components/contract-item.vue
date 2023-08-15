<template>
  <div class="contract-item">
    <div class="contract-item-header">
      <strong>{{data.contractName}}</strong>
      <div>
        <Button type="error" size="small" ghost @click="onEditContract">{{$t('common.edit')}}</Button>
        <!-- <Button
          type="error"
          size="small"
          ghost
          @click="openConfirmDeletePopup"
        >{{$t('common.delete')}}</Button> -->
      </div>
    </div>
    <Row class="info-row">
      <Col span="4" class="field-cols">
        <span>{{$t('project.status')}}</span>
      </Col>
      <Col span="20" class="value-cols">
        <span>{{converContractStatus(data.contractStatus)}}</span>
      </Col>
    </Row>
    <Row class="info-row">
      <Col span="4" class="field-cols">
        <span>{{$t('project.value')}}</span>
      </Col>
      <Col span="20" class="value-cols">
        <span>{{data.contractValue}} {{ data.contractCurrency | convertedCurrency}}</span>
      </Col>
    </Row>
    <Row class="info-row">
      <Col span="4" class="field-cols">
        <span>{{$t('project.type')}}</span>
      </Col>
      <Col span="20" class="value-cols">
        <span>{{convertContractType(data.contractType)}}</span>
      </Col>
    </Row>
    <Row class="info-row">
      <Col span="4" class="field-cols">
        <span v-if="data.contractType === 1">{{$t('project.mileStone')}}</span>
        <span v-if="data.contractType === 0 || data.contractType === 2">{{$t('project.invoice')}}</span>
      </Col>
      <Col span="20" class="value-cols">
        <Table name="contract_tbl"
          v-if="data.contractType === 1"
          :columns="milestoneColumns"
          :data="data.contractMileStones"
          border
          width="800"
        >
          <template slot-scope="{ row }" slot="mileStoneDate">
            <span>{{row.mileStoneDate | convertedDate}}</span>
          </template>
          <template slot-scope="{ row }" slot="invoiceStatus">
            <span>{{convertInvoiceStatus(row.invoiceStatus)}}</span>
          </template>
        </Table>
        <Table name="contract2-tbl"
          v-if="data.contractType === 0 || data.contractType === 2"
          :columns="invoiceColumns"
          :data="data.contractInVoices"
          border
          width="800"
        >
          <template slot-scope="{ row }" slot="time">
            <span>{{row.time | convertedDate}}</span>
          </template>
          <template slot-scope="{ row }" slot="peopleInCharges">
            <div v-for="(person, index) in row.peopleInCharges" :key="index">
              <span>{{person.userName}} - {{person.position}} - {{person.rate}}</span>
            </div>
          </template>
          <template slot-scope="{ row }" slot="invoiceStatus">
            <span>{{convertInvoiceStatus(row.invoiceStatus)}}</span>
          </template>
          <template slot-scope="{ row, index }" slot="action">
            <Button type="error" size="small" @click="onEditInvoice(row)">{{$t('common.edit')}}</Button>
          </template>
        </Table>
      </Col>
    </Row>
    <Modal v-model="isShowConfirmDelete" :title="$t('common.notification')">
      <p>{{$t('project.confirmDeleteNotice')}}</p>
      <Row slot="footer" class="button-zone">
        <Button
          type="error"
          size="default"
          :loading="submit_loading"
          @click="deleteContract"
        >{{$t('common.accept')}}</Button>
        <Button
          type="error"
          size="default"
          ghost
          @click="isShowConfirmDelete=false"
        >{{$t('common.cancel')}}</Button>
      </Row>
    </Modal>
  </div>
</template>
<script lang="ts">
import { Component, Vue, Inject, Prop, Watch, Emit } from "vue-property-decorator";
import Util from "@/lib/util";
import AbpBase from "@/lib/abpbase";
import PageRequest from "@/store/entities/page-request";
import moment from "moment";
import { EInvoiceStatus, EContractStatus } from '../../../../store/entities/status';
import { EContractType } from '../../../../store/entities/contract-type';
import { ECurrency } from '../../../../store/entities/currency';
@Component({
  filters: {
    convertedDate(value: string) {
      return moment(value).format('DD/MM')
    },
    convertedCurrency(value: number) {
      switch (value) {
        case ECurrency.USD:
          return 'USD'
        case ECurrency.VND:
          return 'VND'
        case ECurrency.EUR:
          return 'EUR'
        case ECurrency.JPY:
          return 'JPY'
        default:
          return 'VND'
      }
    }
  },
  components: {}
})
export default class ContractItem extends AbpBase {
  @Prop() data: any
  milestoneColumns: any = []
  invoiceColumns: any = []
  isShowConfirmDelete: boolean = false
  submit_loading: boolean = false

  async created() {
    this.milestoneColumns = [
      {
        title: this.$t('project.mileStone'),
        key: 'mileStone',
        align: 'center',
      },
      {
        title: this.$t('project.description'),
        key: 'description',
        align: 'center',
        ellipsis: true,
        width: 300
      },
      {
        title: this.$t('project.time'),
        slot: "mileStoneDate",
        key: 'mileStoneDate',
        align: 'center',
      },
      {
        title: this.$t('project.status'),
        slot: 'invoiceStatus',
        key: 'invoiceStatus',
        align: 'center',
      }
    ]
    this.invoiceColumns = [
      {
        title: this.$t('project.time'),
        slot: "time",
        key: 'time',
        align: 'center',
      },
      {
        title: this.$t('project.peopleInCharges'),
        slot: "peopleInCharges",
        key: 'peopleInCharges',
        align: 'center',
      },
      {
        title: this.$t('project.status'),
        slot: 'invoiceStatus',
        key: 'invoiceStatus',
        align: 'center',
      },
      {
        title: this.$t('project.action'),
        key: 'action',
        slot: "action",
        align: 'center',
      }
    ]
  }

  @Emit('onEditContract') onEditContract() {
    return this.data
  }

  @Emit('onEditInvoice') onEditInvoice(invoice: any) {
    return invoice
  }

  @Emit('onRefresh') onRefresh() {
    return
  }

  openConfirmDeletePopup() {
    this.isShowConfirmDelete = true
  }

  async deleteContract() {
    this.submit_loading = true
    await this.$store.dispatch({
      type: 'contract/deleteContract',
      contractId: this.data.contractId
    }).then((res) => {
      this.submit_loading = false
      if (res.success) {
        this.onRefresh()
        this.isShowConfirmDelete = false
      }
    }).catch((e) => {
      this.submit_loading = false
    })
  }

  convertInvoiceStatus(value: number) {
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

  converContractStatus(value: number) {
    switch (value) {
      case EContractStatus.Pending:
        return this.$t('common.pending');
      case EContractStatus.OnGoing:
        return this.$t('common.onGoing');
      case EContractStatus.Finish:
        return this.$t('common.finish');
    }
  }

  convertContractType(value: number) {
    switch (value) {
      case EContractType.ODC:
        return this.$t('common.odc');
      case EContractType.FixPriced:
        return this.$t('common.fixedPrice');
      case EContractType.TNM:
        return this.$t('common.tnm');
      case EContractType.Internal:
        return this.$t('common.internal');
    }
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