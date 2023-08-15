<template>
    <div>
      <PageHeading>
        <template #header>{{$t('contact.editContact')}}</template>
        <template #button>
          <Button
            name="contact_delete"
            class="button btn-edit"
            @click="deleteContact"
          >{{$t('common.delete')}}</Button>
        </template>
      </PageHeading>
      <Form ref="contactData" :model="contactData" :rules="ruleValidate" :label-width="200" label-position="left" class="add-invoice">
        <FormItem :label="$t('contact.name')" prop="name">
              <Input name="contactName_input" v-model="contactData.name" placeholder="Enter your name"></Input>
        </FormItem>
        <FormItem :label="$t('contact.email')" prop="mail">
              <Input name="contactMail_input" v-model="contactData.mail" placeholder="Enter your name"></Input>
        </FormItem>
        <FormItem :label="$t('contact.phone')" prop="phone">
              <Input name="contactPhone_input" v-model="contactData.phone" placeholder="Enter your name"></Input>
        </FormItem>
        <FormItem :label="$t('contact.role')" prop="role">
              <Input name="contactRole_input" v-model="contactData.role" placeholder="Enter your name"></Input>
        </FormItem>
        <FormItem :label="$t('contact.description')" prop="description">
              <Input name="contactDescription_input" v-model="contactData.description" type="textarea" :rows="4" placeholder="Enter something..." />
        </FormItem>
        <Row type="flex" class="code-row-bg" justify="space-around" align="middle">
            <Col span="11" align="right">
                <Button 
                  name="contact_save"
                  class="button btn-add" 
                  size="large"
                  @click="saveContact('contactData')"
                >
                  {{$t('common.save')}}
                </Button>
            </Col>
            <Col span="12"  >
                <Button 
                  name="contact_cancel"
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
    import { EContractType } from '@/store/entities/contract-type';
    @Component({
        name: 'EditContact',
        components: {PageHeading}
    })
    export default class EditContact extends AbpBase{
      private contactData = {
        name: '',
        phone: '',
        mail: '',
        role: '',
        description: '',
        id: null,
      }
      ruleValidate:any = {}
      @Watch("$route")
        getEvent() {            
            this.fetchData();
        }
      async saveContact(name: string) {
        const form = await this.$refs[name] as any;     
            await form.validate(async(valid: boolean) => {
                if (valid) {
                    await this.$store.dispatch({
                      type:'contact/saveContact',
                      params: this.contactData
                    })
                    this.showAdd()
                } else {
                    this.$Message.error('Fail!');
                }
            })
        
      }

      async deleteContact() {
          if(this.$route.params.contactId) {
              await this.$store.dispatch({
              type: "contact/deleteContact",
              id: this.$route.params.contactId
              });
          this.$Message.info(this.$t('common.deleted'));
          this.showAdd()
          }
      }
      showAdd () {
        this.$router.push({ name: 'Contact'})
      }

      beforeMount() {
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
        this.fetchData()
      }

      async fetchData() {
        if (this.$route.params.contactId) {
          let data = await this.$store.dispatch({
            type:'contact/getById',
            contactId: this.$route.params.contactId
          })
          this.contactData = data
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
</style>
<style lang="scss">
</style>