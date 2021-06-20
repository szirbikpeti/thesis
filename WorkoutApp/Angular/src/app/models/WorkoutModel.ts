import {ExerciseModel} from "./ExerciseModel";
import {FileModel} from "./FileModel";

export interface WorkoutModel {
  id: string,
  date: Date,
  type: string,
  exercises: ExerciseModel[],
  files: FileModel[],
  createdOn: Date,
  modifiedOn: Date
}
