import { Pipe, PipeTransform } from '@angular/core';
import { AppConstant } from '../shared/app-constant';


@Pipe({
  name:"fullpath"
})
export class FullPathPipe implements PipeTransform {
    constructor() {

    }

    transform(path: string) {
        if(path) {
            return `${AppConstant.BASE_URL}${path}`;
        }
    } 
}