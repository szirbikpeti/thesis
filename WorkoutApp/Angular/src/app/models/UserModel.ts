import {FileModel} from "./FileModel";

export interface UserModel {
  id: string,
  fullName: string,
  userName: string,
  email: string,
  about: string,
  permissions: string[],
  profilePicture: FileModel,
  birthday?: Date,
  lastSignedInOn: Date,
  sourceUserIds: string[],
  targetUserIds: string[],
  followerUserIds: string[],
  followedUserIds: string[],
}
