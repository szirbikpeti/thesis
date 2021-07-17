import {ExerciseRequest} from "./ExerciseRequest";

export interface WorkoutRequest {
  id: string,
  date: Date,
  type: string,
  exercises: ExerciseRequest[],
  fileIds: string[]
}
