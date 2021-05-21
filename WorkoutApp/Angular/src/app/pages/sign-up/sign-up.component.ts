import { Component, OnInit } from '@angular/core';
import {MatDialogRef} from "@angular/material/dialog";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {UserService} from "../../services/user.service";
import {ToastrService} from "ngx-toastr";
import {TranslateService} from "@ngx-translate/core";

@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.scss']
})
export class SignUpComponent implements OnInit {
  signUpForm: FormGroup;

  constructor(private fb: FormBuilder, private _user: UserService,
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
    console.log(this.signUpForm.value);
    this._user.signUp(this.signUpForm.value)
      .subscribe(() => {
        this._toast.success(this._translate.instant('USER_FORM.SUCCESSFUL_SIGNUP', 'GENERAL.INFO'));
        close();
      });
  }

  close() {
    this.dialogRef.close();
  }
}
