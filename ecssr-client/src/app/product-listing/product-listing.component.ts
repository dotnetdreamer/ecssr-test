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
  allColumns = ['id', 'name'];

  termControl = new FormControl();
  terms: string[] = [];
  filteredOptions: Promise<string[]>;

  constructor(public paginationService: PaginationService
    , private productSvc: ProductService) { 

  }

  async ngOnInit() {
    this.termControl.valueChanges.pipe(
      startWith(''),
      map((value) => this._filter(value))
    ).subscribe(async changes => {
      this.filteredOptions = changes;
    });
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

  private async _filter(value: string) {
    const term = value.toLowerCase();
    console.log('filter', term);
    const result = await this.productSvc.search(term);
    const names = result.map(p => p.name);

    return names;
    // return this.terms.filter(option => option.toLowerCase().indexOf(filterValue) === 0);
  }
}
