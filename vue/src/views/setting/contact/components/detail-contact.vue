<template>
    <div>
      <PageHeading>
        <template #header>{{$t('contact.contactDetails')}}</template>
      </PageHeading>
      <div>
        <Row type="flex" class="contact-data">
          <Col span="4">
              {{$t('contact.name')}} :
          </Col>
          <Col span="20">
            <p>{{contactData.name}}</p>
          </Col>
        </Row>
        <Row type="flex" class="contact-data">
          <Col span="4">
              {{$t('contact.email')}} :
          </Col>
          <Col span="20">
            <p>{{contactData.mail}}</p>
          </Col>
        </Row>
        <Row type="flex" class="contact-data">
          <Col span="4">
              {{$t('contact.phone')}} :
          </Col>
          <Col span="20">
            <p>{{contactData.phone}}</p>
          </Col>
        </Row>
        <Row type="flex" class="contact-data">
          <Col span="4">
              {{$t('contact.role')}} :
          </Col>
          <Col span="20">
            <p>{{contactData.role}}</p>
          </Col>
        </Row>
        <Row type="flex" class="contact-data">
          <Col span="4">
              {{$t('contact.description')}} :
          </Col>
          <Col span="20">
            <p>{{contactData.description}}</p>
          </Col>
        </Row>
        <Row type="flex" class="contact-data">
          <Col span="4">
              {{$t('project.client')}} :
          </Col>
          <Col span="20">
            <Table name="detailContact_tbl"
            :columns="columns"
            :data="clientData"
            :no-data-text="$t('common.nodatas')"
            border
            >
            <template slot-scope="{row}" slot="clientName">
              <span>
                <a @click="openDetailCustomer(row.id)">{{row.name}}</a>
              </span>
            </template>
            </Table>
          </Col>
        </Row>
      </div>
      <Row type="flex" class="code-row-bg" justify="space-around" align="middle">
          <Col span="24" align="center">
              <Button type="default"
              size="large"
              @click="showAdd"
              >{{$t('common.cancel')}}
              </Button>
          </Col>
      </Row>
    </div>
</template>
<script lang="ts">
    import { Component, Vue, Prop, Emit, Watch } from 'vue-property-decorator';
    import AbpBase from '@/lib/abpbase'
    import { EContractType } from '@/store/entities/contract-type';
    import { EInvoiceStatus, EListStatusCustomer } from '../../../../store/entities/status';
    import PageHeading from "@/components/heading/heading.vue";
    @Component({
        name: 'DetailContact',
        components: {PageHeading}
    })
    export default class DetailContact extends AbpBase{
      columns: any
      private contactData = {
        name: '',
        phone: '',
        mail: '',
        role: '',
        description: '',
        clients: []
      }
      private clientData:any = []

      @Watch("$route")
      getEvent() {            
          this.fetchData();
      }

      showAdd () {
        this.$router.push({ name: 'Contact'})
      }
      
      beforeMount() {
        this.fetchData()
        this.columns = [
          {
            title: this.$t("customerManagement.no").toString(),
            key: "no",
            width: 100,
            align: 'center',
          },
          {
            title: this.$t("customerManagement.clientName").toString(),
            slot: "clientName",
            align: 'center',
          },
          {
            title: this.$t("customerManagement.email").toString(),
            key: "mail",
            align: 'center',
          },
          {
            title: this.$t("customerManagement.phone").toString(),
            key: "phone",
            align: 'center',
          },
          {
            title: this.$t("customerManagement.status").toString(),
            key: "status",
            align: 'center',
          },
        ]
      }

      converInvoiceStatus(value: number) {
        switch (value) {
          case EListStatusCustomer.New:
            return this.$t('customerManagement.New');
          case EListStatusCustomer.RegularContact:
            return this.$t('customerManagement.RegularContact');
          case EListStatusCustomer.InactiveContact:
            return this.$t('customerManagement.InactiveContact');
        }
      }

      private openDetailCustomer(id?: any) {
        this.$router.push({ name: 'detailCustomer', params: { customerId: id } })
      }
      
      async fetchData() {
        let countNo = 1
        if (this.$route.params.contactId) {
          let data = await this.$store.dispatch({
            type:'contact/getById',
            contactId: this.$route.params.contactId
          })
          this.contactData = data
          this.clientData = data.clients.map((element: any) => {
            let dataElement = element
            dataElement.no = countNo 
            countNo++
            dataElement.status = this.converInvoiceStatus(element.status)
            return dataElement
          })
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