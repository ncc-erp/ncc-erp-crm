export default class Entity<T>{
    id:T
}

export enum EntityDefault {
    Invoice,
    Client,
    Project,
    Contract,
    Deal
}