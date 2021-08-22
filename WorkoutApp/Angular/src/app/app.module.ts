import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './pages/nav-menu/nav-menu.component';
import { HomeComponent } from './pages/home/home.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppRoutingModule } from './app-routing.module';
import { MaterialModule } from "./material.module";
import { I18nModule } from "./i18n/i18n.module";
import {SignUpComponent} from "./pages/sign-up/sign-up.component";
import {MAT_DATE_LOCALE} from "@angular/material/core";
import {ToastrModule} from "ngx-toastr";
import {DashboardComponent} from "./pages/dashboard/dashboard.component";
import {WorkoutComponent} from "./pages/workout/workout.component";
import {NewWorkoutComponent} from "./pages/new-workout/new-workout.component";
import {ProfileComponent} from "./pages/profile/profile.component";
import {ConfirmationDialogComponent} from "./pages/confirmation-dialog/confirmation-dialog.component";
import {GuardsModule} from "./guards.module";
import {WorkoutCardInfoComponent} from "./pages/workout/workout-card-info/workout-card-info.component";
import {NgPipesModule} from "ngx-pipes";
import {DatePipe} from "@angular/common";
import {FriendSearchComponent} from "./pages/friend-search/friend-search.component";
import {NotificationModalComponent} from "./pages/nav-menu/notification-modal/notification-modal.component";
import {FriendsComponent} from "./pages/friends/friends.component";
import {DateFormatterPipe} from './pipes/date-formatter.pipe';
import {NewPostComponent} from "./pages/dashboard/new-post/new-post.component";
import {IsVideoPipe} from "./pipes/is-video.pipe";
import {PostCardComponent} from "./pages/dashboard/post-card/post-card.component";
import {StatisticsComponent} from "./pages/statistics/statistics.component";
import {HighchartsChartModule} from "highcharts-angular";
import {NotificationMenuComponent} from "./pages/nav-menu/notification-menu/notification-menu.component";
import {TruncatePipe} from "./pipes/truncate.pipe";
import {FeedbackBottomSheetComponent} from "./pages/nav-menu/feedback-bottom-sheet/feedback-bottom-sheet.component";
import {LikeModalComponent} from "./pages/dashboard/post-card/like-modal/like-modal.component";
import {EmailConfirmationComponent} from "./pages/email-confirmation/email-confirmation.component";
import {HttpErrorInterceptor} from "./error.interceptor";
import {PasswordResetComponent} from "./pages/password-reset/password-reset.component";

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    NotificationMenuComponent,
    NotificationModalComponent,
    HomeComponent,
    SignUpComponent,
    DashboardComponent,
    PostCardComponent,
    LikeModalComponent,
    NewPostComponent,
    WorkoutComponent,
    WorkoutCardInfoComponent,
    NewWorkoutComponent,
    ProfileComponent,
    FriendSearchComponent,
    FriendsComponent,
    StatisticsComponent,
    ConfirmationDialogComponent,
    FeedbackBottomSheetComponent,
    EmailConfirmationComponent,
    PasswordResetComponent,
    DateFormatterPipe,
    IsVideoPipe,
    TruncatePipe,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
    GuardsModule,
    I18nModule,
    MaterialModule,
    NgPipesModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    HighchartsChartModule,
    ToastrModule.forRoot({
      preventDuplicates: true,
      progressBar: true,
      positionClass: 'toast-bottom-right',
      timeOut: 3000,
    })
  ],
  providers: [
    {provide: MAT_DATE_LOCALE, useValue: 'hu-HU'},
    {provide: HTTP_INTERCEPTORS, useClass: HttpErrorInterceptor, multi: true},
    DatePipe,
    DateFormatterPipe
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
