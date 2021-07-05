import {UserModel} from "./UserModel";

export interface FriendModel {
  followerUsers: UserModel[];
  followedUsers: UserModel[];
}
