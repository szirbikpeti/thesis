import {Time} from "@angular/common";

export interface SetRequest {
  reps: number,
  weight: number,
  duration?: Time
}
