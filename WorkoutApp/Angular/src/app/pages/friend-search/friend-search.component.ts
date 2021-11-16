import {Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {UserService} from "../../services/user.service";
import {UserModel} from "../../models/UserModel";
import {StateService} from "../../services/state.service";
import {getPicture} from "../../utility";
import {DomSanitizer} from "@angular/platform-browser";
import {FollowRequestAndFollowModel} from "../../models/FollowRequestAndFollowModel";
import {MatPaginator} from "@angular/material/paginator";
import {MatTableDataSource} from "@angular/material/table";
import {Observable} from "rxjs";

@Component({
  selector: 'app-friend-search',
  templateUrl: './friend-search.component.html',
  styleUrls: ['./friend-search.component.scss']
})
export class FriendSearchComponent implements OnInit, OnDestroy {
  @ViewChild(MatPaginator) paginator: MatPaginator;
  dataSource: MatTableDataSource<UserModel>;
  users: Observable<UserModel[]>;

  friendSearchForm: FormGroup;

  currentFollowRequestsAndFollows: FollowRequestAndFollowModel;

  getPicture = getPicture;

  constructor(private fb: FormBuilder, private _user: UserService, private _state: StateService,
              public sanitizer: DomSanitizer) {
    this._user.search('bok')
      .subscribe(foundedUsers => {
        this.dataSource = new MatTableDataSource<UserModel>(foundedUsers);
        this.dataSource.paginator = this.paginator;

        this.users = this.dataSource.connect();
      });

    this._user.getFollowRequestsAndFollows()
      .subscribe(frf => this.currentFollowRequestsAndFollows = frf);
  }

  ngOnInit(): void {
    this.friendSearchForm = this.fb.group({
      name: ['', Validators.required]
    });
  }

  ngOnDestroy() {
    if (this.dataSource) {
      this.dataSource.disconnect();
    }
  }

  submitFriendSearchForm(): void {
    this._user.search(this.name)
      .subscribe(foundedUsers => this.dataSource.data = foundedUsers);
  }

  follow(id: string) {
    this._user.followRequest(id)
      .subscribe(frf => this.currentFollowRequestsAndFollows = frf);
  }

  deleteFollowRequest(id: string, isDeletedByTargetUser = false) {
    this._user.deleteFollowRequest(id, isDeletedByTargetUser)
      .subscribe(frf => this.currentFollowRequestsAndFollows = frf);
  }

  acceptFollowRequest(id: string) {
    this._user.acceptFollowRequest(id)
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

  declineFollowRequest(id: string): void {
    this._user.declineFollowRequest(id)
      .subscribe(null);
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

  isBlockedBySourceUser(id: string): boolean {
    return this.currentFollowRequestsAndFollows.sourceUsers
      .find(_ => _.id == id)
      ?.isBlocked ?? false;
  }

  isBlockedByTargetUser(id: string): boolean {
    return this.currentFollowRequestsAndFollows.targetUsers
      .find(_ => _.id == id)
      ?.isBlocked ?? false;
  }

  get name(): string {
    return this.friendSearchForm.get('name').value;
  }
}
