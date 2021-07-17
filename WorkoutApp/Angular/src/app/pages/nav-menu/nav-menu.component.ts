import {Component, Inject, ViewChild} from '@angular/core';
import {TranslateService} from "@ngx-translate/core";
import {StateService} from "../../services/state.service";
import {MatSidenav} from "@angular/material/sidenav";
import {Observable} from "rxjs";
import {BreakpointObserver, Breakpoints} from "@angular/cdk/layout";
import {map, shareReplay} from "rxjs/operators";
import {AuthService} from "../../services/auth.service";
import {Router} from "@angular/router";
import {UserModel} from "../../models/UserModel";
import {getPicture, isNull} from '../../utility';
import {DomSanitizer} from "@angular/platform-browser";
import {UserService} from "../../services/user.service";
import * as signalR from '@microsoft/signalr';
import {NotificationService} from "../../services/notification.service";
import {NotificationModel} from "../../models/NotificationModel";
import {NotificationType} from "../../enums/notification";
import {MatDialog} from "@angular/material/dialog";
import {NotificationModalComponent} from "./notification-modal/notification-modal.component";

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent {

  @ViewChild('drawer') public sideNav: MatSidenav;

  isHandset: boolean;

  isNull = isNull;
  getPicture = getPicture;

  isHandset$: Observable<boolean> = this.breakpointObserver.observe(Breakpoints.Handset)
    .pipe(map(result => result.matches), shareReplay());

  currentUser: UserModel;
  authenticationConfirmed: boolean = true;

  notifications: NotificationModel[];
  followNotifications: NotificationModel[];

  constructor(private breakpointObserver: BreakpointObserver, public _auth: AuthService, private _notification: NotificationService,
              private _user: UserService, private _translate: TranslateService, @Inject('BASE_URL') baseUrl: string,
              private _state: StateService, private dialog: MatDialog, public router: Router, public sanitizer: DomSanitizer) {
    _state.user.subscribe(storedUser => this.currentUser = storedUser);
    this.isHandset$.subscribe((next) => this.isHandset = next);
    this.getNotifications();

    const connection = new signalR.HubConnectionBuilder()
      .configureLogging(signalR.LogLevel.Information)
      .withUrl(baseUrl + "notify")
      .build();

    connection.start()
      .then(() => console.log('SignalR connected!'))
      .catch(err => console.error(err.toString()));

    connection.on("BroadcastFollowNotifications", () => this.getNotifications());
  }

  private getNotifications(): void {
    this._notification.get().subscribe(notis => {
      // TODO - delete is not working right now, console.log(notis);

      this.notifications = notis
        .filter(_ => !this.getNotificationType(_.type).toLowerCase().includes('follow'));

      this.followNotifications = notis
        .filter(_ => this.getNotificationType(_.type).toLowerCase().includes('follow')
          && isNull(_.deletedOn));
    });
  }

  currentLanguage(): string {
    return this._state.language.value;
  }

  switchLanguage(language: any): void {
    this._translate.use(language);
    this._state.language = language;
  }

  toggleDrawerWhenHandset(): void {
    if (this.isHandset) {
      this.sideNav.toggle();
    }
  }

  signOut(): void {
    this._state.user = null;
    this._auth.signOut();
    this.router.navigate(['/']);
  }

  toggleSideNav(): void {
    this.sideNav.toggle();
  }

  editProfile(): void {
    this.toggleDrawerWhenHandset();
    this.router.navigate(['/profile']);
  }

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

  isSideNavOpened(): boolean {
    return this.sideNav?.opened ?? window.innerWidth > 960;
  }

  getFollowNotificationCount(): number {
    return this.followNotifications?.length ?? 0;
  }

  openProfileModal(notification: NotificationModel) {
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
