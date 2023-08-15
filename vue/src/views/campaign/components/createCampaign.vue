<template>
<div>
    <PageHeading>
        <template #header>{{$t('emailCampaign.createCampaign')}}</template>
    </PageHeading>
    <div class="container">
        <div>
            <Row>
                <Col span="5">{{$t('emailCampaign.name')}}: </Col>
                <Col span="12"><Input name="inputName" v-model="formData.name" ></Input></Col>
            </Row>
        </div>
        <div class="margin-top-10">
            <div>{{$t('emailCampaign.contactList')}} :</div>
            <Table name="campaign_tbl" @on-selection-change="selectList" :columns="columns" :data="searchResult" height="350" :no-data-text="$t('common.nodatas')" border>
            </Table>
        </div>
        <div class="button-zone">
            <Button type="primary" @click="create">{{ $t('common.create') }}</Button>
            <Button @click="cancel">{{ $t('common.cancel') }}</Button>
        </div>
    </div>
</div>
</template>
<script lang="ts">
import { Component, Vue, Inject, Prop } from "vue-property-decorator";
import AbpBase from "../../../lib/abpbase";
import PageHeading from "../../../components/heading/heading.vue";
import Editor from './tinyMCE.vue'
@Component({
  name: 'createCampaign',
  components: {
    Editor,
    PageHeading
  }
})
export default class Roles extends AbpBase {
    private formData: any = {
        name: '',
        contactId: []
    }
    private searchResult: any = []
    private columns: any = []
    private content: any = ""
    cancel() {
        this.$router.push({name: "campaign"})
    }
    async create() {
        if(this.formData.name && this.formData.contactId) {
            const response = await this.$store.dispatch({
                type: "campaign/addCampaign",
                data: this.formData
            })
            this.$router.push({
                name: "detailCampaign",
                params: { campaignId: response.result.id }
            }).catch(()=>{})
        }
        else this.$Message.error('Name or contact had empty!');
    }
    selectList(list:any) {
        this.formData.contactId = list.map((item:any) => {
            return item.id
        })
    }
    async created() {

        this.columns = [
        {
            type: 'selection',
            width: 60,
            align: 'center'
        },
        {
            title: this.$t('contact.name'),
            resizable: true,
            align: 'center',
            key: "name",
        },
        {
            title: this.$t('contact.email'),
            key: "mail",
            resizable: true,
            align: 'center',
        },
        {
            title: this.$t('contact.phone'),
            key: "phone",
            resizable: true,
            align: 'center',
        },
        {
            title: this.$t('contact.role'),
            key: "role",
            resizable: true,
            align: 'center',
        },
        {
            title: this.$t('contact.description'),
            key: "description",
            resizable: true,
            align: 'center',
        },
    ]
        let data = await this.$store.dispatch({
        type:'contact/getAllPagging',
        params: {maxResultCount: 10000}
        })
        this.searchResult = data.items
    }
}
</script>
<style lang="scss" scope>
    .margin-top-10 {
        margin-top: 10px;
    }
    .button-zone {
        margin-top: 20px;
        padding: 10px;
        text-align: center;
        button {
            margin-right: 10px;
        }
    }
    .container {
        margin-top: 20px;
    }
</style>
