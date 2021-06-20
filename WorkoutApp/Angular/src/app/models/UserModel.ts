import {FileModel} from "./FileModel";

export interface UserModel {
  id: string,
  fullName: string,
  userName: string,
  email: string,
  about: string,
  permissions: string[],
  profilePicture: FileModel,
  birthDay?: Date,
  lastSignInOn: Date,
  requestingUserIds: string[],
  requestedUserIds: string[],
}
