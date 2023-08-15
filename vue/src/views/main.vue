<style lang="less">
@import "./main.less";
</style>
<template>
  <div class="main" :class="{'main-hide-text': shrink}">
    <div class="main-header-con" :style="{paddingLeft: shrink?'80px':'0'} ">
      <div class="main-header">
        <div slot="top" class="logo-con">
          <a>
            <img src="@/assets/logo.png" alt="NCC" />
            <!-- <h1 >{{L('CRM SYSTEM')}}</h1> -->
          </a>
        </div>
        <div class="header-avator-con">
          <full-screen v-model="isFullScreen" @on-change="fullscreenChange"></full-screen>
          <lock-screen></lock-screen>
          <notice></notice>
          <language-list></language-list>
          <div class="user-dropdown-menu-con">
            <Row type="flex" justify="end" align="middle" class="user-dropdown-innercon">
              <Dropdown transfer trigger="click" @on-click="handleClickUserDropdown">
                <a href="javascript:void(0)">
                  <span class="main-user-name">{{ userName }}</span>
                  <Icon type="arrow-down-b"></Icon>
                </a>
                <DropdownMenu slot="list">
                  <DropdownItem name="ownSpace">{{L('UserProfile')}}</DropdownItem>
                  <DropdownItem name="loginout" divided>{{L('Logout')}}</DropdownItem>
                </DropdownMenu>
              </Dropdown>
              <span class="avatar" style="background: #619fe7;margin-left: 10px;">
                <img src="../images/usericon.jpg" />
              </span>
            </Row>
          </div>
        </div>
      </div>
    </div>
    <div class="single-page-con" :style="{left: shrink?'80px':'0'}">
      <div class="sidebar-menu-con" :style="{width: shrink?'80px':'200px'}">
        <shrinkable-menu
          :shrink="shrink"
          @on-change="handleSubmenuChange"
          :before-push="beforePush"
          :open-names="openedSubmenuArr"
          :menu-list="menuList"
        ></shrinkable-menu>
      </div>
      <div class="single-page">
        <keep-alive :include="cachePage">
          <router-view></router-view>
        </keep-alive>
      </div>
      <!-- <copyfooter :copyright="L('CopyRight')"></copyfooter> -->
    </div>
  </div>
</template>
<script lang="ts">
import { Component, Vue, Inject, Prop, Watch } from 'vue-property-decorator';
import shrinkableMenu from '../components/shrinkable-menu/shrinkable-menu.vue';
import fullScreen from '../components/fullscreen.vue';
import lockScreen from '../components/lockscreen/lockscreen.vue';
import notice from '../components/notices/notice.vue';
import util from '../lib/util';
import copyfooter from '../components/Footer.vue'
import LanguageList from '../components/language-list.vue'
import AbpBase from '../lib/abpbase'
@Component({
  components: { shrinkableMenu, fullScreen, lockScreen, notice, copyfooter, LanguageList }
})
export default class Main extends AbpBase {
  shrink: boolean = false;
  get userName() {
    return this.$store.state.session.user ? this.$store.state.session.user.name : ''
  }
  isFullScreen: boolean = false;
  messageCount: string = '0';
  get openedSubmenuArr() {
    return this.$store.state.app.openedSubmenuArr
  }
  get menuList() {
    return this.$store.state.app.menuList;
  }
  get pageTagsList() {
    return this.$store.state.app.pageOpenedList as Array<any>;
  }
  get currentPath() {
    return this.$store.state.app.currentPath;
  }
  get lang() {
    return this.$store.state.app.lang;
  }
  get avatorPath() {
    return localStorage.avatorImgPath;
  }
  get cachePage() {
    return this.$store.state.app.cachePage;
  }
  get menuTheme() {
    return this.$store.state.app.menuTheme;
  }
  get mesCount() {
    return this.$store.state.app.messageCount;
  }
  init() {
    let pathArr = util.setCurrentPath(this, this.$route.name as string);
    this.$store.commit('app/updateMenulist');
    if (pathArr.length >= 2) {
      this.$store.commit('app/addOpenSubmenu', pathArr[1].name);
    }
    let messageCount = 3;
    this.messageCount = messageCount.toString();
    this.checkTag(this.$route.name);
  }
  toggleClick() {
    this.shrink = !this.shrink;
  }
  handleClickUserDropdown(name: string) {
    if (name === 'ownSpace') {
      util.openNewPage(this, 'ownspace_index', null, null);
      this.$router.push({
        name: 'ownspace_index'
      });
    } else if (name === 'loginout') {
      this.$store.commit('app/logout', this);
      util.abp.auth.clearToken();
      location.reload();
    }
  }
  checkTag(name?: string) {
    let openpageHasTag = this.pageTagsList.some((item: any) => {
      if (item.name === name) {
        return true;
      } else {
        return false
      }
    });
    if (!openpageHasTag) {
      util.openNewPage(this, name as string, this.$route.params || {}, this.$route.query || {});
    }
  }
  handleSubmenuChange(val: string) {
  }
  beforePush(name: string) {
    if (name === 'accesstest_index') {
      return false;
    } else {
      return true;
    }
  }
  fullscreenChange(isFullScreen: boolean) {
  }
  @Watch('$route')
  routeChange(to: any) {
    this.$store.commit('app/setCurrentPageName', to.name);
    let pathArr = util.setCurrentPath(this, to.name);
    if (pathArr.length > 2) {
      this.$store.commit('app/addOpenSubmenu', pathArr[1].name);
    }
    this.checkTag(to.name);
    localStorage.currentPageName = to.name;
  }
  @Watch('lang')
  langChange() {
    util.setCurrentPath(this, this.$route.name as string);
  }
  mounted() {
    this.init();
  }
  created() {
    this.$store.commit('app/setOpenedList');
  }
}
</script>

<style lang="css" scoped>
.single-page {
  position: relative;
  height: fit-content;
  width: 100%;
  padding-bottom: 20px;
}
.single-page-con {
  position: relative;
  display: flex;

  height: 100%;
}

.sidebar-menu-con {
  padding: 20px 0px 20px 20px;
  margin-right: 20px;
}
.main-header-con {
  position: relative;
  height: 65px;
  box-shadow: 0 2px 1px 1px #ba071f;
}
</style>
