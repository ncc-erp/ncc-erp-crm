<template>
  <div class="draggable-view-list">
    <div class="draggable-row">
      <div v-for="(listItem, index) in data" :key="index">
        <ListItem :list="listItem" @onRefresh="onRefresh" />
      </div>
    </div>
  </div>
</template>
<script lang="ts">
import { Component, Vue, Prop, Emit } from 'vue-property-decorator';
import ListItem from './list-item.vue';
@Component({
  components: {
    ListItem
  },
  props: {}
})
export default class DraggableViewList extends Vue {
  @Prop() data: any
  @Emit('onRefresh') onRefresh() {
    return
  }
}
</script>
<style lang="scss" scoped>
.draggable-view-list {
  .draggable-row {
    display: flex;
    justify-content: space-between;
  }
}
</style>


