import {UserModel} from "./UserModel";
import {FileModel} from "./FileModel";
import {CommentModel} from "./CommentModel";
import {WorkoutModel} from "./WorkoutModel";

export interface PostModel {
  id: string,
  postedOn: Date,
  description: string,
  user: UserModel,
  files: FileModel[],
  workout: WorkoutModel,
  comments: CommentModel[],
  likingUsers: UserModel[]
}
