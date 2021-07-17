import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

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
import {EditWorkoutComponent} from "./pages/edit-workout/edit-workout.component";
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

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    NotificationModalComponent,
    HomeComponent,
    SignUpComponent,
    DashboardComponent,
    WorkoutComponent,
    WorkoutCardInfoComponent,
    NewWorkoutComponent,
    EditWorkoutComponent,
    ProfileComponent,
    FriendSearchComponent,
    FriendsComponent,
    ConfirmationDialogComponent,
    DateFormatterPipe
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
    ToastrModule.forRoot({
      preventDuplicates: true,
      positionClass: 'toast-bottom-right',
      timeOut: 3000
    })
  ],
  providers: [
    {provide: MAT_DATE_LOCALE, useValue: 'hu-HU'},
    DatePipe,
    DateFormatterPipe
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
