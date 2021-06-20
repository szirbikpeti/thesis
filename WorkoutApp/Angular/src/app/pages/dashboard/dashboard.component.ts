import { Component, OnInit } from '@angular/core';
import {WorkoutService} from "../../services/workout.service";
import {WorkoutModel} from "../../models/WorkoutModel";
import {DatePipe} from "@angular/common";
import {StateService} from "../../services/state.service";
import {TranslateService} from "@ngx-translate/core";
import {isNull} from "../../utility";

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {

  workouts: WorkoutModel[];

  isNull = isNull;

  constructor(private _workout: WorkoutService, private _state: StateService,
              private _translate: TranslateService, private datePipe: DatePipe) {
    this._workout.list().subscribe(workouts => this.workouts = workouts);
  }

  ngOnInit(): void {
  }

  getGroupTitle(date: Date) {
    const workoutDate = new Date(date);
    const today = new Date(Date.now());

    if (workoutDate.toDateString() === today.toDateString()) {
      return this._translate.instant('GENERAL.TODAY');
    }

    today.setDate(today.getDate() - 1);
    if (workoutDate.toDateString() === today.toDateString()) {
      return this._translate.instant('GENERAL.YESTERDAY');
    }

    const format = this._state.language.value === 'hu' ? 'yyyy.MM.dd' : 'mediumDate';
    return this.datePipe.transform(workoutDate, format);
  }
}
