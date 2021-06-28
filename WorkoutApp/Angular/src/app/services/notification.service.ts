import {Injectable} from '@angular/core';
import {HttpService} from "./http.service";
import {Observable} from "rxjs";
import {NotificationModel} from "../models/NotificationModel";
import {Resource} from "../enums/resource";
import {Method} from "../enums/method";

@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  constructor(private _http: HttpService) {
  }

  get(): Observable<NotificationModel[]> {
    return this._http.request(Resource.NOTIFICATION, Method.GET);
  }

  delete(id: string): Observable<any> {
    return this._http.request(Resource.NOTIFICATION, Method.DELETE, null, id);
  }
}
