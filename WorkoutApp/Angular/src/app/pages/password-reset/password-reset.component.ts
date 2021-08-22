import { Component } from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {AuthService} from "../../services/auth.service";
import {TranslateService} from "@ngx-translate/core";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {ToastrService} from "ngx-toastr";

@Component({
  selector: 'app-password-reset',
  templateUrl: './password-reset.component.html',
  styleUrls: ['./password-reset.component.scss']
})
export class PasswordResetComponent {

  passwordResetForm: FormGroup;

  constructor(private route: ActivatedRoute, private _auth: AuthService, private router: Router,
              private _translate: TranslateService, private fb: FormBuilder, private _toast: ToastrService) {

    this.passwordResetForm = fb.group({
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      passwordConfirm: ['', [Validators.required, Validators.minLength(6)]],
      userId: ['', Validators.required],
      token: ['', Validators.required]
    }, {
      validators: [
        validateMatchingControls('newPassword', 'passwordConfirm'),
      ]
    });

    route.queryParams.subscribe(params => {
      const userId = params.userId;
      const token = params.token;

      if (!(userId && token)) {
        router.navigate(['/home']);
      }

      this.passwordResetForm.get('userId').setValue(userId);
      this.passwordResetForm.get('token').setValue(token);
    });
  }

  submitPasswordResetForm(): void {
    if (this.passwordResetForm.invalid) {
      return;
    }

    this._auth.resetPassword(this.passwordResetForm.value)
      .subscribe(() => {
        this._toast.success(
          this._translate.instant('USER.SUCCESSFUL_PASSWORD_RESET'),
          this._translate.instant('GENERAL.INFO')
        );

        this.router.navigate(['/home']);
      }, () => {
        this._toast.error(
          this._translate.instant('USER.UNSUCCESSFUL_PASSWORD_RESET'),
          this._translate.instant('GENERAL.ERROR')
        );

        this.router.navigate(['/home']);
      });
  }
}

export function validateMatchingControls(controlName: string, matchingControlName: string) {
  return (formGroup: FormGroup) => {
    const control = formGroup.controls[controlName];
    const matchingControl = formGroup.controls[matchingControlName];

    if (matchingControl.errors && !matchingControl.errors.mustMatch) {
      return;
    }

    if (control.value !== matchingControl.value) {
      matchingControl.setErrors({mustMatch: true});
    } else {
      matchingControl.setErrors(null);
    }
  };
}
