import { Component, OnInit } from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {StateService} from "../../services/state.service";
import {UserModel} from "../../models/UserModel";
import {UserService} from "../../services/user.service";

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {

  userForm: FormGroup;

  currentUser: UserModel;

  constructor(private fb: FormBuilder, private _state: StateService, private _user: UserService) {
    this.currentUser = _state.user.value;
  }

  ngOnInit(): void {
    this.userForm = this.fb.group({
      fullName: [this.currentUser.fullName, Validators.required],
      email: [this.currentUser.email, Validators.required],
      about: [this.currentUser.about, Validators.required],
      birthday: [this.currentUser.birthDay]
    });
  }

  submitUserForm(): void {
    console.log(this.userForm.value);
  }
}
