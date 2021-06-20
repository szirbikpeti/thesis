import {Component, Input} from '@angular/core';
import {WorkoutModel} from "../../../models/WorkoutModel";
import {DatePipe} from "@angular/common";
import {StateService} from "../../../services/state.service";
import {getPicture} from "../../../utility";
import {DomSanitizer} from "@angular/platform-browser";
import {Router} from "@angular/router";

@Component({
  selector: 'app-workout-card-info',
  templateUrl: './workout-card-info.component.html',
  styleUrls: ['./workout-card-info.component.scss']
})
export class WorkoutCardInfoComponent{

  @Input() workouts: WorkoutModel[];

  getPicture = getPicture;

  constructor(private datePipe: DatePipe, private _state: StateService,
              private router: Router, public sanitizer: DomSanitizer) { }

  getDateFormat(date: Date) {
    const format = this._state.language.value === 'hu'
      ? 'yyyy.MM.dd'
      : 'mediumDate';

    return this.datePipe.transform(date, format);
  }

  editWorkout(id: string): void {
    this.router.navigate(['/edit-workout', id]);
  }
}