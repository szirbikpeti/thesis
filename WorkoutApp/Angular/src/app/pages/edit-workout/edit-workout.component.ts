import {Component, ElementRef, HostListener, ViewChild} from '@angular/core';
import {FormArray, FormBuilder, FormGroup, Validators} from "@angular/forms";
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

  private setUpWorkoutForm(): void {
    this.workoutForm = this.fb.group({
      date: ['', Validators.required],
      type: ['', Validators.required],
      exercises: this.fb.array([])
    });

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

  addNewExercise(): void {
    this.exercises.push(this.createNewExercise());
  }

  addNewSet(exerciseIndex: number): void {
    this.getSet(exerciseIndex).push(this.createNewSet());
  }

  addAttachment() {
    const e: HTMLElement = this.fileInput.nativeElement;
    e.click();
  }

  deleteExercise(exerciseIndex: number): void {
    this.exercises.removeAt(exerciseIndex);
  }

  deleteSet(exerciseIndex: number, setIndex: number): void {
    this.getSet(exerciseIndex).removeAt(setIndex);
  }

  deleteSelectedAttachment(element: FileTableModel) {
    this.selectedFiles.forEach((value,index)=>{
      if(value.position === element.position) this.selectedFiles.splice(index,1);
    });

    if (!isNull(element.id)) {
      const deletedIndex = this.workout.files.findIndex(fileModel => fileModel.id === element.id);
      this.workout.files.splice(deletedIndex, 1);
    }

    this.dataSource.data = this.selectedFiles;
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

  openConfirmationDialog() {
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

  uploadAttachments(): void {
    if (this.workoutForm.invalid) {
      console.log("invalid form");
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

  @HostListener('window:resize')
  onResize() {
    this.displayedColumns = EditWorkoutComponent.getColumnsToDisplay();
  }

  get exercises(): FormArray {return this.workoutForm.get('exercises') as FormArray;}

  getSet(exerciseIndex: number): FormArray {
    return (this.exercises.at(exerciseIndex) as FormGroup).get('sets') as FormArray;
  }

  getFileName(element: FileTableModel): string {
    return element.file?.name ?? element.name;
  }

  getType(element: FileTableModel): string {
    return element.file?.type ?? element.format;
  }
}
