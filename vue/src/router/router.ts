// meta: {
//     activeMenu: 'example/list' if set path, the sidebar will highlight the path you set
// }
declare global {
    interface RouterMeta {
        title: string;
        activeMenu?: string;
    }
    interface Router {
        path: string;
        name: string;
        icon?: string;
        permission?: string;
        meta?: RouterMeta;
        component: any;
        children?: Array<Router>;
        hidden?: boolean;
    }
    interface System {
        import(request: string): Promise<any>
    }
    var System: System
}
import login from '@/views/login.vue'
import home from '@/views/home/home.vue'
import main from '@/views/main.vue'

export const locking = {
    path: '/locking',
    name: 'locking',
    component: () => import('../components/lockscreen/components/locking-page.vue')
};
export const loginRouter: Router = {
    path: '/',
    name: 'login',
    meta: {
        title: 'LogIn'
    },
    component: () => import('../views/login.vue')
};
export const otherRouters: Router = {
    path: '/main',
    name: 'main',
    permission: '',
    meta: { title: 'ManageMenu' },
    component: main,
    children: [
        { path: 'home', meta: { title: 'HomePage' }, name: 'home', component: () => import('../views/home/home.vue') }
    ]
}
export const appRouters: Array<Router> = [
    {
        path: '/setting',
        name: 'setting',
        permission: '',
        meta: { title: 'ManageMenu' },
        icon: '&#xe68a;',
        component: main,
        children: [
            { path: 'user', permission: 'Pages.Users', meta: { title: 'Users', activeMenu: 'user' }, name: 'user', component: () => import('../views/setting/user/user.vue') },
            { path: 'role', permission: 'Pages.Roles', meta: { title: 'Roles', activeMenu: 'role' }, name: 'role', component: () => import('../views/setting/role/role.vue') },
            { path: 'tenant', permission: 'Pages.Tenants', meta: { title: 'Tenants', activeMenu: 'tenant' }, name: 'tenant', component: () => import('../views/setting/tenant/tenant.vue') },
            { path: 'teams', meta: { title: 'Teams', activeMenu: 'teams' }, name: 'teams', component: () => import('../views/setting/teams/teams-management.vue') },
            { path: 'emailSetting', meta: { title: 'Email Setting', activeMenu: 'emailSetting' }, name: 'emailSetting', component: () => import('../views/setting/emailSetting/emailSetting.vue') },
        ]
    },

    {
        path: '/',
        name: '',
        permission: '',
        meta: { title: '' },
        component: main,
        children: [
            { path: 'dashboard', meta: { title: 'Dashboard', activeMenu: 'Dashboard' }, name: 'Dashboard', component: () => import('../views/setting/dashboard/dashboard-management.vue') }
        ]
    },
    {
        path: '/',
        name: '',
        permission: '',
        meta: { title: '' },
        component: main,
        children: [
            { path: 'customer', meta: { title: 'Customer', activeMenu: 'customer' }, name: 'customer', component: () => import('../views/setting/customer/customerManagement.vue') },
            { path: 'customer/addCustomer', hidden: true, meta: { title: 'Add Customer', activeMenu: 'customer' }, name: 'addCustomer', component: () => import('../views/setting/customer/components/addCustomer.vue') },
            { path: 'customer/detailCustomer/:customerId', hidden: true, meta: { title: 'Detail Customer', activeMenu: 'customer' }, name: 'detailCustomer', component: () => import('../views/setting/customer/components/detailCustomer.vue') },
            { path: 'customer/editCustomer/:customerId', hidden: true, meta: { title: 'Edit Customer', activeMenu: 'customer' }, name: 'editCustomer', component: () => import('../views/setting/customer/components/editCustomer.vue') },

        ]
    },
    {
        path: '/',
        name: '',
        permission: '',
        meta: { title: '' },
        component: main,
        children: [
            { path: 'deal', meta: { title: 'Deal', activeMenu: 'deal' }, name: 'deal', component: () => import('../views/deal/deal.vue') },
            { path: 'deal/detail/:dealId', hidden: true, meta: { title: 'Deal Detail', activeMenu: 'deal' }, name: 'dealDetail', component: () => import('../views/deal/components/detailDeal.vue') },
            { path: 'deal/AddDeal', hidden: true, meta: { title: 'Deal Create', activeMenu: 'deal' }, name: 'AddDeal', component: () => import('../views/deal/components/addDeal.vue') },
            { path: 'deal/edit-deal/:dealId', hidden: true, meta: { title: 'Edit Deal' }, name: 'editDeal', component: () => import('../views/deal/components/editDeal.vue') },
            { path: 'deal/:isViewDrag', hidden: true, meta: { title: 'Deal' }, name: 'dealDrag', component: () => import('../views/deal/deal.vue') },
            { path: 'deal/true/:dealId', hidden: true, meta: { title: 'Deal' }, name: 'dealDragDetail', component: () => import('../views/deal/deal.vue') },
            // { path: 'invoice/false/:invoiceId', hidden: true, meta: { title: 'Invoice Detail' }, name: 'invoiceDetail', component: () => import('../views/invoice/components/detailInvoice.vue') },
        ]
    },
    {
        path: '/',
        name: '',
        permission: '',
        meta: { title: '' },
        component: main,
        children: [
            { path: 'task', meta: { title: 'Task' }, name: 'task', component: () => import('../views/task/task.vue') },
            { path: 'task/detail/Id=:Id', hidden: true, meta: { title: 'Task Detail' }, name: 'taskDetail', component: () => import('../views/task/components/detailTask.vue') },
            { path: 'task/edit-task/Id=:Id', hidden: true, meta: {title: 'Edit Task'}, name: 'editTask', component: () => import('../views/task/components/editTask.vue')},
            { path: 'task/:isViewDrag', hidden: true, meta: { title: 'Task' }, name: 'taskDrag', component: () => import('../views/task/task.vue') },
            // { path: 'deal/true/:dealId', hidden: true, meta: { title: 'Deal' }, name: 'dealDragDetail', component: () => import('../views/deal/deal.vue') },
            // { path: 'invoice/false/:invoiceId', hidden: true, meta: { title: 'Invoice Detail' }, name: 'invoiceDetail', component: () => import('../views/invoice/components/detailInvoice.vue') },
        ]
    },
    {
        path: '/',
        name: '',
        permission: '',
        meta: { title: '' },
        component: main,
        children: [
            { path: 'project', meta: { title: 'Project' }, name: 'project', component: () => import('../views/setting/project/projectManagement.vue') },
            { path: 'project/:projectId', hidden: true, meta: { title: 'Project Detail' }, name: 'projectDetail', component: () => import('../views/setting/project/components/projectDetail.vue') },
            { path: 'project/edit-project/:projectId', hidden: true, meta: {title: 'Edit Project'}, name: 'editProject', component: () => import('../views/setting/project/components/editProject.vue')}
        ]
    },
    {
        path: '/',
        name: '',
        permission: '',
        meta: { title: '' },
        component: main,
        children: [
            { path: 'invoice', meta: { title: 'Invoice', activeMenu: 'invoice' }, name: 'invoice', component: () => import('../views/invoice/invoice.vue') },
            { path: 'invoice/detail/:invoiceId', hidden: true, meta: { title: 'Invoice Detail', activeMenu: 'invoice' }, name: 'invoiceDetail', component: () => import('../views/invoice/components/detailInvoice.vue') },
            { path: 'invoice/:isViewDrag', hidden: true, meta: { title: 'Invoice' }, name: 'invoiceDrag', component: () => import('../views/invoice/invoice.vue') },
            { path: 'invoice/true/:invoiceId', hidden: true, meta: { title: 'Invoice' }, name: 'invoiceDragDetail', component: () => import('../views/invoice/invoice.vue') },
            { path: 'invoice/false/:invoiceId', hidden: true, meta: { title: 'Invoice Detail' }, name: 'invoiceDetail', component: () => import('../views/invoice/components/detailInvoice.vue') },
            { path: 'invoice/editInvoice/:invoiceId', hidden: true, meta: { title: 'Edit Invoice' }, name: 'editInvoice', component: () => import('../views/invoice/components/editInvoice.vue') },
        ]
    },
    {
        path: '/',
        name: '',
        permission: '',
        meta: { title: '' },
        component: main,
        children: [
            { path: 'contact', meta: { title: 'Contact', activeMenu: 'Contact' }, name: 'Contact', component: () => import('../views/setting/contact/contact-management.vue') },
            { path: 'contact/editContact/:contactId', hidden: true, meta: { title: 'Edit Contact' }, name: 'editContact', component: () => import('../views/setting/contact/components/edit-contact.vue') },
            { path: 'contact/detailContact/:contactId', hidden: true, meta: { title: 'Detail Contact', activeMenu: 'Contact' }, name: 'detailContact', component: () => import('../views/setting/contact/components/detail-contact.vue') },
            { path: 'teams/editTeams/:teamsId', hidden: true, meta: { title: 'Edit Teams' }, name: 'editTeams', component: () => import('../views/setting/teams/components/edit-teams.vue') },
            { path: 'teams/detailTeams/:teamsId', hidden: true, meta: { title: 'Detail Teams'}, name: 'detailTeams', component: () => import('../views/setting/teams/components/detail-teams.vue') },
        ]
    },
    {
        path: '/',
        name: '',
        permission: '',
        meta: { title: '' },
        component: main,
        children: [
            { path: 'campaign', meta: { title: 'Email Campaign', activeMenu: 'campaign' }, name: 'campaign', component: () => import('../views/campaign/index.vue') },
            { path: 'campaign/create-campaign', hidden: true, meta: { title: 'Create Campaign' }, name: 'Create Campaign', component: () => import('../views/campaign/components/createCampaign.vue') },
            { path: 'campaign/detail-campaign/:campaignId', hidden: true, meta: { title: 'Detail Campaign', activeMenu: 'email'}, name: 'detailCampaign', component: () => import('../views/campaign/components/campaignDetail.vue') },
        ]
    },
    {
        path: '/',
        name: '',
        permission: '',
        meta: { title: '' },
        component: main,
        children: [
            { path: 'emailTemplate', meta: { title: 'Email Template', activeMenu: 'EmailTemplate' }, name: 'EmailTemplate', component: () => import('../views/emailTemplate/emailTemplate.vue') },
            { path: 'emailTemplate/createEmailTemplate', hidden: true, meta: { title: 'Create EmailTemplate' }, name: 'createEmailTemplate', component: () => import('../views/emailTemplate/create-email-template.vue') },
            { path: 'emailTemplate/createEmailTemplate/:emailId', hidden: true, meta: { title: 'Edit EmailTemplate' }, name: 'editEmailTemplate', component: () => import('../views/emailTemplate/create-email-template.vue') }
        ]
    },
]
export const routers = [
    loginRouter,
    locking,
    ...appRouters,
    otherRouters
];
