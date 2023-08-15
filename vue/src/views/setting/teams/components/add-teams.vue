<template>
    <div>
      <Form ref="teamData" :model="teamData" :rules="ruleValidate" :label-width="100" label-position="left" class="add-invoice">
        <FormItem :label="$t('team.teamName')" prop="name">
              <Input name="add_teamName_input" v-model="teamData.name" placeholder="Enter your name"></Input>
        </FormItem>
        <Row type="flex" class="code-row-bg" justify="space-around" align="middle">
            <Col span="11" align="right">
                <Button 
                  name="add_team_save"
                  type="error"
                  size="large"
                  @click="saveTeam('teamData')"
                >
                  {{$t('common.save')}}
                </Button>
            </Col>
            <Col span="12" align="left">
                <Button
                  name="add_team_cancel" 
                  type="default"
                  size="large"
                  @click="cancel"
                >
                  {{$t('common.cancel')}}
                </Button>
            </Col>
        </Row>
      </Form>
    </div>
</template>
<script lang="ts">
    import { Component, Vue, Prop, Emit } from 'vue-property-decorator';
    import AbpBase from '@/lib/abpbase'
import { EContractType } from '@/store/entities/contract-type';
    @Component({
        name: 'AddTeams',
        components: {}
    })
    export default class AddTeams extends AbpBase{
      private single: boolean = false
      private teamData = {
        name: '',
      }
      ruleValidate:any = {}

      async saveTeam(name: string) {
        const form = await this.$refs[name] as any;     
            await form.validate(async(valid: boolean) => {
                if (valid) {
                    await this.$store.dispatch({
                      type:'team/saveTeam',
                      params: this.teamData
                    })
                    this.saved()
                } else {
                    this.$Message.error('Fail!');
                }
            })
        
      }
      saved() {
        this.$Message.info(this.$t('common.saved'));
        this.showAdd()
      }
      cancel() {
        this.$Message.info(this.$t('common.cancel'));
        this.showAdd()
      }
      @Emit('showAdd') showAdd () {
            return
        }

      created() {
        this.teamData = {
          name: '',
        }
        this.ruleValidate = {
          name: [
              { required: true, message: this.$t('customerManagement.errName'), trigger: 'blur' }
          ],
        }
      }
    }
</script>
<style lang="scss" scoped>
    .error {
        text-align: center;
        color:red;
    }
    .contact-data {
      padding-bottom: 20px;
    }
    .text-align {
      text-align:center
    }
    .permission-zone {
      display: flex;
      .user-zone {
        margin-right: 15px;
      }
    }
</style>
<style lang="scss">
</style>