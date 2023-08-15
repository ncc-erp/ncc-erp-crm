import { Store, Module, ActionContext } from 'vuex'
import ListModule from './list-module'
import ListState from './list-state'
import I18n from '../entities/i18n'
import Ajax from '../../lib/ajax'
import ListMutations from './list-mutations'
import Role from '../entities/role'
import i18n from '../../lang/i18n'
interface i18nState extends ListState<I18n> {}
class i18nMutations extends ListMutations<any>{}
class I18nModule extends ListModule<i18nState,any,I18n>{
  state = {
      name: 'vn',
      totalCount:0,
      currentPage:1,
      pageSize:10,
      list: new Array<I18n>(),
      loading:false,
      editUser:new I18n(),
      roles:new Array<Role>()
  }
  actions = {
    async setLang(context: ActionContext<i18nState, any>, payload: any) {
      context.commit('SET_LANG', payload.data)
    }
  }
  mutations = {
    setCurrentPage(state:i18nState,page:number){
        state.currentPage=page;
    },
    setPageSize(state:i18nState,pagesize:number){
        state.pageSize=pagesize;
    },
    SET_LANG (state, payload) {
      state.name = payload
      sessionStorage.setItem('lang', payload);
      i18n.locale = payload
    }
  }
}
const i18nModule = new I18nModule()
export default i18nModule;