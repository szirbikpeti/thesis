import {ExerciseRequest} from "./ExerciseRequest";

export interface WorkoutRequest {
  date: Date,
  type: string,
  exercises: ExerciseRequest[],
  fileIds: string[]
}
