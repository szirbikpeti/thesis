import { Component } from '@angular/core';
import { AngularFaviconService } from 'angular-favicon';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
  title = 'app';

  constructor(private fav: AngularFaviconService) {
    fav.setFavicon('./assets/favicon.ico');
  }
}
