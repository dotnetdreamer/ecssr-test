import { Injectable } from "@angular/core";

import { IProduct } from './product.model';
import { BaseService } from '../shared/base.service';
import { IDataSourceResult } from '../shared/pagination.service';


@Injectable({
    providedIn: 'root'
})
export class ProductService {
    private BASE_PRODUCT_URL = "product/";

    constructor(private baseService: BaseService) {
        
    }


    getProductList(args: { pageIndex, pageSize }) {
        return this.baseService.getData<IDataSourceResult<IProduct>>({ 
            url: `${this.BASE_PRODUCT_URL}getProductList`,
            body: {
                pageIndex: args.pageIndex,
                pageSize: args.pageSize
            }
        });
    }
}