import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from "./pages/home/home.component";
import { DashboardComponent } from "./pages/dashboard/dashboard.component";
import {NavMenuComponent} from "./pages/nav-menu/nav-menu.component";
import {NewWorkoutComponent} from "./pages/new-workout/new-workout.component";
import {ProfileComponent} from "./pages/profile/profile.component";
import {SignOffGuard} from "./guards/sign-off.guard";
import {AuthGuard} from "./guards/auth.guard";
import {FriendSearchComponent} from "./pages/friend-search/friend-search.component";
import {FriendsComponent} from "./pages/friends/friends.component";
import {WorkoutComponent} from "./pages/workout/workout.component";
import {StatisticsComponent} from "./pages/statistics/statistics.component";
import {EmailConfirmationComponent} from "./pages/email-confirmation/email-confirmation.component";

const routes: Routes = [
  {
    path: '',
    redirectTo: '/home',
    pathMatch: 'full'
  },
  {
    path: 'home',
    component: HomeComponent,
    canActivate: [SignOffGuard]
  },
  {
    path: 'email-confirmation',
    component: EmailConfirmationComponent,
  },
  {
    path: '',
    component: NavMenuComponent,
    canActivate: [AuthGuard],
    children : [
      {
        path: 'dashboard',
        component: DashboardComponent
      },
      {
        path: 'my-workouts',
        component: WorkoutComponent
      },
      {
        path: 'profile',
        component: ProfileComponent
      },
      {
        path: 'new-workout',
        component: NewWorkoutComponent
      },
      {
        path: 'new-workout/:duplicatedWorkoutId',
        component: NewWorkoutComponent
      },
      {
        path: 'edit-workout/:editedWorkoutId',
        component: NewWorkoutComponent
      },
      {
        path: 'friend-search',
        component: FriendSearchComponent
      },
      {
        path: 'friends',
        component: FriendsComponent
      },
      {
        path: 'statistics',
        component: StatisticsComponent
      },
    ]
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
