import { Component, OnInit, EventEmitter, Output, Input, ViewChild, 
  AfterViewInit, ViewEncapsulation, ElementRef } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import {FormControl, FormGroup, FormBuilder} from '@angular/forms';
import {Observable} from 'rxjs';
import {map, startWith} from 'rxjs/operators';
import ApexCharts from 'apexcharts/dist/apexcharts.common.js'

import { PaginationService } from '../shared/pagination.service';
import { IProduct, IReport } from './product.model';
import { ProductService } from './product.service';

@Component({
  selector: 'app-product-listing',
  templateUrl: './product-listing.component.html',
  styleUrls: ['./product-listing.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class ProductListingComponent implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild('indexesContainer') indexesContainer: ElementRef;
  @ViewChild('productsContainer') productsContainer: ElementRef;
  @ViewChild('picturesContainer') picturesContainer: ElementRef;
  searchForm: FormGroup;

  dataSource = new MatTableDataSource<IProduct>();
  allColumns = ['id', 'name', 'pictures'];

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

  get f(){ return this.searchForm.controls; }

  async ngOnInit() {
    this.f.term.valueChanges.pipe(
      startWith(''),
      map((value) => this._filter(value))
    ).subscribe(async changes => {
      this.filteredOptions = changes;
    });
  }

  async ngAfterViewInit() {
    this.paginator.pageIndex = 0;

    const result = await Promise.all([
      this._getReport(),
      this._getProductList()
    ]);
  }

  async onSearched() {
    await this._getProductList();    
  }

  onFormReset() {
    this.searchForm.reset();
  }

  async onPaginatorChanged(ev) {
    await this._getProductList();
  }

  private async _getProductList() {
    let term = this.f.term.value;
    if(term) {
      term = term.trim();
    }
    let color = this.f.color.value;
    let dateFrom = this.f.dateFrom.value;
    let dateTo = this.f.dateTo.value;
    let priceFrom = this.f.priceFrom.value;
    let priceTo = this.f.priceTo.value;

    const args = {
      term: term,
      color: color,
      dateFrom: dateFrom,
      dateTo: dateTo,
      priceFrom: priceFrom,
      priceTo: priceTo,
      pageIndex: this.paginator.pageIndex || 0,
      pageSize: this.paginator.pageSize || 5
    };

    try {
      const result = await this.productSvc.getProductList(args);

      this.dataSource.data = result.data;
      this.paginator.length = result.total;
    } catch(e) {
      throw e;
    }
  }

  private async _filter(value: string) {
    if(!value) {
      return;
    }

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
  }

  private async _getReport() {
    const options = {
      legend: {
          show: false
      },
      dataLabels: {
        enabled: false,
      },
      plotOptions: {
        pie: {
          customScale: 0.8,
          donut: {
              labels: {
                  show: true
              }
          }
        }
      },
      chart: {
        type: 'donut',
        width: '300px'
      }
    }

    const dashboardReport = await this.productSvc.dashboardReport();
    //indexes
    const chartIndexes = new ApexCharts(this.indexesContainer.nativeElement, {
      ...options,
      series: [dashboardReport.totalIndexed],
      labels: ['Total Indexes']
    });
    chartIndexes.render();

    //products
    const chartProducts = new ApexCharts(this.productsContainer.nativeElement, {
      ...options,
      series: [dashboardReport.totalProducts],
      labels: ['Total Products']
    });
    chartProducts.render();

    //pictures
    const chartPictures = new ApexCharts(this.picturesContainer.nativeElement, {
      ...options,
      series: [dashboardReport.totalPictures],
      labels: ['Total Pictures']
    });
    chartPictures.render();
  }
}
