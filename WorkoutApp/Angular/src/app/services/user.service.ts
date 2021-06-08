import {Injectable} from '@angular/core';
import {HttpService} from "./http.service";
import {Resource} from "../enums/resource";
import {Method} from "../enums/method";

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private _http: HttpService) {
  }

  update(user: any): any {  // TODO
    return this._http.request(Resource.USER, Method.PUT);
  }
}
