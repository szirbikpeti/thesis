import {UserModel} from "./UserModel";

export interface FeedbackModel {
  id: string,
  feedback: string,
  stars: number,
  createdOn: Date,
  user: UserModel,
}
