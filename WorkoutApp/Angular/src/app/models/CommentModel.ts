import {UserModel} from "./UserModel";

export interface CommentModel {
  id: string,
  comment: string,
  commentedOn: Date,
  modifiedOn: Date,
  user: UserModel,
}
