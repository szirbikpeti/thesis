import { Component } from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";

@Component({
  selector: 'app-new-post',
  templateUrl: './new-post.component.html',
  styleUrls: ['./new-post.component.scss']
})
export class NewPostComponent {

  postForm: FormGroup

  constructor(private fb: FormBuilder) {
    this.postForm = fb.group({
      description: ['', Validators.required],

    });
  }

}
