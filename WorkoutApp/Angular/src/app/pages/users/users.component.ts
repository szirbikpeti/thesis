import { Component } from '@angular/core';
import {AdminService} from "../../services/admin.service";
import {UserModel} from "../../models/UserModel";
import {ToastrService} from "ngx-toastr";
import {TranslateService} from "@ngx-translate/core";

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent {

  users: UserModel[];

  constructor(private _admin: AdminService, private _toast: ToastrService,
              private _translate: TranslateService) {
    _admin.listUsers()
      .subscribe(users => this.users = users);
  }

  blockUser(id: string): void {
    this._admin.blockUser(id)
      .subscribe(() => {
        this._toast.success(
          this._translate.instant('USER.SUCCESSFUL_BLOCK'),
          this._translate.instant('GENERAL.INFO')
        );
      });
  }

  restoreUser(id: string): void {
    this._admin.restoreUser(id)
      .subscribe(() => {
        this._toast.success(
          this._translate.instant('USER.SUCCESSFUL_RESTORE'),
          this._translate.instant('GENERAL.INFO')
        );
      });
  }

}
