import {UserModel} from "./UserModel";
import {FileModel} from "./FileModel";
import {CommentModel} from "./CommentModel";

export interface PostModel {
  id: string,
  postedOn: Date,
  description: string,
  user: UserModel,
  files: FileModel[],
  comments: CommentModel[],
  likedUsers: UserModel[]
}
