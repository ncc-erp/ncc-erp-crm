import Entity from './entity'
export default class User extends Entity<number>{
    password:string;
}

export const ETeamMember = [{id: 1, name: "Leader "},{id: 2, name: "ViceLeader "},{id: 3, name: "Member"}]
export const EAssignmentStatus = [{id: 0, name: "Todo"},{id: 1, name: "InProgress"},{id: 2, name: "Done"}]
export const EPriority = [{id: 0, name: "Block"},{id: 1, name: "Trival"},{id: 2, name: "Minor"},{id: 3, name: "Major"},{id: 4, name: "Critical"}]