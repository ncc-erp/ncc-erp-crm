<template>
    <div>
      <Form ref="contactData" :model="contactData" :rules="ruleValidate" :label-width="200" label-position="left" class="add-invoice">
        <FormItem :label="$t('contact.name')" prop="name" class="mb-1">
          <Input name="contactName_input" v-model="contactData.name" placeholder="Enter your name"></Input>
        </FormItem>
        <FormItem :label="$t('contact.email')" prop="mail" class="mb-1">
          <Input name="contactMail_input" v-model="contactData.mail" placeholder="Enter your email"></Input>
        </FormItem>
        <FormItem :label="$t('contact.phone')" prop="phone" class="mb-1">
          <Input name="contactPhone_input" v-model="contactData.phone" placeholder="Enter your phone"></Input>
        </FormItem>
        <FormItem :label="$t('contact.role')" prop="role" class="mb-1">
          <Input name="contactRole_input" v-model="contactData.role" placeholder="Enter your role"></Input>
        </FormItem>
        <FormItem :label="$t('contact.description')" prop="description">
          <Input name="contactDescription_input" v-model="contactData.description" type="textarea" :rows="4" placeholder="Enter something..." />
        </FormItem>
        <Row type="flex" class="code-row-bg" justify="space-around" align="middle">
            <Col span="11" align="right">
              <Button 
                name="contact_save"
                class="button btn-add" 
                @click="saveContact('contactData')"
                >
                {{$t('common.save')}}
              </Button>
            </Col>
            <Col span="12" align="left">
              <Button 
                name="contact_cancel"
                type="default"
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
        name: 'AddContact',
        components: {}
    })
    export default class AddContact extends AbpBase{
      private contactData = {
        name: '',
        phone: '',
        mail: '',
        role: '',
        description: '',
      }
      ruleValidate:any = {}

      async saveContact(name: string) {
        const form = await this.$refs[name] as any;     
            await form.validate(async(valid: boolean) => {
                if (valid) {
                    let contactInfor = await this.$store.dispatch({
                      type:'contact/saveContact',
                      params: this.contactData
                    })
                    this.$emit("createCompleted", contactInfor)
                    this.saved()
                } else {
                    this.$Message.error('Fail!');
                }
            })
        
      }
      saved() {
        this.$Message.info(this.$t('common.saved'));
        this.$emit("closeModal", false)
      }
      cancel() {
        this.$Message.info(this.$t('common.cancel'));
        this.$emit("closeModal", false)
      }

      created() {
        this.contactData = {
          name: '',
          phone: '',
          mail: '',
          role: '',
          description: '',
        }
        this.ruleValidate = {
          name: [
              { required: true, message: this.$t('customerManagement.errName'), trigger: 'blur' }
          ],
          mail: [
              { required: true, message: this.$t('customerManagement.errEmail'), trigger: 'blur' },
              { type: 'email', message: this.$t('customerManagement.errEmailFormat'), trigger: 'blur' }
          ],
          phone: [
              { required: true, message: this.$t('customerManagement.errPhone'), trigger: 'blur' }
          ],
          role: [
              { required: true, message: this.$t('contact.errRole'), trigger: 'blur' }
          ],
          // description: [
          //     { required: true, message: this.$t('customerManagement.errDescription'), trigger: 'blur' }
          // ],
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
    .code-row-bg{
      margin-top:10px
    }
    .mb-1{
      margin-bottom: 15px;
    }
</style>
<style lang="scss">
</style>