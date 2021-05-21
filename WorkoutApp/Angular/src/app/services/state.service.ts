import { Injectable } from "@angular/core";
import { UserModel } from "../models/UserModel";
import { BehaviorSubject } from "rxjs";
import { StorageService } from "./storage.service";

@Injectable({
  providedIn: 'root'
})
export class StateService {
  private readonly LANGUAGE = "currentLanguage";
  private readonly USER = "currentUser";

  private readonly currentLanguage: BehaviorSubject<string>;
  private readonly currentUserState: BehaviorSubject<UserModel>;

  constructor(private _storage: StorageService) {
    this.currentLanguage = new BehaviorSubject<string>(_storage.get(this.LANGUAGE));
    this.currentUserState = new BehaviorSubject<UserModel>(_storage.get(this.USER));
  }

  get language() {
    return this.currentLanguage;
  }

  set language(language: any) {
    this.currentLanguage.next(language);
    this._storage.save(this.LANGUAGE, language);
  }

  get user() {
    return this.currentUserState;
  }

  set user(user: any) {
    this.currentUserState.next(user);
    this._storage.save(this.USER, user);
  }
}
