import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {TranslateService} from "@ngx-translate/core";
import {DomSanitizer} from "@angular/platform-browser";
import {UserModel} from "../../../../models/UserModel";
import {getPicture} from "../../../../utility";
import {UserService} from "../../../../services/user.service";
import {FollowRequestAndFollowModel} from "../../../../models/FollowRequestAndFollowModel";
import {StateService} from "../../../../services/state.service";

@Component({
  selector: 'app-like-modal',
  templateUrl: './like-modal.component.html',
  styleUrls: ['./like-modal.component.scss']
})
export class LikeModalComponent {

  users: UserModel[];
  currentUser: UserModel;
  currentFollowRequestsAndFollows: FollowRequestAndFollowModel;

  getPicture = getPicture;

  constructor(@Inject(MAT_DIALOG_DATA) private data: any, private _state: StateService,
              private dialogRef: MatDialogRef<LikeModalComponent>, private _user: UserService,
              private _translate: TranslateService, public sanitizer: DomSanitizer) {
    this.users = data.users;
    this.currentUser = _state.user.value;

    this._user.getFollowRequestsAndFollows()
      .subscribe(frf => this.currentFollowRequestsAndFollows = frf);
  }

  isCurrentUser(id: string): boolean {
    return this.currentUser.id === id;
  }

  isSourceUser(id: string): boolean {
    return this.currentFollowRequestsAndFollows?.sourceUsers
      .map(({id}) => id)
      .includes(id) || false;
  }

  isTargetUser(id: string): boolean {
    return this.currentFollowRequestsAndFollows?.targetUsers
      .map(({id}) => id)
      .includes(id) || false;
  }

  isFollowerUser(id: string): boolean {
    return this.currentFollowRequestsAndFollows?.followerUserIds.includes(id) || false;
  }

  isFollowedUser(id: string): boolean {
    return this.currentFollowRequestsAndFollows?.followedUserIds.includes(id) || false;
  }

  follow(id: string) {
    this._user.followRequest(id)
      .subscribe(frf => this.currentFollowRequestsAndFollows = frf);
  }

  followBack(id: string) {
    this._user.followBack(id)
      .subscribe(frf => this.currentFollowRequestsAndFollows = frf);
  }

  unfollow(id: string) {
    this._user.unfollow(id)
      .subscribe(frf => this.currentFollowRequestsAndFollows = frf);
  }
}
