import {Injectable} from "@angular/core";
import {HttpEvent, HttpHandler, HttpInterceptor, HttpRequest} from "@angular/common/http";
import {catchError} from "rxjs/operators";
import {Observable, throwError} from "rxjs";
import {StateService} from "./services/state.service";
import {Router} from "@angular/router";
import {ToastrService} from "ngx-toastr";
import {TranslateService} from "@ngx-translate/core";

@Injectable()
export class HttpErrorInterceptor implements HttpInterceptor {
  constructor(private router: Router, private _state: StateService,
              private _toast: ToastrService, private _translate: TranslateService) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (this.router.isActive('/home', true)) {
      return next.handle(request);
    }

    return next.handle(request).pipe(catchError(error => {
      if (error.status === 401) {
        this._state.user = null;

        this._toast.info(
          this._translate.instant('GENERAL.LOGIN_AGAIN'),
          this._translate.instant('GENERAL.INFO')
        );

        this.router.navigate(['/home']);
      }

      return throwError(error);
    }))
  }
}
