import { Component, OnInit } from '@angular/core';
import {MatDialogRef} from "@angular/material/dialog";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {ToastrService} from "ngx-toastr";
import {TranslateService} from "@ngx-translate/core";
import {AuthService} from "../../services/auth.service";

@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.scss']
})
export class SignUpComponent implements OnInit {
  signUpForm: FormGroup;

  constructor(private fb: FormBuilder, private _auth: AuthService,
              private _toast: ToastrService, private _translate: TranslateService,
              private dialogRef: MatDialogRef<SignUpComponent>) { }

  ngOnInit(): void {
    this.signUpForm = this.fb.group({
      fullName: ['', Validators.required],
      email: ['', [Validators.email, Validators.required]],
      userName: ['', Validators.required],
      password: ['', Validators.required],
      birthDay: []
    });
  }

  submitSignUpForm() {
    this._auth.signUp(this.signUpForm.value)
      .subscribe(() => {
        close();

        this._toast.success(
          this._translate.instant('USER_FORM.SUCCESSFUL_SIGNUP'),
          this._translate.instant( 'GENERAL.INFO'));
      });
  }

  close() {
    this.dialogRef.close();
  }
}
