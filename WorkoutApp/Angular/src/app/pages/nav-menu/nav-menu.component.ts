import {Component} from '@angular/core';
import {TranslateService} from "@ngx-translate/core";
import {StateService} from "../../services/state.service";

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent {

  constructor(private _translate: TranslateService, private _state: StateService) {
  }

  currentLanguage(): string {
    return this._state.language.value;
  }

  switchLanguage(event: any): void {
    this._translate.use(event.value);
    this._state.language = event.value;
  }
}
