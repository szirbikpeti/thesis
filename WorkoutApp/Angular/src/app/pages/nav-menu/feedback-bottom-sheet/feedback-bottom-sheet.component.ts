import {Component} from '@angular/core';
import {MatBottomSheetRef} from "@angular/material/bottom-sheet";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {FeedbackService} from "../../../services/feedback.service";
import {ToastrService} from "ngx-toastr";
import {TranslateService} from "@ngx-translate/core";

@Component({
  selector: 'app-feedback-bottom-sheet',
  templateUrl: './feedback-bottom-sheet.component.html',
  styleUrls: ['./feedback-bottom-sheet.component.scss']
})
export class FeedbackBottomSheetComponent  {

  feedbackForm: FormGroup;

  constructor(private fb: FormBuilder, private _feedback: FeedbackService,
              private _toast: ToastrService, private _translate: TranslateService,
              private bottomSheetRef: MatBottomSheetRef<FeedbackBottomSheetComponent>) {
    this.feedbackForm = fb.group({
      feedback: ['', Validators.required],
      stars: [4, Validators.required]
    });
  }

  submitFeedbackForm(): void {
    if (this.feedbackForm.invalid) {
      return;
    }

    this._feedback.send(this.feedbackForm.value)
      .subscribe(() => {
        this._toast.success(
          this._translate.instant('FEEDBACK.SUCCESSFUL_ADDITION'),
          this._translate.instant('GENERAL.INFO'));

        this.bottomSheetRef.dismiss();
      });
  }
}
