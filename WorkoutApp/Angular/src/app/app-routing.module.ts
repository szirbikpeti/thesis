import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CounterComponent } from "./pages/counter/counter.component";
import { HomeComponent } from "./pages/home/home.component";
import { FetchDataComponent } from "./pages/fetch-data/fetch-data.component";
import { DashboardComponent } from "./pages/dashboard/dashboard.component";
import {NavMenuComponent} from "./pages/nav-menu/nav-menu.component";
import {NewWorkoutComponent} from "./pages/new-workout/new-workout.component";
import {ProfileComponent} from "./pages/profile/profile.component";

const routes: Routes = [
  {
    path: '',
    redirectTo: '/home',
    pathMatch: 'full'
  },
  {
    path: '',
    component: NavMenuComponent,
    children : [
      {
        path: 'dashboard',
        component: DashboardComponent
      },
      {
        path: 'profile',
        component: ProfileComponent
      },
      {
        path: 'new-workout',
        component: NewWorkoutComponent
      },
    ]
  },
  {
    path: 'home',
    component: HomeComponent
  },
  {
    path : '**',
    pathMatch: 'full',
    redirectTo: ''
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
