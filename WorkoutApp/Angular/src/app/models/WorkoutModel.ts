import {ExerciseModel} from "./ExerciseModel";
import {FileModel} from "./FileModel";
import {PostModel} from "./PostModel";
import {WorkoutType} from "../enums/workout";

export interface WorkoutModel {
  id: string,
  date: Date,
  type: WorkoutType,
  distance?: number,
  duration?: string,
  exercises: ExerciseModel[],
  files: FileModel[],
  relatedPost?: PostModel;
  createdOn: Date,
  modifiedOn: Date
}
