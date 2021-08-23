import {Component, Input, OnInit} from '@angular/core';
import {PostModel} from "../../../models/PostModel";
import {LikeRequest} from "../../../requests/LikeRequest";
import {CommentRequest} from "../../../requests/CommentRequest";
import {getPicture} from "../../../utility";
import {DomSanitizer} from "@angular/platform-browser";
import {StateService} from "../../../services/state.service";
import {PostService} from "../../../services/post.service";
import {UserModel} from "../../../models/UserModel";
import {CommentModel} from "../../../models/CommentModel";
import {ConfirmationDialogComponent} from "../../confirmation-dialog/confirmation-dialog.component";
import {MatDialog} from "@angular/material/dialog";
import {LikeModalComponent} from "./like-modal/like-modal.component";
import {AuthService} from "../../../services/auth.service";

@Component({
  selector: 'app-post-card',
  templateUrl: './post-card.component.html',
  styleUrls: ['./post-card.component.scss']
})
export class PostCardComponent implements OnInit {

  @Input() filteredPosts: PostModel[];
  @Input() tabIndex: number;

  posts: PostModel[] = [];

  currentUser: UserModel;

  getPicture = getPicture;

  postAdditionData = [];
  commentAdditionData = new Map<string, boolean>();

  constructor(public sanitizer: DomSanitizer, public _state: StateService,
              public _auth: AuthService, private _post: PostService, private dialog: MatDialog) {
    this.currentUser = _state.user.value;
  }

  ngOnInit(): void {
    this.posts = this.filteredPosts;
    this.posts.forEach(post => {
      this.postAdditionData.push({postId: post.id, currentFileNumber: 0, isLastMediaFile: post.files.length === 1, isShowDetails: false});
      post.comments.forEach(comment => this.commentAdditionData.set(comment.id, false));
    });
  }

  private addLike(postId: string): void
  {
    this.posts.find(post => post.id === postId).likingUsers.push(this.currentUser);
  }

  private removeLike(postId: string): void
  {
    const index = this.posts.find(post => post.id === postId).likingUsers.indexOf(this.currentUser);
    this.posts.find(post => post.id === postId).likingUsers.splice(index, 1);
  }

  isLikedPost(post: PostModel): boolean {
    return post.likingUsers
      .map(({id}) => id)
      .includes(this.currentUser.id);
  }

  like(postId: string): void {
    this.addLike(postId);

    const likeRequest: LikeRequest = {postId: postId};
    this._post.addLike(likeRequest).subscribe(result => {
      this.posts.find(post => post.id === postId).likingUsers = result.likingUsers;
    }, () => this.removeLike(postId));
  }

  unlike(postId: string): void {
    this.removeLike(postId);

    const likeRequest: LikeRequest = {postId: postId};
    this._post.deleteLike(likeRequest).subscribe(result => {
      this.posts.find(post => post.id === postId).likingUsers = result.likingUsers;
    }, () => this.addLike(postId));
  }

  setShowDetails(index: number): void {
    this.postAdditionData[index].isShowDetails = !this.postAdditionData[index].isShowDetails;
  }

  addOrUpdateComment(postId: string, comment: HTMLInputElement, method: string, commentId?: string): void {
    if (!comment.value) {
      return;
    }

    const commentRequest: CommentRequest = {comment: comment.value};

    this._post[method + 'Comment'](commentId ?? postId, commentRequest).subscribe(result => {
      comment.value = '';
      this.posts.find(post => post.id === postId).comments = result.comments;
      result.comments.forEach(comment => this.commentAdditionData.set(comment.id, false));
    })
  }

  setCurrentFileNumber(index: number, operation: string): void {
    if (operation === '++') {
      this.postAdditionData[index].currentFileNumber++;
    } else {
      this.postAdditionData[index].currentFileNumber--;
    }

    this.postAdditionData[index].isLastMediaFile =
      this.posts[index].files.length === (this.postAdditionData[index].currentFileNumber + 1);
  }

  deletePost(post: PostModel): void {
    this.dialog.open(ConfirmationDialogComponent, {
      disableClose: true,
      data: {
        callback: () => {
          this._post.delete(post.id).subscribe(() => {
            const index = this.posts.indexOf(post);
            this.posts.splice(index, 1);
          });
        }
      }
    });
  }

  deleteComment(postId: string, commentId: string): void {
    this._post.deleteComment(postId, commentId).subscribe(result => {
      this.posts.find(post => post.id === postId).comments = result.comments;
      result.comments.forEach(comment => this.commentAdditionData.set(comment.id, false));
    });
  }

  setEditComment(comment: CommentModel, updateCommentInput?: HTMLInputElement): void {
    if (comment.user.id === this.currentUser.id) {
      this.commentAdditionData.set(comment.id, !this.commentAdditionData.get(comment.id));
    }

    if (updateCommentInput) {
      updateCommentInput.value = comment.comment;
    }
  }

  openLikeModal(likingUsers: UserModel[]): void {
    this.dialog.open(LikeModalComponent, {
      width: '350px',
      height: '450px',
      data: {
        users: likingUsers
      }
    });
  }
}
