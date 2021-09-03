import { Injectable } from '@angular/core';
import {HttpService} from "./http.service";
import {Observable} from "rxjs";
import {Resource} from "../enums/resource";
import {Method} from "../enums/method";
import {MessageModel} from "../models/MessageModel";
import {UserModel} from "../models/UserModel";
import {MessageRequest} from "../requests/MessageRequest";

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  constructor(private _http: HttpService) {
  }

  listUsersWithMessage(): Observable<UserModel[]> {
    return this._http.request(Resource.MESSAGE, Method.GET);
  }

  listMessages(triggeredUserId: string): Observable<MessageModel[]> {
    return this._http.request(Resource.MESSAGE, Method.GET, null, triggeredUserId);
  }

  send(message: MessageRequest): Observable<MessageModel> {
    return this._http.request(Resource.MESSAGE, Method.POST, message);
  }
}
