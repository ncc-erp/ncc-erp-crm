import Vue from 'vue';
import Vuex from 'vuex';
Vue.use(Vuex);
import app from './modules/app'
import session from './modules/session'
import account from './modules/account'
import user from './modules/user'
import role from './modules/role'
import tenant from './modules/tenant'
import i18n from './modules/i18n'
import project from './modules/project'
import invoice from './modules/invoice'
import contract from './modules/contract'
import customer from './modules/customer'
import dashboard from './modules/dashboard'
import deal from './modules/deal'
import contact from './modules/contact'
import team from './modules/team'
import task from './modules/task'
import campaign from './modules/campaign'
import email from './modules/email'
import assignment from './modules/assignment'

const store = new Vuex.Store({
    state: {
        //
    },
    mutations: {
        //
    },
    actions: {

    },
    modules: {
        app,
        session,
        account,
        user,
        role,
        tenant,
        i18n,
        project,
        invoice,
        contract,
        customer,
        dashboard,
        deal,
        contact,
        team,
        task,
        campaign,
        email,
        assignment
    }
});

export default store;
