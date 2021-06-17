import {NgModule} from '@angular/core';
import {AuthGuard} from "./guards/auth.guard";
import {SignOffGuard} from "./guards/sign-off.guard";

@NgModule({
  declarations: [],
  imports: [],
  providers: [
    AuthGuard,
    SignOffGuard,
  ]
})
export class GuardsModule {
}
