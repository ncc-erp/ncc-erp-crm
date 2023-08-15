
export const EPriorityList = [{id: 0, name: "Block"},{id: 1, name: "Trival"},{id: 2, name: "Minor"},{id: 3, name: "Major"},{id: 4, name: "Critical"}] 
export const EEntityTypeList = [{id: 0, name: "Invoice"},{id: 1, name: "Client"},{id: 2, name: "Project"},{id: 3, name: "Contract"},{id: 4, name: "Deal"},{id: 5, name: "Assignment"}] 
export const EStatus = [{id: 0, name: "Todo"},{id: 1, name: "In Progress"},{id: 2, name: "Done"}] 
export const EmailStatus = [{id: 0, name: "Unsent"},{id: 1, name: "Sending"},{id: 2, name: "Sent"},{id: 3, name: "Email not exist"}] 

export interface ICommonList {
    id: Number
    name: String
}
export interface IEntityList {
    entityId: Number
    entityType: Number
}

