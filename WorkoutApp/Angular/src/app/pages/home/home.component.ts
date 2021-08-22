import {Component} from '@angular/core';
import {TranslateService} from "@ngx-translate/core";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {StateService} from "../../services/state.service";
import {SignUpComponent} from "../sign-up/sign-up.component";
import {MatDialog} from "@angular/material/dialog";
import {ToastrService} from "ngx-toastr";
import {Router} from "@angular/router";
import {AuthService} from "../../services/auth.service";
import {LoginRequest} from "../../requests/LoginRequest";
import {HttpErrorResponse} from "@angular/common/http";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent {
  loginForm: FormGroup;
  forgotPasswordForm: FormGroup;

  isLoginFormActive: boolean = true;

  constructor(private fb: FormBuilder, private _auth: AuthService, private dialog: MatDialog,
              private _state: StateService, private _toast: ToastrService, private router: Router,
              public _translate: TranslateService) {
    this.loginForm = this.fb.group({
      userName: ['', Validators.required],
      password: ['', Validators.required],
    });

    this.forgotPasswordForm = this.fb.group({
      userName: ['', Validators.required],
    });
  }

  submitLoginForm(): void {
    if (this.loginForm.invalid) {
      return;
    }

    const loginRequest: LoginRequest = this.loginForm.value;

    this._auth.signIn(loginRequest)
      .subscribe(user => {
        this._state.user = user;
        this.router.navigate(['/dashboard']);
        }, (error: HttpErrorResponse) => {
          if (error.status === 400) {
            this._toast.error(
              this._translate.instant('USER.WRONG_USERNAME_OR_PASSWORD'),
              this._translate.instant('GENERAL.ERROR'));

            return;
          }

          if (error.status === 401) {
            const mm = this._toast.warning(
              this._translate.instant('USER.NOT_CONFIRMED_EMAIL') + '<br><br>' + this._translate.instant('USER.RESEND_EMAIL'),
              this._translate.instant('GENERAL.WARNING'), {
                disableTimeOut: true,
                closeButton: true,
                enableHtml: true,
              });

            mm.onTap.subscribe(() => {
              this._auth.resendEmail(loginRequest.userName).subscribe(null);
            });

            return;
          }

        if (error.status === 403) {
          const errorMessage: string = error.error;

          const openBracketIndex = errorMessage.indexOf('(');
          const closeBracketIndex = errorMessage.indexOf(')');

          const minute = errorMessage.substring(openBracketIndex + 1, closeBracketIndex);

          this._toast.error(
            this._translate.instant('USER.LOCKED_OUT', { minute: minute }),
            this._translate.instant('GENERAL.ERROR'));

          return;
        }

        this._toast.error(
          this._translate.instant('USER.UNSUCCESSFUL_LOGIN'),
          this._translate.instant('GENERAL.ERROR'));
      });
  }

  submitForgotPasswordForm(): void {
    if (this.forgotPasswordForm.invalid) {
      return;
    }

    this._auth.forgotPassword(this.forgotPasswordForm.value)
      .subscribe(() => {
        this.isLoginFormActive = true;

        this._toast.success(
          this._translate.instant('USER.RESET_PASSWORD_EMAIL'),
          this._translate.instant('GENERAL.INFO')
        );
      });
  }

  openSignUpModal(): void {
    this.dialog.open(SignUpComponent, {
      width: '425px',
      height: '510px',
      disableClose: true
    });
  }

  currentLanguage(): string {
    return this._state.language.value;
  }

  switchLanguage(language: any): void {
    this._translate.use(language);
    this._state.language = language;
  }
}
