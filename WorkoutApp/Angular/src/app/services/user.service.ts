import {Injectable} from '@angular/core';
import {HttpService} from "./http.service";
import {Resource} from "../enums/resource";
import {Method} from "../enums/method";
import {Observable} from "rxjs";
import {UserModel} from "../models/UserModel";
import {UserEditRequest} from "../requests/UserEditRequest";

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private _http: HttpService) {
  }

  update(userData: UserEditRequest): Observable<UserModel> {
    return this._http.request(Resource.USER, Method.PUT, userData);
  }

  delete(): Observable<any> {
    return this._http.request(Resource.USER, Method.DELETE);
  }

  search(name: string): Observable<UserModel[]> {
    return this._http.request(Resource.USER, Method.GET, null, name);
  }

  follow(id: string): Observable<UserModel> {
    return this._http.request(Resource.USER, Method.POST, null, id);
  }

  undoFollow(id: string): Observable<UserModel> {
    return this._http.request(Resource.USER, Method.DELETE, null, id);
  }
}
