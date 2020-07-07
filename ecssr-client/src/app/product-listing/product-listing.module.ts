import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';

import { ProductListingComponent } from './product-listing.component';
import { PipesModule } from '../pipes/pipes.module';
 
const routes: Routes = [
  { path: '', component: ProductListingComponent }
];
 
@NgModule({
  imports: [
    CommonModule,
    PipesModule,
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ],
  declarations: []
})
export class ProductListingModule { }