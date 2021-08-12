import {Component, ElementRef, HostListener, ViewChild} from '@angular/core';
import {AbstractControl, FormArray,FormBuilder, FormControl, FormGroup, Validators} from "@angular/forms";
import {TranslateService} from "@ngx-translate/core";
import {WorkoutService} from "../../services/workout.service";
import {FileService} from "../../services/file.service";
import {FileModel} from "../../models/FileModel";
import {Observable, zip} from "rxjs";
import {WorkoutRequest} from "../../requests/WorkoutRequest";
import {MatTableDataSource} from "@angular/material/table";
import {FileTableModel} from "../../models/FileTableModel";
import {ToastrService} from "ngx-toastr";
import {ActivatedRoute, Router} from "@angular/router";
import {MatSelectChange} from "@angular/material/select";
import {WorkoutType} from "../../enums/workout";
import {WorkoutModel} from "../../models/WorkoutModel";
import {ConfirmationDialogComponent} from "../confirmation-dialog/confirmation-dialog.component";
import {MatDialog} from "@angular/material/dialog";
import {getPicture, isNull} from "../../utility";
import {DomSanitizer} from "@angular/platform-browser";

@Component({
  selector: 'app-new-workout',
  templateUrl: './new-workout.component.html',
  styleUrls: ['./new-workout.component.scss']
})
export class NewWorkoutComponent {
  private readonly DURATION_REGEX = '[[0-9]*[h]{0,1}]{0,1}[ ]{0,1}[[0-9]*[m]{0,1}]{0,1}[ ]{0,1}[[0-9]*[s]{0,1}]{0,1}';

  @ViewChild('fileInput') fileInput: ElementRef;

  workoutForm: FormGroup;
  workout: WorkoutModel;

  WorkoutFeature = WorkoutFeature;
  feature: WorkoutFeature;

  selectedFiles: FileTableModel[] = [];

  dataSource: MatTableDataSource<FileTableModel>;
  displayedColumns: string[];

  workoutTypes = Object.keys(WorkoutType)
    .map(key => WorkoutType[key])
    .filter(value => typeof value === 'string') as string[];

  previousWorkoutType = WorkoutType.GYM;

  getPicture = getPicture;

  constructor(private fb: FormBuilder, private _workout: WorkoutService, public sanitizer: DomSanitizer,
              private _file: FileService, private _translate: TranslateService, private dialog: MatDialog,
              private _toast: ToastrService, private router: Router, private route: ActivatedRoute) {
    this.displayedColumns = NewWorkoutComponent.getColumnsToDisplay();
    this.setUpWorkoutForm();

    const editedWorkoutId = route.snapshot.paramMap.get('editedWorkoutId');
    const duplicatedWorkoutId = route.snapshot.paramMap.get('duplicatedWorkoutId');
    const workoutId = duplicatedWorkoutId || editedWorkoutId;

    if (workoutId) {
      this.feature = editedWorkoutId === workoutId
        ? WorkoutFeature.EDIT
        : WorkoutFeature.DUPLICATE;

      this.patchWorkoutForm(workoutId);

    } else {
      this.feature = WorkoutFeature.ADD;
      this.exercises.push(this.createNewExercise());
    }

    this.dataSource = new MatTableDataSource<FileTableModel>(this.selectedFiles);
  }

  private static getColumnsToDisplay(): string[] {
    if (window.innerWidth < 545) {
      return ['position', 'preview', 'operation'];
    }

    if (window.innerWidth < 980) {
      return ['position', 'name', 'preview', 'operation'];
    }

    if (window.innerWidth > 1050) {
      return ['position', 'name', 'type', 'preview', 'operation'];
    }
  }

  private static getSpecifiedTime(number: number, time: string): string {
    return number > 0 ? (number + time) : '';
  }

  private static getSpace(minutes: number): string {
    return minutes > 0 ? ' ' : '';
  }

  private setUpWorkoutForm(): void {
    this.workoutForm = this.fb.group({
      date: [new Date(), Validators.required],
      type: [WorkoutType.GYM, Validators.required],
      exercises: this.fb.array([])
    });
  }

  private patchWorkoutForm(workoutId: string): void {
    this._workout.get(workoutId).subscribe(fetchedWorkout => {
      this.workout = fetchedWorkout;
      this.type.setValue(fetchedWorkout.type);
      this.type.disable();

      if (this.hasExercise()) {
        for (let i = 0; i < fetchedWorkout.exercises.length; i++) {
          this.exercises.push(
            this.fb.group({
              name: ['', Validators.required],
              equipment: [''],
              sets: this.fb.array([])
            }));

          for(let j = 0; j < fetchedWorkout.exercises[i].sets.length; j++) {
            this.getSets(i).push(this.createNewSet());
          }
        }
      } else {
        this.workoutForm.addControl('distance', new FormControl('', Validators.required));
        this.workoutForm.addControl('duration',
          new FormControl('', [Validators.required, Validators.pattern(this.DURATION_REGEX)]));
      }

      this.workoutForm.patchValue(fetchedWorkout);

      if (this.feature === WorkoutFeature.EDIT) {
        let i = 1;
        for(let file of this.workout.files) {
          this.selectedFiles.push(
            new FileTableModel(i++, null, file.data, file.id, file.name, file.format));
        }
      }
    });
  }

  private submitWorkoutForm(fileIds: string[]): void {
    const workoutRequest: WorkoutRequest = this.workoutForm.getRawValue();

    if (this.feature === WorkoutFeature.EDIT) {
      workoutRequest.id = this.workout.id;
      fileIds = fileIds.concat(this.workout.files.map(({id}) => id));
    }

    workoutRequest.fileIds = fileIds;

    if (this.feature === WorkoutFeature.EDIT) {
      this._workout.update(this.workout.id, workoutRequest).subscribe(() => {
        this._toast.success(
          this._translate.instant('WORKOUT.SUCCESSFUL_MODIFICATION'),
          this._translate.instant( 'GENERAL.INFO'));

        this.router.navigate(['/my-workouts']);
      }, () => {
        this._toast.error(
          this._translate.instant('WORKOUT.UNSUCCESSFUL_MODIFICATION'),
          this._translate.instant( 'GENERAL.ERROR'));
      });
    } else {
      this._workout.create(workoutRequest).subscribe(() => {
        this._toast.success(
          this._translate.instant('WORKOUT.SUCCESSFUL_ADDITION'),
          this._translate.instant( 'GENERAL.INFO'));

        this.router.navigate(['/my-workouts']);
      }, () => {
        this._toast.error(
          this._translate.instant('WORKOUT.UNSUCCESSFUL_ADDITION'),
          this._translate.instant( 'GENERAL.ERROR'));
      });
    }
  }

  private createNewExercise(): FormGroup {
    return this.fb.group({
      name: ['', Validators.required],
      equipment: [''],
      sets: this.fb.array([this.createNewSet()])
    });
  }

  private createNewSet(): FormGroup {
    return this.fb.group({
      reps: [1, Validators.required],
      weight: [0, Validators.required],
      duration: [null, Validators.pattern(this.DURATION_REGEX)]
    });
  }

  private deleteWorkout(): void {
    this._workout.delete(this.workout.id)
      .subscribe(() => {
        this._toast.success(
          this._translate.instant('WORKOUT.SUCCESSFUL_DELETE'),
          this._translate.instant( 'GENERAL.INFO'));

        this.router.navigate(['/dashboard']);
      }, () => {
        this._toast.success(
          this._translate.instant('WORKOUT.UNSUCCESSFUL_DELETE'),
          this._translate.instant( 'GENERAL.INFO'));

        this.router.navigate(['/dashboard']);
      });
  }

  futureFilter (d: Date | null): boolean {
    const date = (d || new Date());
    return date < new Date();
  }

  uploadAttachments(): void {
    if (this.workoutForm.invalid) {
      return;
    }

    const uploadCalls$: Observable<FileModel>[] = [];

    const newFiles = this.selectedFiles.filter(file => !isNull(file.file));

    const filesToUpload = this.feature === WorkoutFeature.EDIT ? newFiles : this.selectedFiles;

    if (filesToUpload.length === 0) {
      this.submitWorkoutForm([]);
      return;
    }

    Object.keys(filesToUpload).forEach(key => {
      const currentFile: FileTableModel = filesToUpload[key];

      uploadCalls$.push(this._file.upload(currentFile.file));
    });

    zip(...uploadCalls$).subscribe((uploadedFiles: FileModel[]) => {
      const fileIds = uploadedFiles.map(file => file.id);
      this.submitWorkoutForm(fileIds);
    });
  }

  formatDurationInput(control: AbstractControl) {
    if (control.invalid) {
      return;
    }

    let value = control.value.replace(/\s/g, "");

    if (Number(value)) {
      value = `0h${value}m0s`;
    } else {
      if (!value.includes('h')){
        value = '0h' + value;
      }
      if (!value.includes('s')) {
        value += '0s';
      }
      if (!value.includes('m')) {
        const hIndex = value.indexOf('h') + 1;
        const hours = value.substring(0, hIndex);
        value = value.substring(hIndex);
        value = hours + '0m' + value;
      }
    }

    const hIndex = value.indexOf('h');
    let hours = Number(value.substring(0, hIndex));

    const mIndex = value.indexOf('m');
    let minutes = Number(value.substring(hIndex+1, mIndex));

    const sIndex = value.indexOf('s');
    let seconds = Number(value.substring(mIndex+1, sIndex));

    const allSeconds = hours*60*60 + minutes*60 + seconds;

    const resultHours = Math.floor(allSeconds/60/60);
    const resultMinutes = Math.floor((allSeconds - resultHours*60*60)/60);
    const resultSeconds = Math.floor(allSeconds - resultHours*60*60 - resultMinutes*60);

    const result = (NewWorkoutComponent.getSpecifiedTime(resultHours, 'h') + NewWorkoutComponent.getSpace(resultMinutes)
      + NewWorkoutComponent.getSpecifiedTime(resultMinutes, 'm') + NewWorkoutComponent.getSpace(resultMinutes)
      + NewWorkoutComponent.getSpecifiedTime(resultSeconds, 's')).trim();

    control.setValue(result);
  }

  openConfirmationDialogToDelete(): void {
    this.dialog.open(ConfirmationDialogComponent, {
      disableClose: true,
      data: {
        callback: () => this.deleteWorkout()
      }
    });
  }

  addNewExercise(): void {
    this.exercises.push(this.createNewExercise());
  }

  addNewSet(exerciseIndex: number): void {
    this.getSets(exerciseIndex).push(this.createNewSet());
  }

  addAttachment() {
    const e: HTMLElement = this.fileInput.nativeElement;
    e.click();
  }

  deleteExercise(exerciseIndex: number): void {
    this.exercises.removeAt(exerciseIndex);
  }

  deleteSet(exerciseIndex: number, setIndex: number): void {
    this.getSets(exerciseIndex).removeAt(setIndex);
  }

  deleteSelectedAttachment(element: FileTableModel) {
    this.selectedFiles.forEach((value,index)=>{
      if(value.position === element.position) this.selectedFiles.splice(index,1);
    });

    if (this.feature === WorkoutFeature.EDIT && !isNull(element.id)) {
      const deletedIndex = this.workout.files.findIndex(fileModel => fileModel.id === element.id);
      this.workout.files.splice(deletedIndex, 1);
    }

    this.dataSource.data = this.selectedFiles;
  }

  onFileSelected(event): void {
    const files: FileList = event.target.files;

    Object.keys(files).forEach(key => {
      const currentFile: File = files[key];

      let reader = new FileReader();
      reader.readAsDataURL(currentFile);

      reader.onload = (_event) => {
        this.selectedFiles.push(
          new FileTableModel(this.selectedFiles.length + 1, currentFile, reader.result, null, currentFile.name, currentFile.type));
      }

      reader.onloadend = () => {this.dataSource.data = this.selectedFiles};
    });
  }

  onTypeChange(event: MatSelectChange): void {
    if (this.feature !== WorkoutFeature.ADD) {
      return;
    }

    if (this.hasExercise() && !this.hasPreviousTypeExercise()) {
      this.workoutForm.addControl('exercises', this.fb.array([]));
      this.exercises.push(this.createNewExercise());

      this.workoutForm.removeControl('distance');
      this.workoutForm.removeControl('duration');
    }

    if (!this.hasExercise()) {
      this.workoutForm.removeControl('exercises');
      this.workoutForm.addControl('distance', new FormControl('', Validators.required));
      this.workoutForm.addControl('duration',
        new FormControl('', [Validators.required, Validators.pattern(this.DURATION_REGEX)]));
    }

    this.previousWorkoutType = event.value;
  }

  hasExercise(): boolean {
    return [WorkoutType.GYM, WorkoutType.HOME, WorkoutType.STREET].includes(this.type.value as WorkoutType);
  }

  hasPreviousTypeExercise(): boolean {
    return [WorkoutType.GYM, WorkoutType.HOME, WorkoutType.STREET].includes(this.previousWorkoutType as WorkoutType);
  }

  get type(): AbstractControl { return this.workoutForm.get('type'); }

  get workoutDuration(): AbstractControl { return this.workoutForm.get('duration'); }

  get exercises(): FormArray { return this.workoutForm.get('exercises') as FormArray; }

  getSets(exerciseIndex: number): FormArray {
    return (this.exercises.at(exerciseIndex) as FormGroup).get('sets') as FormArray;
  }

  getDurationOfSet(exerciseIndex: number, setIndex: number): AbstractControl {
    return this.getSets(exerciseIndex).at(setIndex).get('duration');
  }

  @HostListener('window:resize')
  onResize(): void {
    this.displayedColumns = NewWorkoutComponent.getColumnsToDisplay();
  }

  @HostListener('window:beforeunload')
  beforeUnloadHandler(): boolean {
    return !this.workoutForm.dirty;
  }
}

enum WorkoutFeature {
  ADD,
  EDIT,
  DUPLICATE
}
