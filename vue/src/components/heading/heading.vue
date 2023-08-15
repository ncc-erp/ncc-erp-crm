<template>
    <div class="pageheading">
        <Row type="flex" justify="space-between" class="code-row-bg">
            <Col :span="18"><h2> <slot name="header"></slot></h2></Col>
            <Col><slot name="button"></slot></Col>
        </Row>
    </div>
</template>
<script lang="ts">
    import { Component, Vue,Inject, Prop,Watch } from 'vue-property-decorator';
    import Util from '../../lib/util'
    import AbpBase from '../../lib/abpbase'
    import Role from '../../store/entities/role';

export default class  PageHeading extends Vue{
    
}
</script>
<style scoped>
  .pageheading {
    margin-bottom: 10px;
  }
</style>