import { Component, OnInit, EventEmitter, Output, Input, ViewChild, 
  AfterViewInit, ViewEncapsulation, ElementRef } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import {FormControl, FormGroup, FormBuilder, Validators} from '@angular/forms';
import {Observable} from 'rxjs';
import {map, startWith} from 'rxjs/operators';
import ApexCharts from 'apexcharts/dist/apexcharts.common.js'

import { PaginationModel } from '../shared/pagination.model';
import { IProduct, IReport, IProductPicture } from './product.model';
import { ProductService } from './product.service';
import { AppConstant } from '../shared/app-constant';

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
  @ViewChild('thisWeekContainer') thisWeekContainer: ElementRef;
  @ViewChild('categoryContainer') categoryContainer: ElementRef;
  searchForm: FormGroup;

  dataSource = new MatTableDataSource<IProduct>(); 
  allColumns = ['name', 'category', 'companyName', 'model', 'price', 'color', 'pictures', 'videoUrl'];

  filteredOptions: Promise<string[]>;
  paginationModel = new PaginationModel();

  constructor(private formBuilder: FormBuilder
    , private productSvc: ProductService) { 
      //search form
      this.searchForm = formBuilder.group({
        term: ['', Validators.minLength(3)],
        color: [''],
        dateFrom: [''],
        dateTo: [''],
        priceFrom: ['', Validators.min(1)],
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
    if(this.f.term.value || this.f.color.value || this.f.dateFrom.value
      || this.f.dateTo.value || this.f.priceFrom.value || this.f.priceTo.value) {
        await this._getProductList();    
      }
  }

  async onFormReset() {
    this.searchForm.reset();
    await this._getProductList();    
  }

  async onPaginatorChanged(ev) {
    await this._getProductList();
  }

  onReportButtonClicked() {
    window.open(AppConstant.BASE_REPORTING_URL, "_blank");
  }

  onPictureClicked(pic: IProductPicture) {
    window.open(`${AppConstant.BASE_URL}${pic.imageUrl}`, "_blank");
  }

  onVideoClicked(url) {
    window.open(`${AppConstant.BASE_URL}${url}`, "_blank");
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

    //weekly
    const weeklyReport = await this.productSvc.weeklyReport();
    const weeklyReportItems = weeklyReport.map(wr => wr.products);
    const weeklyReportDates = weeklyReport.map(wr => wr.date);
    
    const optionsLineChart = {
      chart: {
          type: 'line',
          width: '500px'
      },
      series: [{
          name: 'Products',
          data: weeklyReportItems
      }],
      xaxis: {
        categories: weeklyReportDates
      }
  }
    const chartWeekly = new ApexCharts(this.thisWeekContainer.nativeElement, optionsLineChart);
    chartWeekly.render();

    //categories
    const catReport = await this.productSvc.categoriesReport();
    const catReportItems = catReport.map(wr => wr.products);
    const catReportNames = catReport.map(wr => wr.name);

    const optionsCategory = {
      legend: {
          show: false
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
          width: '400px'
      },
      series: catReportItems,
      labels: catReportNames
  }
    const chartCategory = new ApexCharts(this.categoryContainer.nativeElement, optionsCategory);
    chartCategory.render();
  }
}
