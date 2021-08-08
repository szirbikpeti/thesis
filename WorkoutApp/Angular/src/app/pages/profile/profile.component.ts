import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {StateService} from "../../services/state.service";
import {UserModel} from "../../models/UserModel";
import {UserService} from "../../services/user.service";
import {ActivatedRoute, Router} from "@angular/router";
import {DomSanitizer} from "@angular/platform-browser";
import {getPicture, isNull} from '../../utility';
import {FileService} from "../../services/file.service";
import {ToastrService} from "ngx-toastr";
import {TranslateService} from "@ngx-translate/core";
import {ConfirmationDialogComponent} from "../confirmation-dialog/confirmation-dialog.component";
import {MatDialog} from "@angular/material/dialog";

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {

  @ViewChild('fileInput') fileInput: ElementRef;

  wrong_old_pw: string = '';  // TODO

  userForm: FormGroup;

  currentUser: UserModel;

  profilePicture: File;
  newProfilePicture: any;

  getPicture = getPicture;
  isNull = isNull;

  constructor(private _user: UserService, private _file: FileService,private fb: FormBuilder, private router: Router,
              private route: ActivatedRoute, private _state: StateService, public sanitizer: DomSanitizer,
              private _toast: ToastrService, private _translate: TranslateService, private dialog: MatDialog) {
    this.currentUser = _state.user.value;
  }

  ngOnInit(): void {
    this.userForm = this.fb.group({
      fullName: [this.currentUser.fullName, Validators.required],
      userName: [{value: this.currentUser.userName, disabled: true}, Validators.required],
      email: [this.currentUser.email, Validators.required],
      about: [this.currentUser.about],
      birthday: [this.currentUser.birthday],
      profilePictureId: [this.currentUser.profilePicture.id],
      passwordChange: this.fb.group({
        oldPassword: [''],
        newPassword: ['']
      })
    });
  }

  uploadProfilePicture(): void {
    if (!isNull(this.newProfilePicture)) {
      this._file.uploadProfilePicture(this.profilePicture, 'profile-picture.png').subscribe(file => {
        this.submitUserForm(file.id);
      });
    } else {
      this.submitUserForm();
    }
  }

  private submitUserForm(fileId?: string): void {

    if (!isNull(fileId)) {
      this.profilePictureId.setValue(fileId);
    }

    this._user.update(this.userForm.value).subscribe(user => {
      this._state.user = user;

      this._toast.success(
        this._translate.instant('USER.SUCCESSFUL_UPDATE'),
        this._translate.instant( 'GENERAL.INFO'));

      this.router.navigate(['/dashboard']);
    }, err => {
      this._toast.error(
        this._translate.instant('USER.UNSUCCESSFUL_UPDATE'),
        this._translate.instant( 'GENERAL.ERROR'));
    });
  }

  removeNewPicture(): void {
    this.newProfilePicture = null;
    this.profilePicture = null;
  }

  deleteUser(): void {
    this.dialog.open(ConfirmationDialogComponent, {
      data: {
        callback: () => {
          this._user.delete().subscribe(() => {
            this._toast.success(
              this._translate.instant('USER.SUCCESSFUL_DELETE'),
              this._translate.instant( 'GENERAL.INFO'));

            this._state.user = null;

            this.router.navigate(['/home']);
          });
        }
      }
    });
  }

  changePicture() {
    const e: HTMLElement = this.fileInput.nativeElement;
    e.click();
  }

  onFileSelected(event) {
    if(event.target.files.length > 0) {
      this.profilePicture = event.target.files[0];
    }

    let reader = new FileReader();
    reader.readAsDataURL(event.target.files[0]);

    reader.onload = (_event) => {
      this.newProfilePicture = reader.result;
    }
  }

  birthDayPickerFilter (d: Date | null): boolean {
    const today = new Date();
    const date = (d || today);

    const oneHundredYearsAgoDate = new Date(today.getFullYear() - 100, today.getMonth(), today.getDay());

    return date < today && date > oneHundredYearsAgoDate;
  }

  get email(): string {
    return this.userForm.get('email').value;
  }

  get profilePictureId() {
    return this.userForm.get('profilePictureId');
  }
}
