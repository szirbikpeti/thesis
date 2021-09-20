import {Component, HostListener} from '@angular/core';
import {AdminService} from "../../services/admin.service";
import {UserModel} from "../../models/UserModel";
import {ToastrService} from "ngx-toastr";
import {TranslateService} from "@ngx-translate/core";
import {MatTableDataSource} from "@angular/material/table";
import {getPicture} from "../../utility";
import {StateService} from "../../services/state.service";
import {DomSanitizer} from "@angular/platform-browser";

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent {

  userDataSource: MatTableDataSource<UserModel>;

  displayedColumns: string[];

  getPicture = getPicture;

  constructor(private _admin: AdminService, private _toast: ToastrService,
              private _translate: TranslateService, public _state: StateService,
              public sanitizer: DomSanitizer) {
    this.displayedColumns = UsersComponent.getColumnsToDisplay();

    _admin.listUsers()
      .subscribe(users =>
        this.userDataSource = new MatTableDataSource<UserModel>(users));
  }

  isDate(cellValue: string) {
    return Date.parse(cellValue);
  }

  blockUser(id: string): void {
    this._admin.blockUser(id)
      .subscribe(() => {
        this.userDataSource.data
          .filter(user => user.id === id)[0].isBlocked = true;

        this._toast.success(
          this._translate.instant('USER.SUCCESSFUL_BLOCK'),
          this._translate.instant('GENERAL.INFO')
        );
      });
  }

  restoreUser(id: string): void {
    this._admin.restoreUser(id)
      .subscribe(() => {
        this.userDataSource.data
          .filter(user => user.id === id)[0].isBlocked = false;

        this._toast.success(
          this._translate.instant('USER.SUCCESSFUL_RESTORE'),
          this._translate.instant('GENERAL.INFO')
        );
      });
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
    this.displayedColumns = UsersComponent.getColumnsToDisplay();
  }

}
