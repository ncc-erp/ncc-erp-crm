export enum EProject{
    page_10 = 10
}
export enum EProjectType {
    ODC = 0,
    FixPriced = 1,
    TNM = 2,
    Internal = 3
  }
export const CurrencyType = [{id: 0, name: "USD"},{id: 1, name: "VND"},{id: 2, name: "EUR"},{id: 3, name: "JPY"}] 
export const ListType = [{id: 0, name: "ODC"},{id: 1, name: "FixedPrice"},{id: 2, name: "TNM"},{id: 3, name: "Internal"}]
export const ListTypeCustomer = [{id: 0, name: "Prospect"},{id: 1, name: "Reseller"},{id: 2, name: "Partner"},{id: 3, name: "Vendor"},{id: 4, name: "Other"}]
export const ListStatus = [{id: 0, name: "Pending"},{id: 1, name: "Chasing"},{id: 2, name: "Fail"},{id: 3, name: "Paid"}]
export const ListStatusProject = [{id: 0, name: "Pending"},{id: 1, name: "Ongoing"},{id: 2, name: "Finish"}]
export const ListStatusCustomer = [{id: 0, name: "New"},{id: 1, name: "RegularContact"},{id: 2, name: "InactiveContact"}]
export const ListStatusDeal = [{id: 0, name: "RequestCome"},{id: 1, name: "ProcessingRequest"},{id: 2, name: "ProjectInProgress "},{id: 3, name: "ProjectWin"},{id: 4, name: "ProjectFail"},{id: 5, name: "DealLost"}]
export const PageSizeOpts = [10, 20, 50, 100]

export interface ICommonList {
    id: Number
    name: String
}
export interface IdataSavePeopleInCharge {
    userId: Number
    position: String
    rate: Number
    manMonth: Number
}
export interface IDataAddInvoice {
    projectId: Number
    contractId: Number
    time: string
    value: Number
    currency: Number
    assignee: number
    invoiceType: Number
    invoiceName ?: string
    status?: number
    id?: number
}

export interface IFilterItems {
    propertyName?: String
    value?: any,
    comparison?: number
}
export interface IAssignee {
    userId: Number
    userName: string
}
export interface ISaveProject {
    id?: number,
    name: string,
    code: string,   
    type: Number,
    clientId: Number,
    status: Number,
    description: string,
    users: Array<Number>
}
export interface ISaveDeal {
    ownerId: number,
    id?: number,
    description: string,
    clientId: number,
    status: number,
    priority: number,
    amount: number,
    name: string,
    winReason?: string,
    loseReason?: string,
    dealStartDate: any,
    dealLastFollow: any,
    dealDetail: any
}
export interface IDataList {
    name: string,
    type: Number,
    no: Number,
    clientId: Number,
    clientName: string,
    status: Number,
    statusName: string
}
export interface IDataProject {
    totalCount: Number,
    items: IDataList
}
export interface IInvoiceUserDetail {
    invoiceUserId?:Number,
    userId?: Number,
    rate?: Number,
    position?: String,
    manMonth?: Number,
    isIncrease?: boolean,
    idPeople?: Number,
    employee?: string
}
export interface IFileDetail {
    id?: Number,
    fileName: string,
    fileUrl: string,
    typeString?: string
}

export interface IEmailTemplate {
    id?: Number,
    name: string,
    subject: string,
    content: string,
}