import {Timestamp} from "rxjs";
import {ExerciseModel} from "./ExerciseModel";

export interface SetModel {
  id: string,
  reps: number,
  weight: number,
  duration?: Timestamp<any>,
  exercise: ExerciseModel
}
