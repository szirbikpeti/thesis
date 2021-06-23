import {UserModel} from "./UserModel";
import {NotificationType} from "../enums/notification";

export interface NotificationModel {
  id: string,
  triggeredOn: Date,
  deletedOn: Date,
  type: NotificationType,
  sentByUser: UserModel,
  receivedUser: UserModel,
}
