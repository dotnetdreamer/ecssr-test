import { Injectable } from "@angular/core";

import { IProduct, IReport } from './product.model';
import { BaseService } from '../shared/base.service';
import { IDataSourceResult } from '../shared/pagination.model';


@Injectable({
    providedIn: 'root'
})
export class ProductService {
    private BASE_PRODUCT_URL = "product/";

    constructor(private baseService: BaseService) {
        
    }


    getProductList(args: { pageIndex, pageSize, term?, color?, dateFrom?, dateTo?, priceFrom?, priceTo?}) {
        return this.baseService.getData<IDataSourceResult<IProduct>>({ 
            url: `${this.BASE_PRODUCT_URL}getProductList`,
            body: {
                pageIndex: args.pageIndex,
                pageSize: args.pageSize,
                term: args.term || undefined,
                color: args.color || undefined,
                fromDate: args.dateFrom || undefined,
                toDate: args.dateTo || undefined,
                priceFrom: args.priceFrom || undefined,
                priceTo: args.priceTo || undefined
            }
        });
    }

    dashboardReport() {
        return this.baseService.getData<IReport>({
            url: `${this.BASE_PRODUCT_URL}dashboardReport`
        });
    }

    search(term) {
        return this.baseService.getData<IProduct[]>({ 
            url: `${this.BASE_PRODUCT_URL}search`,
            body: {
                term: term,
            }
        });
    }
}