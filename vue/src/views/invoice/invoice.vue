<template>
    <div>
       <ListInvoices/>
    </div>
</template>
<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import Util from '@/lib/util'
    import AbpBase from '@/lib/abpbase'
    import ListInvoices from './components/listInvoices.vue'
    @Component({
        components: {ListInvoices}
    })
    export default class Invoice extends AbpBase{
    }
</script>
<style scoped>  
</style>
