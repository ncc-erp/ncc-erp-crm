<template>
  <div class="list-item" @dragover="(event) => allowDrop(event)" @drop="(event) => drop(event)">
    <h3>{{list.name}}</h3>
    <Divider />
    <div class="list-item-content">
      <div v-for="(item, index) in list.list" :key="index">
        <div class="draggable-item" draggable="true" @dragstart="(event) => drag(event, item.id)">
          <p>
            <span>{{$t('common.name')}}:</span>
            <span>{{item.name}}</span>
          </p>
          <p>
            <span>{{$t('common.email')}}:</span>
            <span>{{item.mail}}</span>
          </p>
          <p>
            <span>{{$t('common.type')}}:</span>
            <span>{{item.type}}</span>
          </p>
        </div>
      </div>
    </div>
  </div>
</template>
<script lang="ts">
import { Component, Vue, Prop, Emit } from 'vue-property-decorator';
@Component({
  components: {},
  props: {}
})
export default class ListItem extends Vue {
  @Prop() list: any

  allowDrop(event: any) {
    event.preventDefault();
  }

  async drop(event: any) {
    event.preventDefault();
    var data = event.dataTransfer.getData("draggable-item");
    await this.$store.dispatch({
      type: 'customer/changeStatusClient',
      data: {
        id: data,
        status: this.list.status
      }
    })
    this.onRefresh()
  }

  drag(event: any, itemId: number) {
    event.dataTransfer.setData("draggable-item", itemId);
  }

  @Emit('onRefresh') onRefresh() {
    return
  }
}
</script>
<style lang="scss" scoped>
.list-item {
  padding: 10px;
  background-color: #f0f2f5;
  width: 300px;
  height: 600px;
  margin: 0 10px;
  .draggable-item {
    background-color: #fff;
    padding: 5px;
    margin: 5px 0;
    cursor: pointer;
  }
  .ivu-divider {
    margin: 10px 0;
  }
  .list-item-content {
    height: 520px;
    overflow-y: auto;
  }
}
</style>


