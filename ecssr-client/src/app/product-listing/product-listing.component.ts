import { Component, OnInit, EventEmitter, Output, Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';

import { PaginationService } from '../shared/pagination.service';
import { IProduct } from './product.model';
import { ProductService } from './product.service';

@Component({
  selector: 'app-product-listing',
  templateUrl: './product-listing.component.html',
  styleUrls: ['./product-listing.component.scss']
})
export class ProductListingComponent implements OnInit {
  dataSource = new MatTableDataSource<IProduct>();
  displayedColumns = ['id', 'name'];
  

  totalCount: number = 0;  
  constructor(public paginationService: PaginationService
    , private productSvc: ProductService) { 

  }

  async ngOnInit() {
    await this._getProductList();
  }

  onPageSwitch(ev) {
    console.log('ev', ev);
  }

  private async _getProductList() {
    const products = await this.productSvc.getProductList();
    this.dataSource = new MatTableDataSource<IProduct>(products);
  }
}
