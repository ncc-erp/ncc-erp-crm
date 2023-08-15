<template>
    <div>
       <ListTasks/>
    </div>
</template>
<script lang="ts">
    import { Component, Vue, Prop } from 'vue-property-decorator';
    import Util from '@/lib/util'
    import AbpBase from '@/lib/abpbase'
    import ListTasks from './components/listTask.vue'
    @Component({
        components: {ListTasks}
    })
    export default class Task extends AbpBase{
    }
</script>
<style scoped>  
</style>
