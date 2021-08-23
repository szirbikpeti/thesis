import {Injectable} from '@angular/core';
import {HttpService} from "./http.service";
import {Resource} from "../enums/resource";
import {Method} from "../enums/method";
import {Observable} from "rxjs";
import {UserModel} from "../models/UserModel";
import {FeedbackModel} from "../models/FeedbackModel";

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  constructor(private _http: HttpService ) {
  }

  listUsers(): Observable<UserModel[]> {
    return this._http.request(Resource.ADMIN_USERS, Method.GET);
  }

  listFeedbacks(): Observable<FeedbackModel[]> {
    return this._http.request(Resource.ADMIN_FEEDBACKS, Method.GET);
  }

  blockUser(userId: string): Observable<any> {
    return this._http.request(Resource.ADMIN, Method.DELETE, null, userId);
  }

  restoreUser(userId: string): Observable<any> {
    return this._http.request(Resource.ADMIN, Method.POST, null, userId);
  }
}
