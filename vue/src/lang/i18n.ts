import Vue from 'vue'
import VueI18n from 'vue-i18n'
import vnMessage from './vn'
import enMessage from './en'

Vue.use(VueI18n)

const messages = {
  vn: vnMessage,
  en: enMessage,
}
let currentLanguage = sessionStorage.getItem('lang')
const i18n = new VueI18n({
  locale: currentLanguage ? currentLanguage : 'vn',
  messages,
  fallbackLocale: 'vn',
})

export default i18n