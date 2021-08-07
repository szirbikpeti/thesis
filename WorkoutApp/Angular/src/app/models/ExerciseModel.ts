import {SetModel} from "./SetModel";
import {WorkoutModel} from "./WorkoutModel";

export interface ExerciseModel {
  id: string,
  name: string,
  equipment?: string,
  workout: WorkoutModel,
  sets: SetModel[]
}
