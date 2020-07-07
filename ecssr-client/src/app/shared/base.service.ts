import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';

import { AppConstant } from './app-constant';

@Injectable({
    providedIn: 'root'
})
export class BaseService {
    constructor(private httpClient: HttpClient) {

    }

    getData<T>(args: HttpParams): Promise<T> {
        return new Promise(async (resolve, reject) => {
            let headers: HttpHeaders = await this.prepareHeaders(args);

            args.body = args.body || {};  
            if(!args.overrideUrl) {
                let newUrl = `${AppConstant.BASE_API_URL + args.url}`;

                for(let prop in args.body) {
                    if(args.body.hasOwnProperty(prop)) {
                        if(newUrl.includes('?')) {
                            newUrl += '&';
                        } else {
                            newUrl += '?';
                        }
                        newUrl += `${prop}=${args.body[prop]}`;
                    }
                }   
                args.url = newUrl;
            }

            this.httpClient.get<T>(args.url, {
                headers: headers
            })
            .subscribe(result => {
                resolve(<T>result);
            }, error => {
                this.handleError(error, args);
                if(args.errorCallback) {
                    resolve();
                } else {
                    reject(error);
                }
            });
        });
    }

    postData<T>(args: HttpParams): Promise<T> {
        return new Promise(async (resolve, reject) => {
            let headers: HttpHeaders = await this.prepareHeaders(args);

            let newUrl;
            if(!args.overrideUrl) {
                newUrl = `${AppConstant.BASE_API_URL + args.url}`;
            } else {
                newUrl = args.url;
            }
  
            args.url = newUrl;

            let body = args.body;
            this.httpClient.post<T>(args.url, body, {
                headers: headers
            })
            .subscribe(result => {
                resolve(<T>result);
            }, error => {
                this.handleError(error, args);
                if(args.errorCallback) {
                    resolve();
                } else {
                    reject(error);
                }
            });
        });
    }

    async handleError(e: HttpErrorResponse, args: HttpParams) {
        if(AppConstant.DEBUG) {
            console.log('BaseService: handleError', e);
        }
        switch(e.status) {
            case 401:
               //handle
            break;
            default:
                if(!args.errorCallback) {
                    let msg;
                    if(e.message) {
                        msg = e.message;            
                    } else {
                        msg = e.error.toString();
                    }
                } else {
                    args.errorCallback(e, args);
                }
            break;
        }
    }
    
    private async prepareHeaders(args: HttpParams) {
        let headers = new HttpHeaders();
        if(!args.ignoreContentType) {
            headers = headers.append('Content-Type', 'application/json;charset=utf-8');            
        }
 
        if(args.httpHeaders) {
            args.httpHeaders.keys().forEach(k => {
                headers = headers.append(k, args.httpHeaders.get(k));
            });
        }
        return headers;
    }
}

export class HttpParams {
    url: string
    body?: any
    errorCallback?
    ignoreContentType?: boolean
    overrideUrl?: boolean
    httpHeaders?: HttpHeaders
}