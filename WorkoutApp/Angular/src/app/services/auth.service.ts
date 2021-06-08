import {Injectable} from '@angular/core';
import {HttpService} from "./http.service";
import {StateService} from "./state.service";
import {Resource} from "../enums/resource";
import {Method} from "../enums/method";
import {Observable} from "rxjs";
import {UserModel} from "../models/UserModel";

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private _http: HttpService, private _state: StateService) {
  }

  signUp(request: any): Observable<any> {
    return this._http.request(Resource.SIGNUP, Method.POST, request);
  }

  login(request: any): Observable<UserModel> {
    return this._http.request(Resource.LOGIN, Method.POST, request);
  }

  async signOut() {
    await this._http.request(Resource.LOGOUT, Method.DELETE).toPromise();
  }

  hasPermission(permission: string): boolean {
    return this._state.user.value.permissions.includes(permission);
  }
}
