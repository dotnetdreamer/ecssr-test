import { Component, OnInit, EventEmitter, Output, Input, ViewChild, AfterViewInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import {FormControl} from '@angular/forms';
import {Observable} from 'rxjs';
import {map, startWith} from 'rxjs/operators';

import { PaginationService } from '../shared/pagination.service';
import { IProduct } from './product.model';
import { ProductService } from './product.service';

@Component({
  selector: 'app-product-listing',
  templateUrl: './product-listing.component.html',
  styleUrls: ['./product-listing.component.scss']
})
export class ProductListingComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator) paginator: MatPaginator;

  dataSource = new MatTableDataSource<IProduct>();
  displayedColumns = ['id', 'name'];
  myControl = new FormControl();
  options: string[] = ['One', 'Two', 'Three'];
  filteredOptions: Observable<string[]>;

  constructor(public paginationService: PaginationService
    , private productSvc: ProductService) { 

  }

  ngOnInit() {
    this.filteredOptions = this.myControl.valueChanges.pipe(
      startWith(''),
      map(value => this._filter(value))
    );
  }

  async ngAfterViewInit() {
    this.paginator.pageIndex = 0;

    await this._getProductList();
  }

  async onPaginatorChanged(ev) {
    await this._getProductList();
  }

  private async _getProductList() {
    const result = await this.productSvc.getProductList({
      pageIndex: this.paginator.pageIndex || 0,
      pageSize: this.paginator.pageSize || 5
    });

    this.dataSource.data = result.data;
    this.paginator.length = result.total;
  }

  private _filter(value: string): string[] {
    const filterValue = value.toLowerCase();

    return this.options.filter(option => option.toLowerCase().indexOf(filterValue) === 0);
  }
}
