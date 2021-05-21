import { Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Resource } from '../enums/resource';
import { Observable } from 'rxjs';
import {Method} from "../enums/method";

@Injectable({
  providedIn: 'root'
})
export class HttpService {

  baseUrl: string = '';

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  request(resource: Resource, method: Method, payload?: any, id: string = ''): Observable<any> {
    const apiURL = this.baseUrl + 'api/' + resource + id;

    let options: Object = {
      headers: new HttpHeaders({
        // 'Authorization': 'Bearer ' + this._state.token.valu
      })
    };

    if (payload) {
      options = Object.assign(options, {body: payload});
    }

    return this.http.request(method, apiURL, options);
  }

  requestWithFormData(resource: Resource, payload: File, url: string): Observable<any> {
    let formData = new FormData();
    const apiURL = this.baseUrl + 'api/' + resource + url;

    formData.set('picture', payload, 'profile-picture.png');

    let options: Object = {
      headers: new HttpHeaders({
        // 'Authorization': 'Bearer ' + this._state.token.value
      })
    };

    return this.http.put(apiURL, formData, options);
  }
}
