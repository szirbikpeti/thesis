import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './pages/nav-menu/nav-menu.component';
import { HomeComponent } from './pages/home/home.component';
import { CounterComponent } from './pages/counter/counter.component';
import { FetchDataComponent } from './pages/fetch-data/fetch-data.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppRoutingModule } from './app-routing.module';
import { MaterialModule } from "./material.module";
import { I18nModule } from "./i18n/i18n.module";
import {SignUpComponent} from "./pages/sign-up/sign-up.component";
import {MAT_DATE_LOCALE} from "@angular/material/core";
import {ToastrModule} from "ngx-toastr";
import {DashboardComponent} from "./pages/dashboard/dashboard.component";
import {NewWorkoutComponent} from "./pages/new-workout/new-workout.component";
import {ProfileComponent} from "./pages/profile/profile.component";
import {ConfirmationDialogComponent} from "./pages/confirmation-dialog/confirmation-dialog.component";

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    SignUpComponent,
    DashboardComponent,
    NewWorkoutComponent,
    ProfileComponent,
    ConfirmationDialogComponent,
    CounterComponent,
    FetchDataComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
    I18nModule,
    MaterialModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    ToastrModule.forRoot({
      preventDuplicates: true,
      positionClass: 'toast-bottom-right',
      timeOut: 3000
    })
  ],
  providers: [
    {provide: MAT_DATE_LOCALE, useValue: 'hu-HU'}
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
