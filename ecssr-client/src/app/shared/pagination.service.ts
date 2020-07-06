import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class PaginationService {
    private paginationModel: PaginationModel;

    get page(): number {
        return this.paginationModel.pageIndex;
    }

    get selectItemsPerPage(): number[] {
        return this.paginationModel.selectItemsPerPage;
    }

    get pageSize(): number {
        return this.paginationModel.pageSize;
    }

    constructor() {
        this.paginationModel = new PaginationModel();
    }

    change(pageEvent) {
        this.paginationModel.pageIndex = pageEvent.pageIndex + 1;
        this.paginationModel.pageSize = pageEvent.pageSize;
        this.paginationModel.allItemsLength = pageEvent.length;
    }
}

export class PaginationModel {
    pageSize;
    pageIndex;
    allItemsLength;
    selectItemsPerPage;
    
    constructor() {
        this.pageIndex = 0;
        this.pageSize = 5;
        this.selectItemsPerPage = 0;
        this.allItemsLength = 0;
    }
}