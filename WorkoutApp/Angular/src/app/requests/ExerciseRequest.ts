import {SetRequest} from "./SetRequest";

export interface ExerciseRequest {
  name: string,
  equipment: string,
  sets: SetRequest[]
}
