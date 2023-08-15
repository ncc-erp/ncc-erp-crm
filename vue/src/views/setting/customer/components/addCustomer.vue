<template>
    <div class="add-customer">
        <!-- <Modal
            v-model="isShow"
            footer-hide
            width="65%"> -->
            
            <!-- <div slot="header" align="center">
                <h1>{{$t('customerManagement.addCustomer')}}</h1>
            </div> -->
            <Form ref="formData" :model="formData" :rules="ruleValidate" :label-width="200" label-position="left" class="add-invoice">
                <FormItem :label="$t('customerManagement.clientName')" prop="name">
                    <Input name="customerName_input" v-model="formData.name" :placeholder="$t('common.placeholderEnter',{field: $t('customerManagement.clientName')})" />
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
                <FormItem :label="$t('customerManagement.country')" prop="country">
                    <Input name="customerCountry_input" v-model="formData.country" :placeholder="$t('common.placeholderEnter',{field: $t('customerManagement.country')})" />
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
                <FormItem :label="$t('common.selectStatus')" prop="status">
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
                    <div v-for="item in contacts" :key="item.id" class="item-contact">
                        <Row type="flex">
                        <Col span="6" align="left" class="change-contact">
                            <Select name="customerContactId_input" @on-change="changeContact(item.id)" v-model="item.id" filterable>
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
                    
                    <div type="flex">
                        <Row>
                            <Col span="12" align="left">
                                <Button name="customerContact_add" span="6" class="button btn-add" v-if="!isShowAddContact" @click="increaseContact()" 
                                >{{$t('contact.addContact')}}
                                </Button>
                                <Icon v-else @click="collapse()" type="md-arrow-dropup"  size="25" />
                            </Col>
                            <Col span="6" align="left">
                                <i @click="addRowContact()" class="icon ivu-icon ivu-icon-ios-add-circle-outline"></i>
                            </Col>
                        </Row>

                    </div>
                    <div v-if="isShowAddContact" class="add-contact">
                         <div slot="header" align="center"><span>{{$t('contact.addContact')}}</span></div>
                        <addContact @closeModal="collapse" @createCompleted="updateContacts" style="margin: 10px 10px 10px 10px" />
                     </div>                   
                </FormItem>
                <Row type="flex" class="code-row-bg" justify="space-around" align="middle" v-if="!isShowAddContact">
                    <Col span="11" align="right">
                        <Button name="customer_save" class="button btn-add" 
                            @click="handleAddCustomer('formData')"
                        >{{$t('common.add')}}</Button>
                    </Col>
                    <Col span="12" align="left">
                        <Button name="customer_cancel" type="default"
                            @click="cancel"
                        >{{$t('common.cancel')}}</Button>
                    
                    </Col>
                </Row>
            </Form>
        <!-- </Modal> -->
    </div>
</template>
<script lang="ts">
    import { Component, Vue, Prop, Emit } from 'vue-property-decorator';
    import Util from '@/lib/util'
    import AbpBase from '@/lib/abpbase'
    import {ListTypeCustomer, ListStatusCustomer} from '@/store/entities/project'
    import AddContact from '../../contact/components/add-contact.vue'
    @Component({
        name: 'addCustomer',
        components: { AddContact }
    })
    export default class addCustomer extends AbpBase{
        value:any=""
        typeList = ListTypeCustomer
        private allDataContact: any = []
        ruleValidate:any = {}
        private minIdContact: number = 0
        private isShowAddContact = false;
        statusList = ListStatusCustomer
        private formData: any = {
            phone: "",
            mail: "",
            regionId: "",
            country: "",
            website: "",
            description: "",
            name: "",
            type: 4,
            status: 0
        }
        private contacts: any = [
            {
                id: this.minIdContact,
                mail: '',
                phone: '',
                role: ''
            }
        ]
        private regions: any[] = []
        created() {
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
                // description: [
                //     { required: true, message: this.$t('customerManagement.errDescription'), trigger: 'blur' },
                // ],
                // phone: [
                //     { required: true, message: this.$t('customerManagement.errPhone'), trigger: 'blur' }
                // ],         
                // type: [
                //     { required: true, type: 'number', message: this.$t('customerManagement.errType'), trigger: 'change' }
                // ],
                // status: [
                //     { required: true, type: 'number', message: this.$t('customerManagement.errStatus'), trigger: 'change' }
                // ]
            }
            this.getDataAll()
            this.getDropdownRegion()     
        }

        private async handleAddCustomer(name) {
            const form = await this.$refs[name] as any;     
            await form.validate(async(valid: boolean) => {
                if (valid) {
                    await this.addCustomer()
                } else {
                    this.$Message.error('Fail!');
                }
            })
        }
        async addCustomer() { 
            let listIdContacts = this.contacts.filter(element => element.id && element.id > 0).map(element => element.id)
            listIdContacts = [...new Set(listIdContacts)]
            
            const customerData = await this.$store.dispatch({
            type: "customer/saveClient",
            data: this.formData
            })
            await this.$store.dispatch({
            type: "contact/saveClientContact",
            listContact: {
                clientId: customerData.id,
                contacts: listIdContacts
            }
            })
            this.$Message.info(this.$t('common.saved'));
            this.$emit('addedClient', customerData);
            this.onSubmit()
        }
        cancel () {
            this.$Message.info(this.$t('common.cancel'));
            this.onSubmit()
        }

        @Emit('onSubmit')
        private onSubmit() {
            return
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

        async getDropdownRegion(){
            let data = await this.$store.dispatch({
                type: 'customer/getDropdownRegion'
            })
            this.regions = data
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
    }
</script>
<style scoped>  
    .code-row-bg{
        margin-top: 10px;
    }
    .add-customer{
        padding: 20px;
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
    .add-contact {
        border-style: ridge;
    }
</style>
