import {Injectable} from '@angular/core';
import {HttpService} from "./http.service";
import {Observable} from "rxjs";
import {WorkoutModel} from "../models/WorkoutModel";
import {Resource} from "../enums/resource";
import {Method} from "../enums/method";
import {WorkoutRequest} from "../requests/WorkoutRequest";

@Injectable({
  providedIn: 'root'
})
export class WorkoutService {

  constructor(private _http: HttpService) {
  }

  create(workout: WorkoutRequest): Observable<WorkoutModel> {
    return this._http.request(Resource.WORKOUT, Method.POST, workout);
  }
}
