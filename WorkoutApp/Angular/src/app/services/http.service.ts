import { Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Resource } from '../enums/resource';
import { Observable } from 'rxjs';
import {Method} from "../enums/method";
import {isNull} from "../utility";

@Injectable({
  providedIn: 'root'
})
export class HttpService {

  baseUrl: string = '';

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  request(resource: Resource, method: Method, payload?: any, additionURL: string = ''): Observable<any> {
    const apiURL = this.baseUrl + 'api/' + resource + additionURL;

    let options: Object = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
      })
    };

    if (payload) {
      options = Object.assign(options, {body: payload});
    }

    return this.http.request(method, apiURL, options);
  }

  requestWithFileUpload(resource: Resource, method: Method, payload: File, fileName?: string): Observable<any> {
    let formData = new FormData();
    const apiURL = this.baseUrl + 'api/' + resource;

    const name = isNull(fileName) ? payload.name : fileName;

    formData.set('file', payload, name);

    let options: Object = {
      body: formData
    };

    return this.http.request(method, apiURL, options);
  }
}
