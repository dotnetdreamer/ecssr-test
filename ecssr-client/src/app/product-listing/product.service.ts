import { Injectable } from "@angular/core";

import { IProduct } from './product.model';


@Injectable({
    providedIn: 'root'
})
export class ProductService {
    constructor() {
        
    }

    getProductList() {
        return new Promise<IProduct[]>((resolve, reject) => {
            const data = [{
                id: 1,
                name: 'P1'
            }, {
                id: 2,
                name: 'P2'
            }, {
                id: 3,
                name: 'P3'
            }, {
                id: 4,
                name: 'P4'
            }, {
                id: 5,
                name: 'P5'
            }, {
                id: 6,
                name: 'P6'
            }, {
                id: 7,
                name: 'P7'
            }];

            resolve(data);
        });
    }
}