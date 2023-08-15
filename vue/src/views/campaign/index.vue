<template>
    <div>
       <ListCampaign/>
    </div>
</template>
<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import Util from '@/lib/util'
    import AbpBase from '@/lib/abpbase'
    import ListCampaign from './components/listCampaign.vue'
    @Component({
        components: {ListCampaign}
    })
    export default class Campaign extends AbpBase{
    }
</script>
<style scoped>
</style>
