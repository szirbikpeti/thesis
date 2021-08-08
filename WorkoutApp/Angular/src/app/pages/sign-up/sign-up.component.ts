import { Component, OnInit } from '@angular/core';
import {MatDialogRef} from "@angular/material/dialog";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {ToastrService} from "ngx-toastr";
import {TranslateService} from "@ngx-translate/core";
import {AuthService} from "../../services/auth.service";
import {FileService} from "../../services/file.service";
import {SignUpRequest} from "../../requests/SignUpRequest";

@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.scss']
})
export class SignUpComponent implements OnInit {
  signUpForm: FormGroup;

  constructor(private fb: FormBuilder, private _auth: AuthService, public _file: FileService,
              private _toast: ToastrService, private _translate: TranslateService,
              private dialogRef: MatDialogRef<SignUpComponent>) { }

  ngOnInit(): void {
    this.signUpForm = this.fb.group({
      fullName: ['', Validators.required],
      email: ['', [Validators.email, Validators.required]],
      userName: ['', Validators.required],
      password: ['', [Validators.minLength(6), Validators.required]],
      birthDay: [],
      gender: ['', Validators.required]
    });
  }

  uploadDefaultProfilePicture() {
    if (this.signUpForm.invalid) {
      return;
    }

    const that = this;
    const getFileBlob = function (url, cb) {
      const xhr = new XMLHttpRequest();
      xhr.open("GET", url);
      xhr.responseType = "blob";
      xhr.addEventListener('load', function() {
        cb(xhr.response);
      });
      xhr.send();
    };

    const blobToFile = function (blob, name) {
      blob.lastModifiedDate = new Date();
      blob.name = name;
      return blob;
    };

    const getFileObject = function(filePathOrUrl, cb) {
      getFileBlob(filePathOrUrl, function (blob) {
        cb(blobToFile(blob, 'profile-picture.png'));
      });
    };

    getFileObject(`../../../assets/avatar-${this.gender}.png`, function (fileObject) {
      that._file.upload(fileObject).subscribe(file => that.submitSignUpForm(file.id));
    });
  }

  private submitSignUpForm(fileId: string) {
    const signUpRequest: SignUpRequest = this.signUpForm.getRawValue();
    signUpRequest.profilePictureId = fileId;

    this._auth.signUp(signUpRequest)
      .subscribe(() => {
        this.dialogRef.close();

        this._toast.success(
          this._translate.instant('USER.SUCCESSFUL_SIGNUP'),
          this._translate.instant( 'GENERAL.INFO'));
      });
  }

  birthDayPickerFilter (d: Date | null): boolean {
    const today = new Date();
    const date = (d || today);

    const oneHundredYearsAgoDate = new Date(today.getFullYear() - 100, today.getMonth(), today.getDay());

    return date < today && date > oneHundredYearsAgoDate;
  }

  close() {
    this.dialogRef.close();
  }

  get gender(): string {
    return this.signUpForm.get('gender').value;
  }
}
