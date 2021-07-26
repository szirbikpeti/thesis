import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {FormArray, FormBuilder, FormGroup, Validators} from "@angular/forms";
import {TranslateService} from "@ngx-translate/core";
import {WorkoutService} from "../../services/workout.service";
import {FileService} from "../../services/file.service";
import {FileModel} from "../../models/FileModel";
import {Observable, zip} from "rxjs";
import {WorkoutRequest} from "../../requests/WorkoutRequest";
import {MatTableDataSource} from "@angular/material/table";
import {FileTableModel} from "../../models/FileTableModel";
import {isNull} from "../../utility";
import {ToastrService} from "ngx-toastr";
import {Router} from "@angular/router";

@Component({
  selector: 'app-new-workout',
  templateUrl: './new-workout.component.html',
  styleUrls: ['./new-workout.component.scss']
})
export class NewWorkoutComponent implements OnInit {
  @ViewChild('fileInput') fileInput: ElementRef;

  workoutForm: FormGroup;

  selectedFiles: FileTableModel[] = [];

  dataSource: MatTableDataSource<FileTableModel>;
  displayedColumns: string[] = ['position', 'name', 'type', 'preview', 'operation'];

  constructor(private fb: FormBuilder, private _workout: WorkoutService,
              private _file: FileService, private _translate: TranslateService,
              private _toast: ToastrService, private router: Router) { }

  ngOnInit(): void {
    this.workoutForm = this.fb.group({
      date: ['', Validators.required],
      type: ['', Validators.required],
      exercises: this.fb.array([this.createNewExercise()])
    });

    this.dataSource = new MatTableDataSource<FileTableModel>(this.selectedFiles);
  }

  private createNewExercise(): FormGroup {
    return this.fb.group({
      name: ['', Validators.required],
      equipment: ['', Validators.required],
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

  addNewExercise(): void {
    this.exercises.push(this.createNewExercise());
  }

  addNewSet(exerciseIndex: number): void {
    this.getSet(exerciseIndex).push(this.createNewSet());
  }

  deleteSet(exerciseIndex: number, setIndex: number): void {
    this.getSet(exerciseIndex).removeAt(setIndex);
  }

  addAttachment() {
    const e: HTMLElement = this.fileInput.nativeElement;
    e.click();
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
      return;
    }

    const uploadCalls$: Observable<FileModel>[] = [];

    Object.keys(this.selectedFiles).forEach(key => {
      const currentFile: FileTableModel = this.selectedFiles[key];

      uploadCalls$.push(this._file.upload(currentFile.file));
    });

    zip(...uploadCalls$).subscribe((uploadedFiles: FileModel[]) => {
      const fileIds = uploadedFiles.map(file => file.id);
      this.submitWorkoutForm(fileIds);
    });

    if (this.selectedFiles.length === 0) {
      this.submitWorkoutForm();
    }
  }

  private submitWorkoutForm(fileIds: string[] = null): void {
    const workoutRequest: WorkoutRequest = this.workoutForm.getRawValue();

    if (!isNull(fileIds)){
      workoutRequest.fileIds = fileIds;
    }

    this._workout.create(workoutRequest).subscribe(() => {
      this._toast.success(
        this._translate.instant('WORKOUT.SUCCESSFUL_ADDITION'),
        this._translate.instant( 'GENERAL.INFO'));

      this.router.navigate(['/my-workouts']);
    }, () => {
      this._toast.success(
        this._translate.instant('WORKOUT.UNSUCCESSFUL_ADDITION'),
        this._translate.instant( 'GENERAL.ERROR'));
    });
  }

  futureFilter (d: Date | null): boolean {
    const date = (d || new Date());
    return date < new Date();
  }

  get exercises(): FormArray {return this.workoutForm.get('exercises') as FormArray;}

  getSet(exerciseIndex: number): FormArray {
    return (this.exercises.at(exerciseIndex) as FormGroup).get('sets') as FormArray;
  }

  deleteExercise(exerciseIndex: number): void {
    this.exercises.removeAt(exerciseIndex);
  }

  deleteSelectedAttachment(position: number) {
    this.selectedFiles.forEach((value,index)=>{
      if(value.position === position) this.selectedFiles.splice(index,1);
    });

    this.dataSource.data = this.selectedFiles;
  }
}
