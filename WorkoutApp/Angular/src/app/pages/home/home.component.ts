import {Component, OnInit} from '@angular/core';
import {TranslateService} from "@ngx-translate/core";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {UserService} from "../../services/user.service";
import {StateService} from "../../services/state.service";
import {SignUpComponent} from "../sign-up/sign-up.component";
import {MatDialog} from "@angular/material/dialog";
import {ToastrService} from "ngx-toastr";
import {Router} from "@angular/router";
import {AuthService} from "../../services/auth.service";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit{
  loginForm: FormGroup;

  constructor(private fb: FormBuilder, private _auth: AuthService, private dialog: MatDialog,
              private _state: StateService, private _toast: ToastrService, private router: Router,
              public _translate: TranslateService) {
  }

  ngOnInit(): void {
    this._auth.signOut();

    this.loginForm = this.fb.group({
      userName: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  submitLoginForm() {
    if (this.loginForm.invalid) {
      return;
    }

    this._auth.login(this.loginForm.value)
      .subscribe(user => {
        this._state.user = user;
        this.router.navigate(['/dashboard']);
        }, () => {
        this._toast.error(
          this._translate.instant('USER_FORM.UNSUCCESSFUL_LOGIN'),
          this._translate.instant('GENERAL.ERROR'));
      });
  }

  openSignUpModal() {
    this.dialog.open(SignUpComponent, {
      width: '425px',
      height: '500px',
      disableClose: true
    });
  }
}
