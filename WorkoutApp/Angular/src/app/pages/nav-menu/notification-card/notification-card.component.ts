import {Component, Input} from '@angular/core';
import {NotificationModel} from "../../../models/NotificationModel";
import {NotificationModalComponent} from "../notification-modal/notification-modal.component";
import {MatDialog} from "@angular/material/dialog";
import {StateService} from "../../../services/state.service";
import {UserService} from "../../../services/user.service";
import {NotificationService} from "../../../services/notification.service";
import {NotificationCategory, NotificationType} from "../../../enums/notification";
import {getPicture} from '../../../utility';
import {DomSanitizer} from "@angular/platform-browser";

@Component({
  selector: 'app-notification-card',
  templateUrl: './notification-card.component.html',
  styleUrls: ['./notification-card.component.scss']
})
export class NotificationCardComponent {

  @Input() notifications: NotificationModel[];
  @Input() category: NotificationCategory;

  getPicture = getPicture;
  NotificationCategory = NotificationCategory;
  NotificationType = NotificationType;

  constructor(public sanitizer: DomSanitizer, private dialog: MatDialog, private _state: StateService,
              private _user: UserService, private _notification: NotificationService) { }

  acceptRequest(id: string): void {
    this._user.acceptFollowRequest(id)
      .subscribe(null);
  }

  declineRequest(id: string): void {
    this._user.declineFollowRequest(id)
      .subscribe(null);
  }

  markAsReadNotification(id: string): void {
    this._notification.delete(id)
      .subscribe(null);
  }

  openNotificationModal(notification: NotificationModel) {
    this.dialog.open(NotificationModalComponent, {
      width: '350px',
      data: {
        language: this._state.language.value,
        notification: notification,
        acceptCallback: () => {
          this.acceptRequest(notification.sentByUser.id);
        },
        declineCallback: () => {
          this.declineRequest(notification.sentByUser.id);
        },
        markAsReadCallback: () => {
          this.markAsReadNotification(notification.id);
        }
      }
    });
  }

  getNotificationType(type: NotificationType): string {
    return NotificationType[type];
  }
}
