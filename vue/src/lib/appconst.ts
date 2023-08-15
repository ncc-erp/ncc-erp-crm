import url from './url'
const AppConsts= {
    userManagement:{
        defaultAdminUserName: 'admin'
    },
    localization:{
        defaultLocalizationSourceName: 'CRM'
    },
    authorization:{
        encrptedAuthTokenName: 'enc_auth_token'
    },
    appBaseUrl: process.env.NODE_ENV === "production" ? "http://crm.dev.nccsoft.vn" : "http://localhost:8080",
    remoteServiceBaseUrl:url
}
export default AppConsts