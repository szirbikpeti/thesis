import {NgModule} from '@angular/core';
import {HttpClient, HttpClientModule} from "@angular/common/http";
import {TranslateModule, TranslateLoader, TranslateService} from "@ngx-translate/core";
import {TranslateHttpLoader} from "@ngx-translate/http-loader";
import {StateService} from "../services/state.service";

@NgModule({
  declarations: [],
  imports: [
    HttpClientModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: translateLoaderFactory,
        deps: [HttpClient]
      }
    }),
  ],
  exports: [TranslateModule]
})
export class I18nModule {
  constructor(_translate: TranslateService, private _state: StateService) {
    _translate.addLangs(['hu', 'en']);

    if (_state.language.value === null) {
      _state.language = 'hu';
    }

    _translate.use(_state.language.value);
  }
}

export function translateLoaderFactory(httpClient: HttpClient): TranslateHttpLoader {
  return new TranslateHttpLoader(httpClient);
}
