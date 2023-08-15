<template>
  <div>
    <Card>
      <div class="page-body">
      <PageHeading>
        <template #header v-if="!this.$route.params.emailId">{{ $t("assignment.createEmailTemplate") }}</template>
        <template #header v-else>{{ $t("assignment.editEmailTemplate") }}</template>
        <!-- <template #button
          ><Button class="button btn-add" @click="openCreateEmailTemplate">{{
            $t("contact.addContact")
          }}</Button></template
        > -->
      </PageHeading>
        <Form ref="dataSubmit" :model="dataSubmit" :rules="ruleValidate" :label-width="200" label-position="left">
            <FormItem :label="$t('common.name')"  prop="name">
                <Input name="emailTemplateName_input" v-model="dataSubmit.name" :placeholder="$t('common.placeholderEnter',{field: $t('common.name')})" />
            </FormItem>
            <FormItem :label="$t('assignment.subject')"  prop="subject">
                <Input name="emailTemplateSubject_input" v-model="dataSubmit.subject"  :placeholder="$t('common.placeholderEnter',{field: $t('assignment.subject')})" />
            </FormItem>
            <FormItem :label="$t('assignment.content')"  prop="content">
              <Editor name="emailTemplateContent_input" :value="dataSubmit.content" @onChange="handleDescriptionChange" />
            </FormItem>

            <Row type="flex" class="code-row-bg" justify="space-around" align="middle">
                <Col span="11" align="right">
                    <Button 
                      name="emailTemplate_save"
                      type="error"
                      size="large"
                      @click="handleAddProject('dataSubmit')"
                    >
                      {{$t('common.save')}}
                    </Button>
                </Col>
                <Col span="12" align="left">
                    <Button 
                      name="emailTemplate_cancel"
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
    </Card>
  </div>
</template>
<script lang="ts">
import { Component, Vue, Inject, Prop, Watch } from "vue-property-decorator";
import Util from "@/lib/util";
import AbpBase from "@/lib/abpbase";
import PageRequest from "../../store/entities/page-request";
import PageHeading from "../../components/heading/heading.vue";
import editorComponent from "../tinymce/editorComponent.vue";
import { IEmailTemplate } from "@/store/entities/project";
@Component({
  name: "EmailTemplate",
  components: {
    PageHeading,
    Editor: editorComponent
  },
})
export default class createEmailTemplate extends AbpBase {
  ruleValidate: any = {}
  dataSubmit = {} as IEmailTemplate

  @Watch("$route")
    getEvent() {
        this.getEmailTemplate();
    }

  async created() {
      this.ruleValidate = {
          name: [
              { required: true, message: this.$t('assignment.errName') , trigger: 'blur' },
          ],
          subject: [
              { required: true, message: this.$t('assignment.errSubject') , trigger: 'blur' },
          ],
          content: [
              { required: true, message: this.$t('assignment.errContent') , trigger: 'blur' },
          ],
      }
      this.getEmailTemplate()
  }
  deactivated() {
    (this.$refs['dataSubmit'] as any).resetFields();
  }

  activated() {
    (this.$refs['dataSubmit'] as any).resetFields();
  }
  async getEmailTemplate() {
    if (this.$route.params.emailId) {
      let response = await this.$store.dispatch({
        type: "email/getDetailEmailTemplate",
        emailId: this.$route.params.emailId
      });
      this.dataSubmit = response
    }
  }

  handleDescriptionChange(value: string) {
    this.dataSubmit.content = value;
  }
  cancel() {
      this.$Message.info(this.$t('common.cancel'));
      this.$router.push({ name: 'EmailTemplate'});
  }

   async handleAddProject(name) {            
      const form = await this.$refs[name] as any;
      await form.validate(async(valid: boolean) => {
          if (valid) {
              await this.$store.dispatch({
                  type: 'email/saveEmailTemplate',
                  data: this.dataSubmit
              }) 
              this.$Message.info(this.$t('common.saved'));
              this.cancel()
          } else {
              this.$Message.error('Fail!');
          }
      })
      
  }
}
</script>
<style lang="scss" scoped>
</style>
