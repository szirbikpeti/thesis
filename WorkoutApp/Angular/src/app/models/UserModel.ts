import {FileModel} from "./FileModel";

export interface UserModel {
  id: string,
  fullName: string,
  userName: string,
  email: string,
  about: string,
  profilePicture: FileModel,
  birthDay?: Date,
  lastSignInOn: Date
}
