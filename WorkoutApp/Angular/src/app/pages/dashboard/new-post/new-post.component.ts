import { Component } from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {WorkoutService} from "../../../services/workout.service";
import {WorkoutModel} from "../../../models/WorkoutModel";
import {StateService} from "../../../services/state.service";
import {getPicture} from "../../../utility";
import {DomSanitizer} from "@angular/platform-browser";
import {PostService} from "../../../services/post.service";
import {ToastrService} from "ngx-toastr";
import {TranslateService} from "@ngx-translate/core";
import {MatDialogRef} from "@angular/material/dialog";

@Component({
  selector: 'app-new-post',
  templateUrl: './new-post.component.html',
  styleUrls: ['./new-post.component.scss']
})
export class NewPostComponent {

  postForm: FormGroup;

  unPostedWorkouts: WorkoutModel[];

  selectedWorkout: WorkoutModel;
  selectableFiles = [];

  getPicture = getPicture;

  constructor(public _state: StateService, public sanitizer: DomSanitizer, private fb: FormBuilder,
              private _toast: ToastrService, private _translate: TranslateService,
              private _workout: WorkoutService, private _post: PostService,
              private dialogRef: MatDialogRef<NewPostComponent>) {
    this.postForm = fb.group({
      workoutId: [{value: '', disabled: true}],
      description: ['', Validators.required],
      fileIds: [{value: '', disabled: true}]
    });

    _workout.list_unposted()
      .subscribe(workouts => this.unPostedWorkouts = workouts);
  }

  submitPostForm(): void {
    if (this.postForm.invalid) {
      this.postForm.markAllAsTouched();
      return;
    }

    const rawData = this.postForm.getRawValue();
    rawData.workoutId = this.selectedWorkout.id;
    rawData.fileIds = this.selectableFiles
      .filter(file => file.isChecked)
      .map(({file}) => file.id);

    this._post.create(rawData).subscribe(() => {
      this.dialogRef.close();

      this._toast.success(
        this._translate.instant('POST.SUCCESSFUL_ADDITION'),
        this._translate.instant( 'GENERAL.INFO'));
    }, () => {

      this._toast.error(
        this._translate.instant('POST.UNSUCCESSFUL_ADDITION'),
        this._translate.instant( 'GENERAL.ERROR'));
    });
  }

  getFiles(): void {
    this.selectableFiles = [];

    this.selectedWorkout.files.forEach(file => {
      this.selectableFiles.push({file: file, isChecked: true});
    });

    console.log(this.selectableFiles);
  }

  mediaFileOnClick(index: number): void {
    if (this.selectableFiles[index].isChecked && this.selectableFiles.filter(file => file.isChecked).length === 1) {
      return;
    }

    this.selectableFiles[index].isChecked = !this.selectableFiles[index].isChecked;
  }
}
