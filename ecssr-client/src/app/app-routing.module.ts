import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';


const routes: Routes = [
  { path: 'product-listing', loadChildren: () => import('./product-listing/product-listing.module').then(m => m.ProductListingModule) }, 
  { path: '', redirectTo: '/product-listing', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
