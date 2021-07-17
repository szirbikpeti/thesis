import { Injectable } from '@angular/core';
import {HttpService} from "./http.service";
import {Observable} from "rxjs";
import {Resource} from "../enums/resource";
import {Method} from "../enums/method";
import {PostModel} from "../models/PostModel";
import {PostRequest} from "../requests/PostRequest";
import {CommentRequest} from "../requests/CommentRequest";
import {LikeRequest} from "../requests/LikeRequest";

@Injectable({
  providedIn: 'root'
})
export class PostService {

  constructor(private _http: HttpService) {
  }

  list(): Observable<PostModel[]> {
    return this._http.request(Resource.POST, Method.GET);
  }

  get(id: string) {
    return this._http.request(Resource.POST, Method.GET, null, id);
  }

  create(post: PostRequest) {
    return this._http.request(Resource.POST, Method.POST, post);
  }

  createComment(postId: string, comment: CommentRequest): Observable<PostModel>  {
    return this._http.request(Resource.POST, Method.POST, comment, `${postId}/comment`);
  }

  addLike(like: LikeRequest): Observable<PostModel> {
    return this._http.request(Resource.POST, Method.POST, like, 'like');
  }

  updateComment(commentId: string, comment: CommentRequest): Observable<PostModel> {
    return this._http.request(Resource.POST, Method.PUT, comment, commentId);
  }

  delete(postId: string) {
    return this._http.request(Resource.POST, Method.DELETE, null, postId);
  }

  deleteComment(postId: string, commentId: string): Observable<PostModel> {
    return this._http.request(Resource.POST, Method.DELETE, null, `${postId}/comment/${commentId}`);
  }

  deleteLike(like: LikeRequest): Observable<PostModel> {
    return this._http.request(Resource.POST, Method.DELETE, like, 'like');
  }
}
