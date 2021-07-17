import { Component } from '@angular/core';
import {WorkoutModel} from "../../models/WorkoutModel";
import {WorkoutService} from "../../services/workout.service";
import {StateService} from "../../services/state.service";
import {TranslateService} from "@ngx-translate/core";
import {isNull} from "../../utility";
import {DateFormatterPipe} from "../../pipes/date-formatter.pipe";

@Component({
  selector: 'app-workout',
  templateUrl: './workout.component.html',
  styleUrls: ['./workout.component.scss']
})
export class WorkoutComponent {
  workouts: WorkoutModel[];

  isNull = isNull;

  constructor(private _workout: WorkoutService, private _state: StateService,
              private _translate: TranslateService, private customDatePipe: DateFormatterPipe) {
    this._workout.list().subscribe(workouts => this.workouts = workouts);
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

    return this.customDatePipe.transform(workoutDate, this._state.language.value, false);
  }
}
