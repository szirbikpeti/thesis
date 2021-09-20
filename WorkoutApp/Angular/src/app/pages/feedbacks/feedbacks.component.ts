import { Component, } from '@angular/core';
import {AdminService} from "../../services/admin.service";
import {FeedbackModel} from "../../models/FeedbackModel";
import {getPicture} from "../../utility";
import {StateService} from "../../services/state.service";
import {DomSanitizer} from "@angular/platform-browser";

@Component({
  selector: 'app-feedbacks',
  templateUrl: './feedbacks.component.html',
  styleUrls: ['./feedbacks.component.scss']
})
export class FeedbacksComponent  {

  feedbacks: FeedbackModel[];

  getPicture = getPicture;

  constructor(private _admin: AdminService, public _state: StateService,
              public sanitizer: DomSanitizer) {
    _admin.listFeedbacks()
      .subscribe(feedbacks => this.feedbacks = feedbacks);
  }

}
