<template>
    <div>
      <PageHeading>
        <template #header>{{$t('team.teamDetails')}}</template>
      </PageHeading>
      <div>
        <Row type="flex" class="team-data">
          <Col span="4">
              {{$t('team.teamName')}} :
          </Col>
          <Col span="20">
            <p>{{teamData.name}}</p>
          </Col>
        </Row>
        <Row type="flex" class="team-data">
          <Col span="4">
              {{$t('team.isActive')}} :
          </Col>
          <Col span="20">
           <Checkbox v-model="teamData.isActive" size="large" disabled></Checkbox>
          </Col>
        </Row>
        <Row type="flex" class="team-data">
          <Col span="4">
              {{$t('team.member')}} :
          </Col>
          <Col span="20">
           <Table name="detailTeam_tbl"
                :columns="columns"
                :data="listMember"
              >
              <template slot-scope="{row}" slot="userName">
                  <p>{{row.userName}}</p>
              </template>
              <template slot-scope="{row}" slot="permission">
                <div v-for="item in row.getTeamPermissionDtos" :key="item.id">
                  <p>{{ item.permissionName }}</p>
                </div>
              </template>
              <template slot-scope="{row}" slot="role">
                <div v-for="item in ETeamMember" :key="item.id">
                  <p v-if="item.id === row.teamMember">
                  {{ item.name }}</p>
                </div>
              </template>
              </Table>
          </Col>
        </Row>
      </div>
      <Row type="flex" class="code-row-bg" justify="space-around" align="middle">
          <Col span="24" align="center">
              <Button type="default"
              size="large"
              @click="showAdd"
              >{{$t('common.cancel')}}
              </Button>
          </Col>
      </Row>
    </div>
</template>
<script lang="ts">
    import { Component, Vue, Prop, Emit, Watch } from 'vue-property-decorator';
    import AbpBase from '@/lib/abpbase'
    import { EContractType } from '@/store/entities/contract-type';
    import { EInvoiceStatus, EListStatusCustomer } from '../../../../store/entities/status';
    import PageHeading from "@/components/heading/heading.vue";
    import { ETeamMember } from '@/store/entities/user';
    @Component({
        name: 'DetailTeams',
        components: {PageHeading}
    })
    export default class DetailTeams extends AbpBase{
      private teamData = {
        name: '',
        isActive: false,
      }
      private clientData: any = []
      private allUser: any = []
      private columns: any = []
      private listMember: any = []
      private ETeamMember:any = ETeamMember

      @Watch("$route")
      getEvent() {            
          this.fetchData();
      }

      showAdd () {
        this.$router.push({ name: 'teams'})
      }
      
      beforeMount() {
        this.fetchData()
      }
      
      async fetchData() {
        if (this.$route.params.teamsId) {
         let data = await this.$store.dispatch({
            type:'team/getAllPagging',
          })
          data.forEach(element => {
            if (element.id == this.$route.params.teamsId) {
              this.teamData = element
              return
            }
          });
        }
        let userTeam = await this.$store.dispatch({
            type:'team/getUserTeam',
            teamId: this.$route.params.teamsId
          })
          if (userTeam.length >= 1) {
            this.listMember = userTeam[0].getUserTeamPermissionDtos
          }
      }
      async getpage() {
        let pagerequest = {
          skipCount: 0,
          maxResultCount: 1000000,
        }
        const data = await this.$store.dispatch({
          type: 'user/getAll',
          data: pagerequest
        })
        this.allUser = data.items
      }

      created() {
        this.columns = [{
          title: this.L('UserName'),
          slot: 'userName'
        }, {
          title: this.$t('team.permission'),
          slot: 'permission'
        },
        {
          title: this.$t('contact.role'),
          slot: 'role'
        },
        ]
      }
    }
</script>
<style lang="scss" scoped>
    .error {
        text-align: center;
        color:red;
    }
    .team-data {
      padding-bottom: 20px;
    }
</style>
<style lang="scss">
</style>