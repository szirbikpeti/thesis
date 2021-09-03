import {UserModel} from "./UserModel";

export interface MessageModel {
  id: string,
  message: string,
  sentOn: Date,
  senderUser: UserModel,
  triggeredUser: UserModel
}
