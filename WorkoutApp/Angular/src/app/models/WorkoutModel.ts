import {ExerciseModel} from "./ExerciseModel";
import {FileModel} from "./FileModel";
import {PostModel} from "./PostModel";

export interface WorkoutModel {
  id: string,
  date: Date,
  type: string,
  exercises: ExerciseModel[],
  files: FileModel[],
  relatedPost?: PostModel;
  createdOn: Date,
  modifiedOn: Date
}
