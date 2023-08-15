<template>
  <div>
    <Modal
      v-model="isShow"
      :width="listContract.contractType === EContractType.FixPriced? '80' : '30'"
      class="edit-contract-popup"
      @on-visible-change="handleClose"
    >
      <h2>{{listContract.contractId ? $t('project.editContract') : $t('project.addContract')}}</h2>
      <Row class="info-row">
        <Col span="4" class="field-cols">
          <span>{{$t('project.name')}}</span>
        </Col>
        <Col span="20" class="value-cols">
          <Input
            name="inputContractName"
            v-model="listContract.contractName"
            style="width: 200px"
            :class="{'error-field' : hasErrorContractName && isClickedSubmit}"
          />
        </Col>
      </Row>
      <Row class="info-row">
        <Col span="4" class="field-cols">
          <span>{{$t('project.status')}}</span>
        </Col>
        <Col span="20" class="value-cols">
          <Select
            name="inputContractStatus"
            v-model="listContract.contractStatus"
            style="width:200px"
            :disabled="!listContract.contractId"
            :class="{'error-field' : hasErrorContractStatus && isClickedSubmit}"
          >
            <Option
              v-for="item in contractStatus"
              :value="item.value"
              :key="item.value"
            >{{ item.label }}</Option>
          </Select>
        </Col>
      </Row>
      <Row class="info-row">
        <Col span="4" class="field-cols">
          <span>{{$t('project.value')}}</span>
        </Col>
        <Col span="20" class="value-cols">
          <Input
            name="inputContractValue"
            v-model="listContract.contractValue"
            type="number"
            :number="true"
            style="width: 200px"
            :class="{'error-field' : hasErrorContractValue && isClickedSubmit}"
          />
        </Col>
      </Row>
      <Row class="info-row">
        <Col span="4" class="field-cols">
          <span>{{$t('project.currency')}}</span>
        </Col>
        <Col span="20" class="value-cols">
          <Select
            name="inputContractCurrency"
            v-model="listContract.contractCurrency"
            style="width:200px"
            :class="{'error-field' : hasErrorContractCurrency && isClickedSubmit}"
          >
            <Option
              v-for="item in currencyList"
              :value="item.value"
              :key="item.value"
            >{{ item.label }}</Option>
          </Select>
        </Col>
      </Row>
      <Row class="info-row">
        <Col span="4" class="field-cols">
          <span>{{$t('project.type')}}</span>
        </Col>
        <Col span="20" class="value-cols">
          <Select
            name="inputContractType"
            v-model="listContract.contractType"
            style="width:200px"
            :class="{'error-field' : hasErrorContractType && isClickedSubmit}"
          >
            <Option
              v-for="item in contractType"
              :value="item.value"
              :key="item.value"
            >{{ item.label }}</Option>
          </Select>
        </Col>
      </Row>

      <Row class="info-row">
        <Col span="4" class="field-cols">
          <span>{{$t('project.startTime')}}</span>
        </Col>
        <Col span="20" class="value-cols">
          <DatePicker
            name="inputStartTime"
            v-model="listContract.startTime"
            type="date"
            placeholder="Select date"
            style="width: 200px"
            :class="{'error-field' : hasErrorContractStartTime && isClickedSubmit}"
          ></DatePicker>
        </Col>
      </Row>
      <Row class="info-row">
        <Col span="4" class="field-cols">
          <span>{{$t('project.endTime')}}</span>
        </Col>
        <Col span="20" class="value-cols">
          <DatePicker
            name="inputEndTime"
            v-model="listContract.endTime"
            type="date"
            placeholder="Select date"
            style="width: 200px"
            :class="{'error-field' : hasErrorContractEndTime && isClickedSubmit}"
          ></DatePicker>
        </Col>
      </Row>
      <Row class="code-row-bg" align="middle" v-if="listContract.contractId">
        <Col span="4" class="field-cols">
          <span>{{$t('project.attachment')}}</span>
        </Col>
        <Col span="20" class="value-cols">
          <UploadFile :entity="entity" :entityId="listContract.contractId" @handleInputFile="upload"></UploadFile>
          <Table name="editcontract_tbl" border :columns="fileColumns" :data="dataFileTable" :no-data-text="$t('common.nodatas')" style="width: 100%">
              <template slot-scope="{ row }" slot="fileName">            
                <a @click="openFile(row.fileUrl)">{{row.fileUrl}}</a>             
              </template>             
              <template slot-scope="{ row }" slot="action">
                <Button
                  class="btn-edit"
                  style="margin-right: 5px"
                  @click="deleteFile(row.id)"
                >{{$t('common.delete')}}</Button>
              </template>
          </Table>        
        </Col>
    </Row>

      <Row class="info-row" v-if="listContract.contractType === EContractType.FixPriced">
        <Col span="4" class="field-cols">
          <span v-if="listContract.contractType === 1">{{$t('project.mileStone')}}</span>
          <span
            v-if="listContract.contractType === 0 || listContract.contractType === 2"
          >{{$t('project.invoice')}}</span>
        </Col>
        <Col span="20" class="value-cols">
          <Table name="editContract2_tbl"
            :columns="milestoneColumns"
            :data="listContract.newContractMileStones"
            border
            width="auto"
          >
            <template slot-scope="{ row, index }" slot="mileStone">
              <Input
                :value="row.mileStone"
                @on-change="(event) => onChangeValue(event, 'mileStone', index)"
                :class="{'error-field' : !row.mileStone && isClickedSubmit}"
              />
            </template>
            <template slot-scope="{ row, index }" slot="description">
              <Input
                :value="row.description"
                @on-change="(event) => onChangeValue(event, 'description', index)"
                :class="{'error-field' : !row.description && isClickedSubmit}"
              />
            </template>
            <template slot-scope="{ row, index }" slot="percentage">
              <Input
                :value="row.percentage"
                type="number"
                :number="true"
                @on-change="(event) => onChangeValue(event, 'percentage', index)"
                :class="{'error-field' : (!row.percentage || hasErrorTotalPercentageOfMilestone) && isClickedSubmit}"
              />
            </template>
            <template slot-scope="{ row, index }" slot="mileStoneValue">
              <Input
                :value="row.mileStoneValue"
                type="number"
                :number="true"
                @on-change="(event) => onChangeValue(event, 'mileStoneValue', index)"
                :class="{'error-field' : (!row.mileStoneValue || row.mileStoneValue <= 0) && isClickedSubmit}"
              />
            </template>
            <template slot-scope="{ row, index }" slot="invoiceStatus">
              <span>{{ status(row.invoiceStatus) }}</span>
            </template>

            <template slot-scope="{ row, index }" slot="action">
              <Button type="error" size="default" @click="deleteRow(index)">X</Button>
            </template>
          </Table>
        </Col>
      </Row>
      <Row v-if="listContract.contractType === EContractType.FixPriced">
        <Button type="primary" size="default" style="float: right" @click="addMoreRow">+</Button>
      </Row>
      <Row slot="footer" class="button-zone">
        <Button
          type="error"
          size="default"
          :loading="submit_loading"
          @click="submitContract"
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
import { cloneDeep, clone } from "lodash";
import { EContractStatus } from '../../../../store/entities/status';
import { ECurrency } from '../../../../store/entities/currency';
import UploadFile from '../../../../components/upload-file/upload-file.vue'
import { EntityDefault } from "@/store/entities/entity";
import appconst from '../../../../lib/appconst'

@Component({
  components: {UploadFile},
  filters: {}
})
export default class EditContractPopup extends AbpBase {
  @Prop() data: any
  @Prop({ type: Boolean, default: true }) show: boolean
  milestoneColumns: any = []
  EContractType = EContractType
  EContractStatus = EContractStatus
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
  contractStatus: any = [
    {
      value: EContractStatus.Pending,
      label: 'Pending'
    },
    {
      value: EContractStatus.OnGoing,
      label: 'On Going'
    },
    {
      value: EContractStatus.Finish,
      label: 'Finish'
    },
  ]
  currencyList: any = [
    {
      value: ECurrency.USD,
      label: 'USD'
    },
    {
      value: ECurrency.VND,
      label: 'VND'
    },
    {
      value: ECurrency.EUR,
      label: 'EUR'
    },
    {
      value: ECurrency.JPY,
      label: 'JPY'
    },
  ]
  listContract: any = {}
  submit_loading: boolean = false
  isShow: boolean = false
  isClickedSubmit: boolean = false
  file: any
  fileColumns: any = []
  dataFileTable : any = []
  entity = EntityDefault.Contract

  created() {
    this.milestoneColumns = [
      {
        title: this.$t('project.mileStone'),
        slot: 'mileStone',
        key: 'mileStone',
        align: 'center'
      },
      {
        title: this.$t('project.description'),
        key: 'description',
        slot: 'description',
        align: 'center',
        ellipsis: true,
        width: 300
      },
      {
        title: this.$t('project.percentage'),
        slot: 'percentage',
        key: 'percentage',
        align: 'center',
      },
      {
        title: this.$t('project.value'),
        slot: 'mileStoneValue',
        key: 'mileStoneValue',
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
        slot: "action",
        key: 'action',
        className: 'action-cell',
        width: '200'
      }
    ]
    this.fileColumns = [
      {
        title:this.$t('invoice.attachment').toString(),      
        align: 'center',
        children: [
          {
            title: this.$t('common.name').toString(),
            align: 'center',
            slot: 'fileName',
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

  async mounted() {
    this.isShow = this.show ? true : false
    this.listContract = {
      ...this.data,
      newContractMileStones: []
    }
    if (this.listContract.contractType === 1) {
      this.data.contractMileStones.map(milestone => {
        this.listContract.newContractMileStones.push(cloneDeep(milestone))
      })
    }
    if(this.listContract.contractId){     
      await this.getFiles(this.listContract.contractId); 
    }
  }

  onChangeValue(event: any, field: string, index: number) {
    this.listContract.newContractMileStones[index][field] = event.target.value
    return this.listContract
  }

  @Emit('onCancel') handleCancel() {
    return
  }

  deleteRow(index: number) {
    this.listContract.newContractMileStones.splice(index, 1)
  }

  addMoreRow() {
    this.listContract.newContractMileStones.push({
      mileStoneDate: new Date(),
      description: '',
      percentage: null,
      mileStoneValue: null,
      invoiceStatus: 0,
      mileStone: '',
      mileStoneId: 0,
    })
  }

  get hasErrorContractName() {
    return !this.listContract.contractName
  }
  get hasErrorContractStatus() {
    return !this.listContract.contractStatus && this.listContract.contractStatus !== 0
  }
  get hasErrorContractValue() {
    return !this.listContract.contractValue || this.listContract.contractValue <= 0
  }
  get hasErrorContractCurrency() {
    return !this.listContract.contractCurrency && this.listContract.contractCurrency !== 0
  }
  get hasErrorContractType() {
    return !this.listContract.contractType && this.listContract.contractType !== 0
  }
  get hasErrorContractStartTime() {
    return !this.listContract.startTime
  }
  get hasErrorContractEndTime() {
    return !this.listContract.endTime
  }
  get hasErrorTotalPercentageOfMilestone() {
    if (!this.listContract.newContractMileStones.length) {
      return false
    }
    let total = 0
    this.listContract.newContractMileStones.map(milestone => {
      total = total + Number(milestone.percentage)
    })
    return total !== 100
  }

  async submitContract() {
    this.isClickedSubmit = true
    let hasErrorValidate: boolean = this.hasErrorContractName || this.hasErrorContractStatus || this.hasErrorContractValue || this.hasErrorContractCurrency || this.hasErrorContractType || this.hasErrorContractStartTime || this.hasErrorContractEndTime

    if (!hasErrorValidate) {
      let newContractMileStones = []
      let hasErrorMilestone: boolean = false
      if (this.listContract.newContractMileStones && this.listContract.newContractMileStones.length) {
        this.listContract.newContractMileStones.map(milestone => {
          if (milestone.mileStone && milestone.description && milestone.percentage && milestone.mileStoneValue && milestone.mileStoneValue >= 0) {
            newContractMileStones.push({
              name: milestone.mileStone,
              mileStoneDate: milestone.mileStoneDate,
              description: milestone.description,
              percentage: milestone.percentage,
              mileStoneValue: milestone.mileStoneValue,
              id: milestone.mileStoneId,
            })
          }
          else hasErrorMilestone = true
        })
      }

      if (!hasErrorMilestone && !this.hasErrorTotalPercentageOfMilestone) {
        this.submit_loading = true;
        let payload = {
          projectId: this.listContract.projectId,
          name: this.listContract.contractName,
          startTime: `${this.listContract.startTime.getFullYear()}-${this.listContract.startTime.getMonth()+1}-${this.listContract.startTime.getDate()}`,
          endTime: `${this.listContract.endTime.getFullYear()}-${this.listContract.endTime.getMonth()+1}-${this.listContract.endTime.getDate()}`,
          type: this.listContract.contractType,
          status: this.listContract.contractStatus,
          contractValue: this.listContract.contractValue,
          mileStones: newContractMileStones,
          contractCurrency: this.listContract.contractCurrency,
          id: this.listContract.contractId,
          file: this.file
        }
        await this.$store.dispatch({
          type: 'contract/saveContract',
          data: payload
        }).then(res => {
          if (res.success) {
            this.isClickedSubmit = false
            this.handleCancel()
          }
        }).catch((err) => {
          this.submit_loading = false;
        })
      }
    }
  }

  status(value: number) {
    switch (value) {
      case EContractStatus.Pending:
        return this.$t('common.pending');
      case EContractStatus.OnGoing:
        return this.$t('common.onGoing');
      case EContractStatus.Finish:
        return this.$t('common.finish');
    }
  }

  handleClose() {
    if (!this.isShow) {
      this.$emit('onCancel', null);
    }
  }

  async getFiles(contractId){
     let data = await this.$store.dispatch({
            type: 'contract/getFileByContrtract',
            contractId: contractId
        })  
      this.dataFileTable = data.result
  }

  async upload(e) {
      if(e){
          await this.getFiles(this.listContract.contractId);
      }
  }
  async deleteFile(contractFileId) {
    
      let data = await this.$store.dispatch({
          type: 'contract/deleteContractFile',
          contractFileId: contractFileId
      })
      await this.getFiles(this.listContract.contractId);
  }
  openFile(url){
    window.open(appconst.remoteServiceBaseUrl + url)
  }
}
</script>

<style lang="scss" scoped>
.edit-contract-popup {
  .info-row {
    margin: 10px 0;
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

<style lang="scss">
.edit-contract-popup {
  .error-field {
    .ivu-select-selection,
    input {
      border: 1px solid red;
    }
  }
}
</style>