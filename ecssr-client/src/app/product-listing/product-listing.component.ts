import { Component, OnInit, EventEmitter, Output, Input, ViewChild, AfterViewInit, ViewEncapsulation } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import {FormControl, FormGroup, FormBuilder} from '@angular/forms';
import {Observable} from 'rxjs';
import {map, startWith} from 'rxjs/operators';

import { PaginationService } from '../shared/pagination.service';
import { IProduct } from './product.model';
import { ProductService } from './product.service';

@Component({
  selector: 'app-product-listing',
  templateUrl: './product-listing.component.html',
  styleUrls: ['./product-listing.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class ProductListingComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator) paginator: MatPaginator;
  searchForm: FormGroup;

  dataSource = new MatTableDataSource<IProduct>();
  allColumns = ['id', 'name', 'pictures'];

  termControl = new FormControl();
  terms: string[] = [];
  filteredOptions: Promise<string[]>;

  constructor(private formBuilder: FormBuilder
    , public paginationService: PaginationService
    , private productSvc: ProductService) { 
      //search form
      this.searchForm = formBuilder.group({
        term: [''],
        color: [''],
        dateFrom: [''],
        dateTo: [''],
        priceFrom: [''],
        priceTo: ['']
      });
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
    if(Array.isArray(value)) {
      return;
    }

    const term = value.toLowerCase();
    if(term.length <= 2) {
      return;
    }

    const result = await this.productSvc.search(term);
    const names = result.map(p => p.name);

    return names;
    // return this.terms.filter(option => option.toLowerCase().indexOf(filterValue) === 0);
  }
}
