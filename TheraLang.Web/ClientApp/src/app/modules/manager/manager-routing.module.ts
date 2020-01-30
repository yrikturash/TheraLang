import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import { PageManagerComponent } from "./page-manager/page-manager.component";
import { CreatePageComponent } from "./page-manager/create-page/create-page.component";
import { ManagerComponent } from "./manager.component";

const routes: Routes = [
  {
    path: "pages",
    component: PageManagerComponent,
    children: [{ path: "create", component: CreatePageComponent }]
  }
];

@NgModule({
  imports: [
    RouterModule.forChild([
      { path: "", component: ManagerComponent, children: routes }
    ])
  ],
  exports: [RouterModule]
})
export class ManagerRoutingModule {}
