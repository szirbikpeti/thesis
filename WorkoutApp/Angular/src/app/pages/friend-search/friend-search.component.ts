import { Component, OnInit } from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {UserService} from "../../services/user.service";
import {UserModel} from "../../models/UserModel";
import {StateService} from "../../services/state.service";

@Component({
  selector: 'app-friend-search',
  templateUrl: './friend-search.component.html',
  styleUrls: ['./friend-search.component.scss']
})
export class FriendSearchComponent implements OnInit {

  friendSearchForm: FormGroup;

  users: UserModel[];

  currentUser: UserModel;

  constructor(private fb: FormBuilder, private _user: UserService, private _state: StateService) {
    _state.user.subscribe(storedUser => this.currentUser = storedUser);
  }

  ngOnInit(): void {
    this.friendSearchForm = this.fb.group({
      name: ['', Validators.required]
    });
  }

  submitFriendSearchForm(): void {
    this._user.search(this.name)
      .subscribe(foundedUsers => {this.users = foundedUsers; console.log(this.users);});
  }

  follow(id: string) {
    this._user.follow(id)
      .subscribe(currentUser => this._state.user = currentUser);
  }

  undo(id: string) {
    this._user.undoFollow(id)
      .subscribe(currentUser => this._state.user = currentUser);
  }

  isRequestedUser(id: string): boolean {
    return this.currentUser.requestedUserIds.includes(id);
  }

  get name(): string {
    return this.friendSearchForm.get('name').value;
  }
}
