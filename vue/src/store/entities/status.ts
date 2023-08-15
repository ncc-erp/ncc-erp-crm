export enum EInvoiceStatus {
  Pending = 0,
  Chasing = 1,
  Fail = 2,
  Paid = 3
}

export enum EContractStatus {
  Pending = 0,
  OnGoing = 1,
  Finish = 2
}

export enum EProjectStatus {
  Pending = 0,
  OnGoing = 1,
  Finish = 2
}

export enum EContractType {
  ODC = 0,
  FixedPrice = 1,
  TNM = 2,
  Internal = 3
}

export enum EInvoiceStatusList {
  Pending = 'Pending',
  Chasing = 'Chasing',
  Fail = 'Fail',
  Paid = 'Paid'
}

export enum EListStatusCustomer {
  New = 0,
  RegularContact = 1,
  InactiveContact = 2,
}
export enum EDealStatusList {
  RequestCome = "RequestCome",
  ProcessingRequest = "ProcessingRequest",
  ProjectInProgress = "ProjectInProgress",
  ProjectWin = "ProjectWin",
  ProjectFail = "ProjectFail",
  DealLost = "DealLost"
}