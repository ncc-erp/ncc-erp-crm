<style lang="less">
@import "../styles/menu.less";
</style>

<template>
  <Menu
    style="height: 100%"
    ref="sideMenu"
    :active-name="$route.meta.activeMenu"
    :open-names="openNames"
    width="auto"
    @on-select="changeMenu"
  >
    <div v-for="(item, index) in menuList" :key="index">
      <div v-if="item.children.filter((child) => !child.hidden).length <= 1">
        <template v-for="(child, index) in item.children">
          <MenuItem v-if="index === 0" :name="child.name" :key="child.name">
            <i class="iconfont" v-html="child.icon"></i>
            <span>{{ child.meta.title }}</span>
          </MenuItem>
        </template>
      </div>
      <Submenu
        v-if="item.children.filter((child) => !child.hidden).length > 1"
        :name="item.name"
        :key="item.name"
      >
        <template slot="title">
          <i class="iconfont" v-html="item.icon"></i>
          <span>{{ itemTitle(item) }}</span>
        </template>
        <template v-for="child in item.children">
          <MenuItem :name="child.name" :key="child.name">
            <i class="iconfont" v-html="child.icon"></i>
            <span>{{ child.meta.title }}</span>
          </MenuItem>
        </template>
      </Submenu>
    </div>
  </Menu>
</template>

<script lang="ts">
import { Component, Vue, Inject, Prop, Emit } from "vue-property-decorator";
import AbpBase from "../../../lib/abpbase";
@Component({})
export default class SidebarMenu extends AbpBase {
  name: string = "sidebarMenu";
  @Prop({ type: Array }) menuList: Array<any>;
  @Prop({ type: Number }) iconSize: number;
  @Prop({ type: String, default: "light" }) menuTheme: string;
  @Prop({ type: Array }) openNames: Array<string>;
  itemTitle(item: any): string {
    return item.meta.title;
  }
  @Emit("on-change")
  changeMenu(active: string) {}
  updated() {
    this.$nextTick(() => {
      if (this.$refs.sideMenu) {
        (this.$refs.sideMenu as any).updateActiveName();
      }
    });
  }
}
</script>
