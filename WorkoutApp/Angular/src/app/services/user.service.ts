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

  followRequest(id: string): Observable<UserModel> {
    return this._http.request(Resource.USER_REQUEST, Method.POST, null, id);
  }

  deleteFollowRequest(id: string): Observable<UserModel> {
    return this._http.request(Resource.USER_REQUEST, Method.DELETE, null, id);
  }

  declineFollowRequest(id: string): Observable<UserModel> {
    return this._http.request(Resource.USER_REQUEST, Method.PATCH, null, id);
  }

  acceptFollowRequest(id: string): Observable<UserModel> {
    return this._http.request(Resource.USER_FOLLOW, Method.PATCH, null, id);
  }

  followBack(id: string): Observable<UserModel> {
    return this._http.request(Resource.USER_FOLLOW, Method.POST, null, id);
  }

  unfollow(id: string): Observable<UserModel> {
    return this._http.request(Resource.USER_FOLLOW, Method.DELETE, null, id);
  }

}
