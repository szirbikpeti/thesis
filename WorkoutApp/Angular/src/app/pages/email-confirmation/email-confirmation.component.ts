import { Component } from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {AuthService} from "../../services/auth.service";
import {EmailConfirmationRequest} from "../../requests/EmailConfirmationRequest";
import {TranslateService} from "@ngx-translate/core";

@Component({
  selector: 'app-email-confirmation',
  templateUrl: './email-confirmation.component.html',
  styleUrls: ['./email-confirmation.component.scss']
})
export class EmailConfirmationComponent  {

  message: string;

  constructor(private route: ActivatedRoute, private _auth: AuthService,
              private _translate: TranslateService) {
    route.queryParams.subscribe(params => {
      const userId = params.userId;
      const token = params.token;

      if (!(userId && token)) {
        this.message = _translate.instant('USER.EMAIL_CONFIRMATION.WRONG_URL');
      }

      const request: EmailConfirmationRequest = { userId: userId, token: token };

      _auth.confirmEmail(request).subscribe(() => {
        this.message = _translate.instant('USER.EMAIL_CONFIRMATION.THANKFUL_MESSAGE');
      }, error => {
        if (error.status === 400) {
          this.message = _translate.instant('USER.EMAIL_CONFIRMATION.ALREADY_CONFIRMED');
          return;
        }

        this.message = _translate.instant('USER.EMAIL_CONFIRMATION.FAILED_CONFIRMATION');
      });
    });
  }

}
