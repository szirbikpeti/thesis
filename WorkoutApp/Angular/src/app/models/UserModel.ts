import {FileModel} from "./FileModel";

export interface UserModel {
  id: string,
  fullName: string,
  userName: string,
  about: string,
  profilePicture: FileModel,
  birthDay?: Date,
  lastSignInOn: Date
}
