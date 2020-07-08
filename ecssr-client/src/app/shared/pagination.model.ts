import { Injectable } from '@angular/core';

export class PaginationModel {
    pageSize;
    pageIndex;
    allItemsLength;
    selectItemsPerPage;
    
    constructor() {
        this.pageIndex = 0;
        this.pageSize = 5;
        this.selectItemsPerPage = [5, 10, 20, 50, 100];
        this.allItemsLength = 0;
    }
}

export interface IDataSourceResult<T>{
    extraData: any;
    data: T[];
    errors: any;
    total: number;
}