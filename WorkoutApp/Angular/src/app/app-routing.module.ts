import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from "./pages/home/home.component";
import { DashboardComponent } from "./pages/dashboard/dashboard.component";
import {NavMenuComponent} from "./pages/nav-menu/nav-menu.component";
import {ManageWorkoutComponent} from "./pages/manage-workout/manage-workout.component";
import {ProfileComponent} from "./pages/profile/profile.component";
import {SignOffGuard} from "./guards/sign-off.guard";
import {AuthGuard} from "./guards/auth.guard";
import {FriendSearchComponent} from "./pages/friend-search/friend-search.component";
import {FriendsComponent} from "./pages/friends/friends.component";
import {WorkoutComponent} from "./pages/workout/workout.component";
import {StatisticsComponent} from "./pages/statistics/statistics.component";
import {EmailConfirmationComponent} from "./pages/email-confirmation/email-confirmation.component";
import {PasswordResetComponent} from "./pages/password-reset/password-reset.component";
import {FeedbacksComponent} from "./pages/feedbacks/feedbacks.component";
import {AdminGuard} from "./guards/admin.guard";
import {UsersComponent} from "./pages/users/users.component";
import {MessageComponent} from "./pages/message/message.component";

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
    path: 'password-reset',
    component: PasswordResetComponent,
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
        component: ManageWorkoutComponent
      },
      {
        path: 'new-workout/:duplicatedWorkoutId',
        component: ManageWorkoutComponent
      },
      {
        path: 'edit-workout/:editedWorkoutId',
        component: ManageWorkoutComponent
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
      {
        path: 'messages',
        component: MessageComponent
      },
      {
        path: 'messages/:id',
        component: MessageComponent
      },
      {
        path: 'users',
        component: UsersComponent,
        canActivate: [AdminGuard]
      },
      {
        path: 'feedbacks',
        component: FeedbacksComponent,
        canActivate: [AdminGuard]
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
