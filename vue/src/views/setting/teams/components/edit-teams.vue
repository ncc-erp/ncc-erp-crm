<template>
    <div>
      <PageHeading>
        <template #header>{{$t('contact.editContact')}}</template>
        <template #button>
          <Button
            name="team_delete"
            class="button btn-edit"
            @click="deleteTeam()"
          >{{$t('common.delete')}}</Button>
        </template>
      </PageHeading>
      <Form ref="teamData" :model="teamData" :rules="ruleValidate" :label-width="200" label-position="left" class="add-invoice">
        <FormItem :label="$t('team.teamName')" prop="name">
              <Input name="edit_teamName_input" v-model="teamData.name" placeholder="Enter your name"></Input>
        </FormItem>
        <FormItem :label="$t('team.isActive')">
              <Checkbox name="edit_teamIsActive_input" v-model="teamData.isActive" size="large"></Checkbox>
        </FormItem>
        <FormItem :label="$t('team.member')">
              <Table name="editTeam_tbl"
                :columns="columns"
                :data="listMember"
                class="select-option"
              >
              <template slot-scope="{row, index}" slot="userName">
                  <Select :name="`teamUserId${index}_input`" v-model="row.userId" filterable @on-change="editData(row)">
                      <Option v-for="item in allUser" :value="item.id" :key="item.id">{{ item.userName }}</Option>
                  </Select>
                  <p class="error-text" v-if="!row.userId && !isTeamMember">{{$t('customerManagement.errName')}} </p>
                  <p class="error-text" v-if="row.isExist && !isTeamMember">{{$t('team.errName')}} </p>
              </template>
              <template slot-scope="{row, index}" slot="permission">
                <Row type="flex" class="team-data">
                  <Col span="12">
                     <Checkbox :name="`teamSuperior${index}_input`" v-model="row.superior" size="large" @on-change="editData(row)">{{$t('team.superior')}}</Checkbox>
                  </Col>
                  <Col span="12">
                    <Checkbox :name="`teamLowerGrade${index}_input`" v-model="row.lowerGrade" size="large" @on-change="editData(row)">{{$t('team.lowerGrade')}}</Checkbox>
                  </Col>
                </Row>
                <Row type="flex" class="team-data">
                  <Col span="12">
                      <Checkbox :name="`teamSameLevel${index}_input`" v-model="row.samelevel" size="large" @on-change="editData(row)">{{$t('team.samelevel')}}</Checkbox>
                  </Col>
                  <Col span="12">
                    <Checkbox :name="`teamInternal${index}_input`" v-model="row.internal" size="large" @on-change="editData(row)">{{$t('team.internal')}}</Checkbox>
                  </Col>
                </Row>
                <p class="error-text" v-if="!row.superior && !row.lowerGrade && !row.samelevel && !row.internal && !isTeamMember">{{$t('team.errPermission')}} </p>
              </template>
              <template slot-scope="{row}" slot="role">
                <Select name="`teamMember${index}_input`" v-model="row.teamMember" @on-change="editData(row)">
                    <Option v-for="item in ETeamMember" :value="item.id" :key="item.id">{{ item.name }}</Option>
                </Select>
                <p class="error-text" v-if="!row.teamMember && !isTeamMember">{{$t('contact.errRole')}} </p>
              </template>
              <template slot-scope="{row}" slot="active">
                <i name="team_close" @click="closeTeam(row.idMember)" class="icon ivu-icon ivu-icon-md-close-circle"></i>
                <i name="team_increase" v-if="row.isAdd" @click="increaseTeam" class="icon ivu-icon ivu-icon-ios-add-circle-outline"></i>
              </template>
              </Table>
        </FormItem>
        <Row type="flex" class="code-row-bg" justify="space-around" align="middle">
            <Col span="11" align="right">
                <Button 
                  name="team_save"
                  type="error"
                  size="large"
                  @click="saveTeam('teamData')"
                >
                  {{$t('common.save')}}
                </Button>
            </Col>
            <Col span="12"  >
                <Button 
                  name="team_cancel"
                  type="default"
                  size="large"
                  @click="showAdd"
                >
                  {{$t('common.cancel')}}
                </Button>
            </Col>
        </Row>
      </Form>
    </div>
</template>
<script lang="ts">
    import { Component, Vue, Prop, Emit, Watch } from 'vue-property-decorator';
    import AbpBase from '@/lib/abpbase'
    import PageHeading from "@/components/heading/heading.vue"; 
    import { ETeamMember } from '@/store/entities/user';
    @Component({
        name: 'EditTeams',
        components: {PageHeading}
    })
    export default class EditTeams extends AbpBase{
      private teamData = {
        name: '',
        isActive: false,
        id: null,
      }
      private allUser: any = []
      private columns: any = []
      private listMember: any = []
      private idMember: number = 1
      private isTeamMember:boolean = true
      ruleValidate:any = {}
      private ETeamMember:any = ETeamMember
      @Watch("$route")
        getEvent() {            
            this.fetchData();
        }
      async saveTeam(name: string) {
        const form = await this.$refs[name] as any;
        this.isTeamMember = true
        this.teamData.id = this.$route.params.teamsId
        let param = {
          teamId: this.$route.params.teamsId,
          userTeamPermissions: []
        }
        param.userTeamPermissions = this.listMember.map(el => {
            let _el: any = {}
            _el.userId = el.userId
            _el.teamMember = el.teamMember
            _el.teamPermissionIds = []
            if (el.superior) {
              _el.teamPermissionIds.push(1)
            }
            if (el.lowerGrade) {
              _el.teamPermissionIds.push(2)
            }
            if (el.samelevel) {
              _el.teamPermissionIds.push(3)
            }
            if (el.internal) {
              _el.teamPermissionIds.push(4)
            }
            return _el
        })
        param.userTeamPermissions = param.userTeamPermissions.filter(element => element.userId !== null)
        this.listMember.forEach(element => {
          if ((!element.teamMember && element.userId) || (element.teamMember && !element.userId)
           || element.isExist || (!element.superior && !element.lowerGrade && !element.samelevel && !element.internal && element.userId)) {
            this.isTeamMember = false
          }
        });

            await form.validate(async(valid: boolean) => {
                if (valid && this.isTeamMember) {
                    await this.$store.dispatch({
                      type:'team/saveTeam',
                      params: this.teamData
                    })
                    await this.$store.dispatch({
                      type:'team/updateUserTeam',
                      createData: param
                    })
                    this.showAdd()
                } else {
                    this.$Message.error('Fail!');
                }
            })
        
      }

      private closeTeam(id: number) {
          if (this.listMember.length > 1) {
              this.listMember = this.listMember.filter(element => element.idMember !== id)
          } else this.listMember = [{
              idMember: this.idMember,
              isAdd: true,
              superior: false,
              lowerGrade: false,
              samelevel: false,
              internal: false,
              userId: null,
              teamMember: null
          }]
          this.listMember[this.listMember.length - 1].isAdd = true
      }

      private editData(row: any) {
        row.isExist = false
        this.listMember.forEach(element => {
          if (element.userId === row.userId && element.idMember !== row.idMember) {
            row.isExist = true
          }
        });
        this.listMember = this.listMember.map(element => {
          let _element = element
          if (row.idMember === element.idMember) {
            _element = row
          }
          return _element
        });
          this.listMember = [...this.listMember]
        
        this.listMember  = this.listMember.map(el => {
          let _el = el
          const member = this.listMember.filter(element => el.userId === element.userId)
          if (member.length < 2) {
            _el.isExist = false
          }
          return _el
        })
      }

      async deleteTeam() {
          if(this.$route.params.teamsId) {
              await this.$store.dispatch({
              type: "team/delete",
              id: this.$route.params.teamsId
              });
          this.$Message.info(this.$t('common.deleted'));
          this.showAdd()
          }
      }
      showAdd () {
        this.$router.push({ name: 'teams'})
      }

      beforeMount() {
        this.ruleValidate = {
          name: [
              { required: true, message: this.$t('customerManagement.errName'), trigger: 'blur' }
          ],
        }
        this.fetchData()
      }

      async fetchData() {
        this.getpage()
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
          let userTeam = await this.$store.dispatch({
            type:'team/getUserTeam',
            teamId: this.$route.params.teamsId
          })
          if (userTeam.length >= 1) {
            this.listMember = userTeam[0].getUserTeamPermissionDtos
            this.listMember = this.listMember.map(el => {
              let _el = el
              _el.idMember = this.idMember
              _el.isAdd = false
              this.idMember++
              _el.superior = false
              _el.lowerGrade = false
              _el.samelevel = false
              _el.internal = false
              el.getTeamPermissionDtos.forEach(element => {
                if (element.teamPermissionId === 1) {
                  _el.superior = true
                }
                if (element.teamPermissionId === 2) {
                  _el.lowerGrade = true
                }
                if (element.teamPermissionId === 3) {
                  _el.samelevel = true
                }
                if (element.teamPermissionId === 4) {
                  _el.internal = true
                }
              });
              return _el
            })
            this.listMember[this.listMember.length - 1].isAdd = true
          }
          if (this.listMember.length < 1) {
            this.increaseTeam()
          }
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
        {
          title: this.$t('team.active'),
          slot: 'active'
        },
        ]
      }
      private increaseTeam() {
        this.idMember++
         this.listMember = this.listMember.map(element => {
          let _element = element
            _element.isAdd = false
          return _element
        });
        this.listMember.push({
          idMember: this.idMember,
          superior: false,
          lowerGrade: false,
          samelevel: false,
          internal: false,
          userId: null,
          teamMember: null
        })
        this.listMember[this.listMember.length - 1].isAdd = true
      }
    }
</script>
<style lang="scss" scoped>
    .select-option {
      padding-bottom: 200px;
    }
    .icon {
        font-size: 32px;
        margin-right: 15px;
    }
    .error-text {
        color: red;
    }
</style>
<style lang="scss">
</style>