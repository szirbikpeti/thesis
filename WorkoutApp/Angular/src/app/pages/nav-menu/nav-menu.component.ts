import {Component, ViewChild} from '@angular/core';
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

  constructor(private breakpointObserver: BreakpointObserver, public _auth: AuthService,
              private _translate: TranslateService, private _state: StateService,
              public router: Router, public sanitizer: DomSanitizer) {
    _state.user.subscribe(storedUser => this.currentUser = storedUser);
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

  toggleSideNav() {
    if (window.innerWidth > 950) {
      document.getElementsByClassName('menu-button')[0]
        .setAttribute("style", "left: " + (!this.sideNav.opened ? 325 : 40) + "px");
    }
    this.sideNav.toggle();
  }

  editProfile(): void {
    this.router.navigate(['/profile']);
  }
}
