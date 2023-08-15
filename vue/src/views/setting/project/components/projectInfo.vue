<template>
  <div>
    <div class="header-title">
      <h1>{{$t('project.project')}}: {{data.projectName}}</h1>
      <Divider />
      <Row>
        <Col span="4">
          <span>{{$t('project.name')}}</span>
        </Col>
        <Col span="20">
          <span>{{data.projectName}}</span>
        </Col>
      </Row>
      <Row>
        <Col span="4">
          <span>{{$t('project.projectCode')}}</span>
        </Col>
        <Col span="20">
          <span>{{data.projectCode}}</span>
        </Col>
      </Row>
      <Row>
        <Col span="4">
          <span>{{$t('project.projectType')}}</span>
        </Col>
        <Col span="20">
          <span>{{convertType(data.projectType)}}</span>
        </Col>
      </Row>
      <Row>
        <Col span="4">
          <span>{{$t('project.assignee')}}</span>
        </Col>
        <Col span="20">
          <span v-for="(assignee, index) in data.userName" :key="index">{{index > 0 ? " - " : ""}}{{assignee}}</span>
        </Col>
      </Row>
      <Row>
        <Col span="4">
          <span>{{$t('project.description')}}</span>
        </Col>
        <Col span="20">
          <span>{{data.description}}</span>
        </Col>
      </Row>
      <Row>
        <Col span="4">
          <span>{{$t('project.client')}}</span>
        </Col>
        <Col span="20">
          <a @click="redirectToCustomerDetail">{{data.client}}</a>
        </Col>
      </Row>
      <Row>
        <Col span="4">
          <span>{{$t('project.status')}}</span>
        </Col>
        <Col span="20">
          <span>{{status(data.projectStatus)}}</span>
        </Col>
      </Row>
    </div>
  </div>
</template>
<script lang="ts">
import { Component, Vue, Inject, Prop, Watch } from "vue-property-decorator";
import Util from "@/lib/util";
import AbpBase from "@/lib/abpbase";
import PageRequest from "@/store/entities/page-request";
import { EProjectStatus } from '../../../../store/entities/status';
import { EProjectType } from '../../../../store/entities/project-type';


@Component({
  components: {},
})
export default class ProjectInfo extends AbpBase {
  @Prop() data: any
  private EProjectStatus = EProjectStatus
  private EProjectType = EProjectType
  async created() {
  }
  
  status(value: number) {
    switch (value) {
      case EProjectStatus.Pending:
        return this.$t('common.pending');
      case EProjectStatus.OnGoing:
        return this.$t('common.onGoing');
      case EProjectStatus.Finish:
        return this.$t('common.finish');
    }
  }
  
  convertType(value: number){
    switch (value) {
      case EProjectType.ODC:
        return this.$t('common.odc');
      case EProjectType.FixPriced:
        return this.$t('common.fixedPrice');
      case EProjectType.TNM:
        return this.$t('common.tnm');
      case EProjectType.Internal:
        return this.$t('common.internal');
    }
  }

  redirectToCustomerDetail() {
    this.$router.push({ name: 'detailCustomer', params: {customerId: this.data.clientId}});
    event.preventDefault();
  }
}
</script>
<style lang="scss" scoped>
.header-title {
  h1 {
    font-size: 16px;
  }
}
</style>