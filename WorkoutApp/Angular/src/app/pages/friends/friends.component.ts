import {Component, HostListener} from '@angular/core';
import {UserService} from "../../services/user.service";
import {FriendModel} from "../../models/FriendModel";
import {MatTableDataSource} from "@angular/material/table";
import {UserModel} from "../../models/UserModel";
import {getPicture} from "../../utility";
import {DomSanitizer} from "@angular/platform-browser";
import {animate, state, style, transition, trigger} from '@angular/animations';
import {TranslateService} from "@ngx-translate/core";
import {StateService} from "../../services/state.service";

@Component({
  selector: 'app-friends',
  templateUrl: './friends.component.html',
  styleUrls: ['./friends.component.scss'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({height: '0px', minHeight: '0'})),
      state('expanded', style({height: '*'})),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ],
})
export class FriendsComponent {

  friends: FriendModel;

  followerDataSource: MatTableDataSource<UserModel>;
  followedDataSource: MatTableDataSource<UserModel>;

  displayedColumns: string[];
  expandedElement: UserModel | null;

  getPicture = getPicture;

  isDisabledRipple: boolean = false;

  constructor(private _user: UserService, private _translate: TranslateService,
              public _state: StateService, public sanitizer: DomSanitizer) {
    this.displayedColumns = FriendsComponent.getColumnsToDisplay();

    this.getFriends();
  }

  private getFriends(): void {
    this._user.getFriends().subscribe(friends => {
      this.friends = friends;
      console.log(friends);
      this.followerDataSource = new MatTableDataSource<UserModel>(friends.followerUsers);
      this.followedDataSource = new MatTableDataSource<UserModel>(friends.followedUsers);
    });
  }

  unfollow(id: string): void {
    this._user.unfollow(id).subscribe(() => this.getFriends());
  }

  followBack(id: string): void {
    this._user.followBack(id).subscribe(() => this.getFriends());
  }

  isDate(cellValue: string) {
    return Date.parse(cellValue);
  }

  private static getColumnsToDisplay(): string[] {
    if (window.innerWidth < 420) {
      return ['profilePicture', 'userName', 'operation'];
    }

    if (window.innerWidth < 545) {
      return ['profilePicture', 'fullName', 'userName', 'operation'];
    }

    if (window.innerWidth < 1050) {
      return ['profilePicture', 'fullName', 'userName', 'lastSignedInOn', 'operation'];
    }

    if (window.innerWidth > 1050) {
      return ['profilePicture', 'fullName', 'userName', 'email', 'lastSignedInOn', 'operation'];
    }
  }

  @HostListener('window:resize')
  onResize() {
    this.displayedColumns = FriendsComponent.getColumnsToDisplay();
  }
}
