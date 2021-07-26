import {Component} from '@angular/core';
import {PostService} from "../../services/post.service";
import {PostModel} from "../../models/PostModel";
import {MatDialog} from "@angular/material/dialog";
import {NewPostComponent} from "./new-post/new-post.component";

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent {

  posts: PostModel[];

  constructor(private _post: PostService, private dialog: MatDialog) {
    _post.list().subscribe(posts => {
      console.log(posts);
      this.posts = posts
    });
  }

  openPostCreatorModal() {
    this.dialog.open(NewPostComponent, {
      width: '650px',
      disableClose: true,
      // data: {
      //   callback: () => this.deleteWorkout()
      // }
    });
  }
}
