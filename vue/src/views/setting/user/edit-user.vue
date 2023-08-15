<template>
    <div>
        <Modal
         :title="L('EditUser')"
         :value="value"
         @on-ok="save"
         @on-visible-change="visibleChange"
        >
            <Form ref="userForm"  label-position="top" :rules="userRule" :model="user">
                <Tabs value="detail">
                    <TabPane :label="L('UserDetails')" name="detail">
                        <FormItem :label="L('UserName')" prop="userName">
                            <Input name="edit_userUserName_input" v-model="user.userName" :maxlength="32" :minlength="2"></Input>
                        </FormItem>
                        <FormItem :label="L('Name')" prop="name">
                            <Input name="edit_userName_input" v-model="user.name" :maxlength="32"></Input>
                        </FormItem>
                        <FormItem :label="L('Surname')" prop="surname">
                            <Input name="edit_userSurname_input" v-model="user.surname" :maxlength="1024"></Input>
                        </FormItem>
                        <FormItem :label="L('EmailAddress')" prop="emailAddress">
                            <Input name="edit_userEmailAddress_input" v-model="user.emailAddress" type="email" :maxlength="32"></Input>
                        </FormItem>
                        <FormItem>
                            <Checkbox name="edit_userIsActive_input" v-model="user.isActive">{{L('IsActive')}}</Checkbox>
                        </FormItem>
                    </TabPane>
                    <TabPane :label="L('UserRoles')" name="roles">
                        <CheckboxGroup v-model="user.roleNames">
                            <Checkbox :name="`userRole${role.name}_input`" :label="role.normalizedName" v-for="role in roles" :key="role.id"><span>{{role.name}}</span></Checkbox>
                        </CheckboxGroup>
                    </TabPane>
                </Tabs>
            </Form>
            <div slot="footer">
                <Button name="edit_user_cancel" @click="cancel">{{L('Cancel')}}</Button>
                <Button name="edit_user_save" @click="save" type="primary">{{L('OK')}}</Button>
            </div>
        </Modal>
    </div>
</template>
<script lang="ts">
    import { Component, Vue,Inject, Prop,Watch } from 'vue-property-decorator';
    import Util from '../../../lib/util'
    import AbpBase from '../../../lib/abpbase'
    import User from '../../../store/entities/user'
    @Component
    export default class EditUser extends AbpBase{
        @Prop({type:Boolean,default:false}) value:boolean;
        user:User=new User();
        created(){
            
        }
        get roles(){
            return this.$store.state.user.roles;
        }
        save(){
            (this.$refs.userForm as any).validate(async (valid:boolean)=>{
                if(valid){
                    await this.$store.dispatch({
                        type:'user/update',
                        data:this.user
                    });
                    (this.$refs.userForm as any).resetFields();
                    this.$emit('save-success');
                    this.$emit('input',false);
                }
            })
        }
        cancel(){
            (this.$refs.userForm as any).resetFields();
            this.$emit('input',false);
        }
        visibleChange(value:boolean){
            if(!value){
                this.$emit('input',value);
            }else{
                this.user=Util.extend(true,{},this.$store.state.user.editUser);
            }
        }
        userRule={
            userName:[{required: true,message:this.L('FieldIsRequired',undefined,this.L('UserName')),trigger: 'blur'}],
            name:[{required:true,message:this.L('FieldIsRequired',undefined,this.L('Name')),trigger: 'blur'}],
            surname:[{required:true,message:this.L('FieldIsRequired',undefined,this.L('Surname')),trigger: 'blur'}],
            emailAddress:[{required:true,message:this.L('FieldIsRequired',undefined,this.L('Email')),trigger: 'blur'},{type: 'email'}],
        }
    }
</script>

