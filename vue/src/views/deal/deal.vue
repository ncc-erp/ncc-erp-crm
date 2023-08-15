<template>
    <div>
       <ListDeals/>
    </div>
</template>
<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import Util from '@/lib/util'
    import AbpBase from '@/lib/abpbase'
    import ListDeals from './components/listDeals.vue'
    @Component({
        components: {ListDeals}
    })
    export default class Deal extends AbpBase{
    }
</script>
<style scoped>  
</style>
