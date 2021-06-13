import {Injectable} from '@angular/core';
import {HttpService} from "./http.service";
import {Resource} from "../enums/resource";
import {Observable} from "rxjs";
import {FileModel} from "../models/FileModel";
import {Method} from "../enums/method";

@Injectable({
  providedIn: 'root'
})
export class FileService {

  constructor(private _http: HttpService) {
  }

  upload(file: File, fileName?: string): Observable<FileModel> {
    return this._http.requestWithFileUpload(Resource.FILE, Method.POST, file, fileName);
  }

  uploadProfilePicture(file: File, fileName?: string): Observable<FileModel> {
    return this._http.requestWithFileUpload(Resource.FILE, Method.PATCH, file, fileName);
  }
}
