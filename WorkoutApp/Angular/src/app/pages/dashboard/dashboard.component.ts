import {Component} from '@angular/core';
import {PostService} from "../../services/post.service";
import {PostModel} from "../../models/PostModel";
import {MatDialog} from "@angular/material/dialog";
import {NewPostComponent} from "./new-post/new-post.component";
import {UserModel} from "../../models/UserModel";
import {StateService} from "../../services/state.service";

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent {

  currentUser: UserModel;

  posts: PostModel[];

  constructor(private _post: PostService, private _state: StateService, private dialog: MatDialog) {
    this.currentUser = _state.user.value;

    _post.list().subscribe(posts => this.posts = posts);
  }

  openPostCreatorModal(): void {
    this.dialog.open(NewPostComponent, {
      width: '650px',
      disableClose: true
    });
  }

  getFriendsPosts(): PostModel[] {
    return this.posts.filter(post => post.user.id !== this.currentUser.id);
  }

  getMyPosts(): PostModel[] {
    return this.posts.filter(post => post.user.id === this.currentUser.id);
  }
}
