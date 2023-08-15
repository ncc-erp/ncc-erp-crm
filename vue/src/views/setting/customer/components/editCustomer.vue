<template>
    <div class="detail-customer">
      <PageHeading>
        <template #header>{{$t('customerManagement.customersEdit')}}</template>
        <template #button>
        <Button name="customer_save" class="button btn-add" @click="onSubmit('formData')">{{$t('common.save')}}</Button>
        <Button name="customer_back" type="default" @click="onBack">{{$t('common.backToList')}}</Button>
        <Button name="customer_delete" type="default"  @click="openConfirmDeletePopup">{{$t('common.delete')}}</Button>
        </template>
      </PageHeading>
      <Form ref="formData" :model="formData" :rules="ruleValidate" :label-width="300" label-position="left" class="add-invoice">
          <FormItem :label="$t('customerManagement.clientName')" prop="name">
            <Input name="customerName_input" v-model="formData.name" :placeholder="$t('common.placeholderEnter',{field: $t('customerManagement.clientName')})" />
          </FormItem>
          <FormItem :label="$t('customerManagement.country')" prop="country">
            <Input name="customerCountry_input" v-model="formData.country" :placeholder="$t('common.placeholderEnter',{field: $t('customerManagement.country')})" />
          </FormItem>
          <FormItem :label="$t('customerManagement.email')" prop="mail">
            <Input name="customerMail_input" v-model="formData.mail" :placeholder="$t('common.placeholderEnter',{field: $t('customerManagement.email')})" />
          </FormItem>
          <FormItem :label="$t('customerManagement.region')" prop="region">
            <Select name="customerRegionId_input" v-model="formData.regionId" :clearable="true" :placeholder="$t('common.placeholderSelect',{field: $t('common.selectRegion')})">
                <Option
                v-for="(item,index) of regions"
                :key="index"
                :value="item.regionId"
                >{{item.regionName}}</Option>
            </Select>
          </FormItem>
          <FormItem :label="$t('customerManagement.website')" prop="website">
            <Input name="customerWebsite_input" v-model="formData.website" :placeholder="$t('common.placeholderEnter',{field: $t('customerManagement.website')})" />
          </FormItem>
          <FormItem :label="$t('customerManagement.phone')" prop="phone">
            <Input name="customerPhone_input" v-model="formData.phone" :placeholder="$t('common.placeholderEnter',{field: $t('customerManagement.phone')})" />
          </FormItem>
          <FormItem :label="$t('customerManagement.type')" prop="type">
            <Select name="customerType_input" v-model="formData.type" :placeholder="$t('common.placeholderSelect',{field: $t('customerManagement.type')})">
                <Option v-for="item in typeList" :value="item.id" :key="item.id">{{ item.name }}</Option>
            </Select>
          </FormItem>
          <FormItem :label="$t('common.selectStatus')" prop="type">
              <Select name="customerStatus_input" v-model="formData.status" :placeholder="$t('common.placeholderSelect',{field: $t('common.selectStatus')})">
                  <Option
                  v-for="(item,index) of statusList"
                  :key="index"
                  :value="item.id"
                  >{{item.name}}</Option>
              </Select>
          </FormItem>
          <FormItem :label="$t('customerManagement.description')" prop="description">
              <Input name="customerDescription_input" v-model="formData.description" :autosize="{minRows: 5}" :placeholder="$t('common.placeholderEnter',{field: $t('customerManagement.description')})" type="textarea" />
          </FormItem>
          <FormItem :label="$t('customerManagement.addContact')">
              <div v-for="(item, index) in contacts" :key="item.id" class="item-contact">
                  <Row type="flex">
                  <Col span="6" align="left" class="change-contact">
                      <Select :name="`customerContactId${index}_input`" @on-change="changeContact(item.id)" v-model="item.id" filterable>
                          <Option v-for="item in allDataContact" :value="item.id" :key="item.id   ">{{ item.name }}</Option>
                      </Select>
                  </Col>
                  <Col span="8" align="left">
                      {{$t('contact.email')}}: {{item.mail}}
                  </Col>
                  <Col span="5" align="left">
                      {{$t('contact.phone')}}: {{item.phone}}
                  </Col>
                  <Col span="5" align="left">
                      {{$t('contact.role')}}: {{item.role}}
                      <i @click="deleteContact(item.id)" class="icon-small ivu-icon ivu-icon-md-close-circle"></i>
                  </Col>
                  </Row>
              </div>
              <!-- <div align="center">
                  <i @click="increaseContact" class="icon ivu-icon ivu-icon-ios-add-circle-outline"></i>
              </div> -->
              <div type="flex">
                <Row>
                    <Col span="12" align="left">
                        <Button name="customerContact_add" span="6" class="button btn-add" v-if="!isShowAddContact" @click="increaseContact()" 
                        >{{$t('contact.addContact')}}
                        </Button>
                    </Col>
                    <Col span="6" align="left">
                        <i @click="addRowContact()" class="icon ivu-icon ivu-icon-ios-add-circle-outline"></i>
                    </Col>
                </Row>
              </div>
              <Modal
                v-model="isShowAddContact"
                width="50%">
                <div slot="header" align="center"><h1>{{$t('contact.addContact')}}</h1></div>
                <addContact class="addInvoicePopup" @closeModal="collapse" @createCompleted="updateContacts" v-if="isShowAddContact"/>
                <div slot="footer" class="button-zone" align="center">
                </div>
              </Modal>
          </FormItem>
      </Form>
      <Modal v-model="isShowConfirmDelete" :title="$t('common.notification')">
        <p>{{$t('customerManagement.confirmDeleteNotice')}}</p>
        <Row slot="footer" class="button-zone">
          <Button
            name="customerContact_save"
            class="button btn-add"
            size="default"
            @click="deleteCustomer"
          >
            {{$t('common.accept')}}
          </Button>
          <Button
            name="customerContact_cancel"
            type="error"
            size="default"
            ghost
            @click="isShowConfirmDelete=false"
          >{{$t('common.cancel')}}</Button>
        </Row>
      </Modal>
    </div>
</template>
<script lang="ts">
    import { Component, Vue, Prop, Emit, Watch } from 'vue-property-decorator';
    import Util from '@/lib/util'
    import AbpBase from '@/lib/abpbase'
    import PageHeading from "@/components/heading/heading.vue";
    import {ListTypeCustomer, ListStatusCustomer} from '@/store/entities/project'
    import AddContact from '../../contact/components/add-contact.vue'
    @Component({
        name: 'editCustomer',
        components: {
          PageHeading, AddContact
        }
    })
    export default class editCustomer extends AbpBase{
      private formData: any = {}
      private isClickSubmit: boolean = false
      private isShowAddContact = false;
      private minIdContact: number = 0
      ruleValidate:any = {}
      typeList = ListTypeCustomer
      statusList = ListStatusCustomer
      private allDataContact: any = []
      private allData: any = []
      private contacts: any = [
          {
              id: null,
              mail: '',
              phone: '',
              role: '',
          }
      ]
      private regions: any[] = []
      isShowConfirmDelete = false
        
      @Watch("$route")
      getEvent() {
        this.fetchData();
      }
      private async fetchData() {
        this.getDataAll()
        if (this.$route.params.customerId) {
          this.getDataContacts()
        }
        this.getDropdownRegion()
        this.formData = {
          phone: "",
          mail: "",
          regionId: "",
          country: "",
          website: "",
          description: "",
          name: "",
          type: null,
          status: null}
        if(this.$route.params.customerId) {
          let response = await this.$store.dispatch({
            type: "customer/getCustomer",
            id: this.$route.params.customerId
          });
          this.formData = response
        }
        this.ruleValidate = {
           name: [
                { required: true, message: this.$t('customerManagement.errName'), trigger: 'blur' }
            ], 
            mail: [
                { required: true, message: this.$t('customerManagement.errEmail') , trigger: 'blur' },
                { type: 'email', message: this.$t('customerManagement.errEmailFormat'), trigger: 'blur' }
            ],
            country: [
                { required: true, message: this.$t('customerManagement.errCountry'), trigger: 'blur' }
            ],
            // website: [
            //     { required: true, message: this.$t('customerManagement.errWebsite'), trigger: 'blur' }
            // ],
            // phone: [
            //     { required: true, message: this.$t('customerManagement.errPhone'), trigger: 'blur' }
            // ],
            // description: [
            //     { required: true, message: this.$t('customerManagement.errDescription'), trigger: 'blur' },
            // ],
            // type: [
            //     { required: true, type: 'number', message: this.$t('customerManagement.errType'), trigger: 'change' }
            // ]
        }
      }
      openConfirmDeletePopup() {
        this.isShowConfirmDelete = true
      }
      async deleteCustomer() {
        if(this.$route.params.customerId) {
          await this.$store.dispatch({
            type: "customer/deleteClient",
            id: this.$route.params.customerId
          });
          this.$Message.info(this.$t('common.deleted'));
          this.isShowConfirmDelete = false
          this.onBack()
        }
      }
      private beforeMount() {
        this.fetchData()
      }
      private async onSubmit(name) {
        const form = await this.$refs[name] as any;     
        await form.validate(async(valid: boolean) => {
            if (valid) {
                await this.edit()
            } else {
                this.$Message.error('Fail!');
            }
        })
      }
      async edit() {
        let listIdContacts = this.contacts.filter(element => element.id && element.id > 0).map(element => element.id)
        listIdContacts = [...new Set(listIdContacts)]
        await this.$store.dispatch({
        type: "customer/saveClient",
        data: this.formData
        })
        this.$Message.info(this.$t('common.saved'));
        await this.$store.dispatch({
        type: "contact/saveClientContact",
        listContact: {
            clientId: this.$route.params.customerId,
            contacts: listIdContacts
        }
        })
        await this.onBack()
      }
      private onBack() {
        this.$router.push({ name: 'customer'})
      }

      async getDataAll() {
          let data = await this.$store.dispatch({
            type:'contact/getAllPagging',
            params: {
                maxResultCount: 10000,
                skipCount: 0
            }
          })
          this.allDataContact = data.items
          this.allData = this.allDataContact
      }

      private changeContact(id: number) {
          if (id) {
              this.contacts.forEach(element => {
                  if (element.id === id) {
                      this.allDataContact.forEach(el => {
                          if (el.id === id) {
                              element.mail = el.mail
                              element.phone = el.phone
                              element.role = el.role
                          }
                      });
                  }
              });
          }
      }

      private increaseContact() {
          this.isShowAddContact = true
      }
        
      collapse(value: boolean = false) {
          this.isShowAddContact = value
      }
      private addRowContact() {
          this.minIdContact -= 1
          let newRow = {
              id: this.minIdContact,
              mail: '',
              phone: '',
              role: ''
          }
          this.contacts.push(newRow)
      }
      updateContacts(contactInfor: object = null) {
            let lastItem = this.contacts.at(-1)
            
            if (lastItem != null && lastItem.id < 1) {
                lastItem.id = contactInfor['id'],
                lastItem.mail = contactInfor['mail'],
                lastItem.phone = contactInfor['phone'],
                lastItem.role = contactInfor['role']
            } else {
                let newContact = {
                    id: contactInfor['id'],
                    mail: contactInfor['mail'],
                    phone: contactInfor['phone'],
                    role: contactInfor['role']
                }
                this.contacts.push(newContact)
            }
            this.getDataAll();
        }

      private deleteContact(id: number) {
            if (this.contacts.length > 1) {
                this.contacts = this.contacts.filter(element => element.id != id)
            } else {
                this.contacts = [
                    {
                        id: this.minIdContact,
                        mail: '',
                        phone: '',
                        role: ''
                    }
                ]
            }
      }
      private async getDataContacts() {
        this.contacts = await this.$store.dispatch({
            type:'contact/getByClient',
            clientId: this.$route.params.customerId
        })
        
      }

      async getDropdownRegion(){
            let data = await this.$store.dispatch({
                type: 'customer/getDropdownRegion'
            })
            this.regions = data
        }
    }
</script>
<style lang="scss" scoped>  
    .code-row-bg{
        margin-top: 10px;
    }
    .detail-customer{
        padding: 20px;
    }
    .button-back{
      margin-top: 20px;
      .btn-submit{
        margin-right: 15px;
      }
    }
    .error-text {
        color: red;
    }
    .change-contact {
        padding-right: 15px;
    }
    .icon {
        font-size: 32px;
    }
    .item-contact {
        padding-bottom: 10px;
    }
    .icon-small {
        font-size: 30px;
        float:right;
    }
    Button {
        margin-right: 5px;
    }
</style>
