import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CounterComponent } from "./pages/counter/counter.component";
import { HomeComponent } from "./pages/home/home.component";
import { FetchDataComponent } from "./pages/fetch-data/fetch-data.component";
import { DashboardComponent } from "./pages/dashboard/dashboard.component";

const routes: Routes = [
  // {path: '', component: HomeComponent},
  // {path: 'dashboard', component: DashboardComponent},
  {path: '', component: DashboardComponent},
  {path: 'counter', component: CounterComponent},
  {path: 'fetch-data', component: FetchDataComponent},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
