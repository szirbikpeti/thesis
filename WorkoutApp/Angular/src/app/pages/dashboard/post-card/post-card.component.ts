import {Component, Input, OnInit} from '@angular/core';
import {PostModel} from "../../../models/PostModel";
import {LikeRequest} from "../../../requests/LikeRequest";
import {CommentRequest} from "../../../requests/CommentRequest";
import {getPicture} from "../../../utility";
import {DomSanitizer} from "@angular/platform-browser";
import {StateService} from "../../../services/state.service";
import {PostService} from "../../../services/post.service";
import {UserModel} from "../../../models/UserModel";

@Component({
  selector: 'app-post-card',
  templateUrl: './post-card.component.html',
  styleUrls: ['./post-card.component.scss']
})
export class PostCardComponent implements OnInit{

  @Input() filteredPosts: PostModel[];
  @Input() tabIndex: number;

  posts: PostModel[] = [];

  currentUser: UserModel;

  getPicture = getPicture;

  postAdditionData = [];

  constructor(public sanitizer: DomSanitizer, private _state: StateService,
              private _post: PostService) {
    this.currentUser = _state.user.value;
  }

  ngOnInit(): void {
    this.posts = this.filteredPosts;
    this.posts.forEach(post => {
      this.postAdditionData.push({postId: post.id, currentFileNumber: 0, isLastMediaFile: post.files.length === 1, isShowDetails: false});
    });
  }

  isLikedPost(post: PostModel): boolean {
    return post.likedUsers
      .map(({id}) => id)
      .includes(this.currentUser.id);
  }

  like(id: string): void {
    const likeRequest: LikeRequest = {postId: id};
    this._post.addLike(likeRequest).subscribe(result => {
      this.posts.find(post => post.id === id).likedUsers = result.likedUsers;
    });
  }

  unlike(id: string): void {
    const likeRequest: LikeRequest = {postId: id};
    this._post.deleteLike(likeRequest).subscribe(result => {
      this.posts.find(post => post.id === id).likedUsers = result.likedUsers;
    });
  }

  setShowDetails(index: number): void {
    this.postAdditionData[index].isShowDetails = !this.postAdditionData[index].isShowDetails;
  }

  addComment(postId: string, comment: HTMLInputElement): void {
    if (!comment.value) {
      return;
    }

    const commentRequest: CommentRequest = {comment: comment.value};

    this._post.createComment(postId, commentRequest).subscribe(result => {
      comment.value = '';
      this.posts.find(post => post.id === postId).comments = result.comments;
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
    this._post.delete(post.id)
      .subscribe(() => {
        const index = this.posts.indexOf(post);
        this.posts.splice(index, 1);
      });
  }

  deleteComment(postId: string, commentId: string) {
    this._post.deleteComment(postId, commentId).subscribe(result => {
      this.posts.find(post => post.id === postId).comments = result.comments;
    });
  }
}
