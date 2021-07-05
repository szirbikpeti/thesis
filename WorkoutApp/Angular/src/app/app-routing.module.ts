import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from "./pages/home/home.component";
import { DashboardComponent } from "./pages/dashboard/dashboard.component";
import {NavMenuComponent} from "./pages/nav-menu/nav-menu.component";
import {NewWorkoutComponent} from "./pages/new-workout/new-workout.component";
import {EditWorkoutComponent} from "./pages/edit-workout/edit-workout.component";
import {ProfileComponent} from "./pages/profile/profile.component";
import {SignOffGuard} from "./guards/sign-off.guard";
import {AuthGuard} from "./guards/auth.guard";
import {FriendSearchComponent} from "./pages/friend-search/friend-search.component";
import {FriendsComponent} from "./pages/friends/friends.component";

const routes: Routes = [
  {
    path: '',
    redirectTo: '/home',
    pathMatch: 'full'
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
        path: 'profile',
        component: ProfileComponent
      },
      {
        path: 'new-workout',
        component: NewWorkoutComponent
      },
      {
        path: 'edit-workout/:id',
        component: EditWorkoutComponent
      },
      {
        path: 'friend-search',
        component: FriendSearchComponent
      },
      {
        path: 'friends',
        component: FriendsComponent
      },
    ]
  },
  {
    path: 'home',
    component: HomeComponent,
    canActivate: [SignOffGuard]
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
