import {Injectable} from '@angular/core';
import {Observable} from "rxjs";
import {HttpService} from "./http.service";
import {Resource} from "../enums/resource";
import {Method} from "../enums/method";

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private _http: HttpService) {
  }

  signUp(request: any): Observable<any> {
    return this._http.request(Resource.SIGNUP, Method.POST, request)
  }

  login(request: any): Observable<any> {
    return this._http.request(Resource.LOGIN, Method.POST, request)
  }

  logout(): Observable<any> {
    return this._http.request(Resource.LOGOUT, Method.DELETE)
  }
}
