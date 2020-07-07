import { NgModule } from '@angular/core';

import { FullPathPipe } from './full-path.pipe';

@NgModule({
    declarations: [
        FullPathPipe
    ],
    imports: [ ],
    exports: [
        FullPathPipe
    ]
})
export class PipesModule { }