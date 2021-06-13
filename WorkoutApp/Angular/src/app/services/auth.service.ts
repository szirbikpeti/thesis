import {Injectable} from '@angular/core';
import {HttpService} from "./http.service";
import {StateService} from "./state.service";
import {Resource} from "../enums/resource";
import {Method} from "../enums/method";
import {Observable} from "rxjs";
import {UserModel} from "../models/UserModel";
import {SignUpRequest} from "../requests/SignUpRequest";
import {LoginRequest} from "../requests/LoginRequest";

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private _http: HttpService, private _state: StateService) {
  }

  signUp(request: SignUpRequest): Observable<any> {
    return this._http.request(Resource.SIGNUP, Method.POST, request);
  }

  signIn(request: LoginRequest): Observable<UserModel> {
    return this._http.request(Resource.LOGIN, Method.POST, request);
  }

  async signOut(): Promise<void> {
    await this._http.request(Resource.LOGOUT, Method.DELETE).toPromise();
  }

  hasPermission(permission: string): boolean {
    return this._state.user.value.permissions.includes(permission);
  }
}
