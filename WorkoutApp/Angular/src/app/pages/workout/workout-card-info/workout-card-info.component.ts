import {Component, Input} from '@angular/core';
import {WorkoutModel} from "../../../models/WorkoutModel";
import {StateService} from "../../../services/state.service";
import {getPicture} from "../../../utility";
import {DomSanitizer} from "@angular/platform-browser";
import {Router} from "@angular/router";
import {WorkoutService} from "../../../services/workout.service";
import {NewPostComponent} from "../../dashboard/new-post/new-post.component";
import {MatDialog} from "@angular/material/dialog";
import {ConfirmationDialogComponent} from "../../confirmation-dialog/confirmation-dialog.component";

@Component({
  selector: 'app-workout-card-info',
  templateUrl: './workout-card-info.component.html',
  styleUrls: ['./workout-card-info.component.scss']
})
export class WorkoutCardInfoComponent{

  @Input() workouts: WorkoutModel[];

  getPicture = getPicture;

  constructor(public _state: StateService, private _workout: WorkoutService,
              private router: Router, public sanitizer: DomSanitizer, private dialog: MatDialog) { }

  editWorkout(id: string): void {
    this.router.navigate(['/edit-workout', id]);
  }

  deleteWorkout(id: string) {
    this.dialog.open(ConfirmationDialogComponent, {
      data: {
        callback: () => {
          this._workout.delete(id)
            .subscribe(() => window.location.reload()); // TODO - make it better
        }
      }
    });
  }

  openPostCreatorModal(workout: WorkoutModel) {
    this.dialog.open(NewPostComponent, {
      width: '650px',
      disableClose: true,
      data: {
        workout: workout
      }
    });
  }
}
