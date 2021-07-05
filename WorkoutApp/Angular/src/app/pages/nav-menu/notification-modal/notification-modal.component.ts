import {Component, Inject} from '@angular/core';
import {NotificationModel} from "../../../models/NotificationModel";
import {UserModel} from "../../../models/UserModel";
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {TranslateService} from "@ngx-translate/core";
import {DomSanitizer} from "@angular/platform-browser";
import {NotificationType} from "../../../enums/notification";
import {getPicture} from "../../../utility";

@Component({
  selector: 'app-notification-modal',
  templateUrl: './notification-modal.component.html',
  styleUrls: ['./notification-modal.component.scss']
})
export class NotificationModalComponent  {

  notification: NotificationModel;
  sentByUser: UserModel;

  getPicture = getPicture;
  NotificationType = NotificationType;

  currentLanguage: string;

  constructor(
    @Inject(MAT_DIALOG_DATA) private data: any,
    private dialogRef: MatDialogRef<NotificationModalComponent>,
    private _translate: TranslateService, public sanitizer: DomSanitizer) {
    this.currentLanguage = data.language;
    this.notification = data.notification;
    this.sentByUser = this.notification.sentByUser;
  }

  accept() {
    this.data.acceptCallback();
  }

  decline() {
    this.data.declineCallback();
  }

  markAsRead() {
    this.data.markAsReadCallback();
  }
}
