import {Component, ElementRef, HostListener, ViewChild} from '@angular/core';
import {AbstractControl, FormArray, FormBuilder, FormControl, FormGroup, Validators} from "@angular/forms";
import {FileTableModel} from "../../models/FileTableModel";
import {MatTableDataSource} from "@angular/material/table";
import {WorkoutService} from "../../services/workout.service";
import {FileService} from "../../services/file.service";
import {TranslateService} from "@ngx-translate/core";
import {ToastrService} from "ngx-toastr";
import {ActivatedRoute, Router} from "@angular/router";
import {Observable, zip} from "rxjs";
import {FileModel} from "../../models/FileModel";
import {WorkoutRequest} from "../../requests/WorkoutRequest";
import {getPicture, isNull} from "../../utility";
import {WorkoutModel} from "../../models/WorkoutModel";
import {DomSanitizer} from "@angular/platform-browser";
import {ConfirmationDialogComponent} from "../confirmation-dialog/confirmation-dialog.component";
import {MatDialog} from "@angular/material/dialog";
import {WorkoutType} from "../../enums/workout";

@Component({
  selector: 'app-edit-workout',
  templateUrl: './edit-workout.component.html',
  styleUrls: ['./edit-workout.component.scss']
})
export class EditWorkoutComponent {
  @ViewChild('fileInput') fileInput: ElementRef;

  workoutForm: FormGroup;
  workout: WorkoutModel;

  selectedFiles: FileTableModel[] = [];

  dataSource: MatTableDataSource<FileTableModel>;
  displayedColumns: string[];

  getPicture = getPicture;

  workoutTypes = Object.keys(WorkoutType)
    .map(key => WorkoutType[key])
    .filter(value => typeof value === 'string') as string[];

  constructor(private fb: FormBuilder, private _workout: WorkoutService,
              private _file: FileService, private _translate: TranslateService, public sanitizer: DomSanitizer,
              private _toast: ToastrService, private router: Router, private route: ActivatedRoute, private dialog: MatDialog) {
    this.displayedColumns = EditWorkoutComponent.getColumnsToDisplay();

    this.route.params.subscribe(event =>
      this._workout.get(event.id)
        .subscribe(fetchedWorkout => {
          this.workout = fetchedWorkout;
          this.setUpWorkoutForm();
        }));
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
      date: ['', Validators.required],
      type: ['', Validators.required],
      exercises: this.fb.array([])
    });

    if (this.hasExercise()) {
      this.workoutForm.addControl('exercises', this.fb.array([]));
    } else {
      this.workoutForm.addControl('distance', new FormControl('', Validators.required));
      this.workoutForm.addControl('duration', new FormControl('', Validators.required));
      this.workoutDuration.setValidators(Validators.pattern('[[0-9]*[h]{0,1}]{0,1}[ ]{0,1}[[0-9]*[m]{0,1}]{0,1}[ ]{0,1}[[0-9]*[s]{0,1}]{0,1}'));
    }

    for (let i = 0; i < this.workout.exercises.length; i++) {
      this.exercises.push(
        this.fb.group({
          id: [{value: '', disabled: true}],
          name: ['', Validators.required],
          equipment: [''],
          sets: this.fb.array([])
      }));

      for(let j = 0; j < this.workout.exercises[i].sets.length; j++) {
        this.getSet(i).push(
          this.fb.group({
            id: [{value: '', disabled: true}],
            reps: [0, Validators.required],
            weight: [0, Validators.required],
            duration: []
        }));
      }
    }

    this.workoutForm.patchValue(this.workout);
    this.type.disable();

    let i = 1;
    for(let file of this.workout.files) {
      this.selectedFiles.push(
        new FileTableModel(i++, null, file.data, file.id, file.name, file.format));
    }

    this.dataSource = new MatTableDataSource<FileTableModel>(this.selectedFiles);
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
      duration: []
    });
  }

  private deleteWorkout() {
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

  private submitWorkoutForm(fileIds: string[]): void {
    const workoutRequest: WorkoutRequest = this.workoutForm.getRawValue();
    workoutRequest.id = this.workout.id;

    fileIds = fileIds.concat(this.workout.files.map(({id}) => id));
    workoutRequest.fileIds = fileIds;

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

    Object.keys(newFiles).forEach(key => {
      const currentFile: FileTableModel = newFiles[key];
      uploadCalls$.push(this._file.upload(currentFile.file));
    });

    zip(...uploadCalls$).subscribe((uploadedFiles: FileModel[]) => {
      let fileIds = uploadedFiles.map(file => file.id);

      this.submitWorkoutForm(fileIds);
    });

    if (newFiles.length === 0) {
      this.submitWorkoutForm([]);
    }
  }

  addNewExercise(): void {
    this.exercises.push(this.createNewExercise());
  }

  addNewSet(exerciseIndex: number): void {
    this.getSet(exerciseIndex).push(this.createNewSet());
  }

  addAttachment(): void {
    const e: HTMLElement = this.fileInput.nativeElement;
    e.click();
  }

  deleteExercise(exerciseIndex: number): void {
    this.exercises.removeAt(exerciseIndex);
  }

  deleteSet(exerciseIndex: number, setIndex: number): void {
    this.getSet(exerciseIndex).removeAt(setIndex);
  }

  deleteSelectedAttachment(element: FileTableModel): void {
    this.selectedFiles.forEach((value,index)=>{
      if(value.position === element.position) this.selectedFiles.splice(index,1);
    });

    if (!isNull(element.id)) {
      const deletedIndex = this.workout.files.findIndex(fileModel => fileModel.id === element.id);
      this.workout.files.splice(deletedIndex, 1);
    }

    this.dataSource.data = this.selectedFiles;
  }

  openConfirmationDialog(): void {
    this.dialog.open(ConfirmationDialogComponent, {
      disableClose: true,
      data: {
        callback: () => this.deleteWorkout()
      }
    });
  }

  onFileSelected(event): void {
    const files: FileList = event.target.files;

    Object.keys(files).forEach(key => {
      const currentFile: File = files[key];

      let reader = new FileReader();
      reader.readAsDataURL(currentFile);

      reader.onload = (_event) => {
        this.selectedFiles.push(
          new FileTableModel(this.selectedFiles.length + 1, currentFile, reader.result));
      }

      reader.onloadend = () => {this.dataSource.data = this.selectedFiles};
    });
  }

  hasExercise(): boolean {
    return this.workout.exercises.length > 0;
  }

  @HostListener('window:resize')
  onResize(): void {
    this.displayedColumns = EditWorkoutComponent.getColumnsToDisplay();
  }

  get type(): AbstractControl { return this.workoutForm.get('type'); }

  get workoutDuration(): AbstractControl { return this.workoutForm.get('duration'); }

  get exercises(): FormArray {return this.workoutForm.get('exercises') as FormArray;}

  getSet(exerciseIndex: number): FormArray {
    return (this.exercises.at(exerciseIndex) as FormGroup).get('sets') as FormArray;
  }

  getDurationOfSet(exerciseIndex: number): AbstractControl {
    return this.getSet(exerciseIndex).get('duration');
  }

  getFileName(element: FileTableModel): string {
    return element.file?.name ?? element.name;
  }

  getType(element: FileTableModel): string {
    return element.file?.type ?? element.format;
  }

  formatDurationInput(control: AbstractControl): void {
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

    const result = (EditWorkoutComponent.getSpecifiedTime(resultHours, 'h') + EditWorkoutComponent.getSpace(resultMinutes)
      + EditWorkoutComponent.getSpecifiedTime(resultMinutes, 'm') + EditWorkoutComponent.getSpace(resultMinutes)
      + EditWorkoutComponent.getSpecifiedTime(resultSeconds, 's')).trim();

    control.setValue(result);
  }
}
