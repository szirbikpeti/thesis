import { Injectable } from '@angular/core';
import {HttpService} from "./http.service";
import {Observable} from "rxjs";
import {Resource} from "../enums/resource";
import {Method} from "../enums/method";
import {FeedbackModel} from "../models/FeedbackModel";
import {FeedbackRequest} from "../requests/FeedbackRequest";

@Injectable({
  providedIn: 'root'
})
export class FeedbackService {

  constructor(private _http: HttpService) {
  }

  list(): Observable<FeedbackModel[]> {
    return this._http.request(Resource.FEEDBACK, Method.GET);
  }

  send(feedback: FeedbackRequest): Observable<any> {
    return this._http.request(Resource.FEEDBACK, Method.POST, feedback);
  }
}
