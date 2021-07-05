import {Injectable} from '@angular/core';
import {HttpService} from "./http.service";
import {Resource} from "../enums/resource";
import {Method} from "../enums/method";
import {Observable} from "rxjs";
import {UserModel} from "../models/UserModel";
import {UserEditRequest} from "../requests/UserEditRequest";
import {FollowRequestAndFollowModel} from "../models/FollowRequestAndFollowModel";
import {FriendModel} from "../models/FriendModel";

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

  getFollowRequestsAndFollows(): Observable<FollowRequestAndFollowModel>{
    return this._http.request(Resource.USER_FOLLOW_REQUESTS_AND_FOLLOWS, Method.GET, null);
  }

  search(name: string): Observable<UserModel[]> {
    return this._http.request(Resource.USER, Method.GET, null, name);
  }

  followRequest(id: string): Observable<FollowRequestAndFollowModel> {
    return this._http.request(Resource.USER_REQUEST, Method.POST, null, id);
  }

  deleteFollowRequest(id: string, isDeletedByTargetUser: boolean): Observable<FollowRequestAndFollowModel> {
    return this._http.request(Resource.USER_REQUEST, Method.DELETE, isDeletedByTargetUser, id);
  }

  declineFollowRequest(id: string): Observable<FollowRequestAndFollowModel> {
    return this._http.request(Resource.USER_REQUEST, Method.PATCH, null, id);
  }

  acceptFollowRequest(id: string): Observable<FollowRequestAndFollowModel> {
    return this._http.request(Resource.USER_FOLLOW, Method.PATCH, null, id);
  }

  followBack(id: string): Observable<FollowRequestAndFollowModel> {
    return this._http.request(Resource.USER_FOLLOW, Method.POST, null, id);
  }

  unfollow(id: string): Observable<FollowRequestAndFollowModel> {
    return this._http.request(Resource.USER_FOLLOW, Method.DELETE, null, id);
  }

  getFriends(): Observable<FriendModel> {
    return this._http.request(Resource.USER_FRIENDS, Method.GET);
  }
}
