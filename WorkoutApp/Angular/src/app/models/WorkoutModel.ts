import {ExerciseModel} from "./ExerciseModel";

export interface WorkoutModel {
  id: string,
  date: Date,
  type: string,
  exercises: ExerciseModel[]
}
