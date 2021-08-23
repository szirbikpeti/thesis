import { Component, } from '@angular/core';
import {AdminService} from "../../services/admin.service";
import {FeedbackModel} from "../../models/FeedbackModel";

@Component({
  selector: 'app-feedbacks',
  templateUrl: './feedbacks.component.html',
  styleUrls: ['./feedbacks.component.scss']
})
export class FeedbacksComponent  {

  feedbacks: FeedbackModel[];

  constructor(private _admin: AdminService) {
    _admin.listFeedbacks()
      .subscribe(feedbacks => this.feedbacks = feedbacks);
  }

}
