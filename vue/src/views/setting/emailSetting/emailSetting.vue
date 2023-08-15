<template>
    <div>
        <Card>
          <div class="page-body">
            <PageHeading>
              <template #header>{{$t('emailSetting.emailSetting')}}</template>
              <template #button><Button name="emailSetting_save" class="button btn-add" @click="saveEmailSetting('dataSubmit')">
                  {{$t("common.save")}}</Button></template>
            </PageHeading>
            <Form ref="dataSubmit" :model="dataSubmit" :rules="ruleValidate" :label-width="200" label-position="left">
                <FormItem :label="$t('emailSetting.host')"  prop="host">
                    <Input name="emailSettingHost_input" v-model="dataSubmit.host" :placeholder="$t('common.placeholderEnter',{field: $t('emailSetting.host')})" />
                </FormItem>
                <FormItem :label="$t('emailSetting.port')"  prop="port">
                    <Input name="emailSettingPort_input" v-model="dataSubmit.port" :placeholder="$t('common.placeholderEnter',{field: $t('emailSetting.port')})" />
                </FormItem>
                <FormItem :label="$t('emailSetting.fromDisplayName')"  prop="fromDisplayName">
                    <Input name="emailSettingFromDisplayName_input" v-model="dataSubmit.fromDisplayName" :placeholder="$t('common.placeholderEnter',{field: $t('emailSetting.fromDisplayName')})" />
                </FormItem>
                <FormItem :label="$t('emailSetting.userName')"  prop="userName">
                    <Input name="emailSettingUserName_input" v-model="dataSubmit.userName" :placeholder="$t('common.placeholderEnter',{field: $t('emailSetting.userName')})" />
                </FormItem>
                <FormItem :label="$t('emailSetting.password')"  prop="password">
                    <Input name="emailSettingPassword_input" v-model="dataSubmit.password" type="password" password :placeholder="$t('common.placeholderEnter',{field: $t('emailSetting.password')})" />
                </FormItem>
                <FormItem :label="$t('emailSetting.defaultFromAddress')"  prop="defaultFromAddress">
                    <Input name="emailSettingDefaultFromAddress_input" v-model="dataSubmit.defaultFromAddress" :placeholder="$t('common.placeholderEnter',{field: $t('emailSetting.defaultFromAddress')})" />
                </FormItem>
                <FormItem :label="$t('emailSetting.enableSsl')"  prop="enableSsl">
                    <Select name="emailSettingEnableSsl_input" v-model="dataSubmit.enableSsl" style="width:400px">
                        <Option v-for="item in enableSslList" :value="item.value" :key="item.value">{{ item.label }}</Option>
                    </Select>
                </FormItem>
            </Form>
          </div>
        </Card>
    </div>
</template>
<script lang="ts">
import { Component, Vue, Inject, Prop, Watch } from "vue-property-decorator";
import Util from "@/lib/util";
import AbpBase from "@/lib/abpbase";
import PageRequest from "../../../store/entities/page-request";
import PageHeading from "../../../components/heading/heading.vue";
@Component({
  name: 'TeamsManagement',
  components: {
    PageHeading,
  }
})
export default class TeamsManagement extends AbpBase {
    ruleValidate: any = {}
    dataSubmit = {} as any
    enableSslList = [
        {
            value: 'true',
            label: 'true'
        },
        {
            value: 'false',
            label: 'false'
        },
    ]

    async created() {
      this.ruleValidate = {
          host: [
              { required: true, message: this.$t('emailSetting.errNotEmpty').toString() + this.$t('emailSetting.host'), trigger: 'blur' },
          ],
          port: [
              { required: true, message: this.$t('emailSetting.errNotEmpty').toString() + this.$t('emailSetting.port'), trigger: 'blur' },
          ],
          fromDisplayName: [
              { required: true, message: this.$t('emailSetting.errNotEmpty').toString() + this.$t('emailSetting.fromDisplayName'), trigger: 'blur' },
          ],
          userName: [
              { required: true, message: this.$t('emailSetting.errNotEmpty').toString() + this.$t('emailSetting.userName'), trigger: 'blur' },
          ],
          password: [
              { required: true, message: this.$t('emailSetting.errNotEmpty').toString() + this.$t('emailSetting.password'), trigger: 'blur' },
          ],
          defaultFromAddress: [
              { required: true, message: this.$t('emailSetting.errNotEmpty').toString() + this.$t('emailSetting.defaultFromAddress'), trigger: 'blur' },
          ],
          enableSsl: [
              { required: true, message: this.$t('emailSetting.errNotEmpty').toString() + this.$t('emailSetting.enableSsl'), trigger: 'blur' },
          ],
      }
      this.getEmailSetting()
    }

    async getEmailSetting() {
        let response = await this.$store.dispatch({
        type: "email/getEmailSetting",
      });
      this.dataSubmit = response
    }

    async saveEmailSetting(name) {            
      const form = await this.$refs[name] as any;
      await form.validate(async(valid: boolean) => {
          if (valid) {
              await this.$store.dispatch({
                  type: 'email/changeEmailSetting',
                  data: this.dataSubmit
              }) 
              this.$Message.info(this.$t('common.saved'));
          } else {
              this.$Message.error('Fail!');
          }
      })
      
  }
}
</script>
<style lang="scss" scoped >
</style>