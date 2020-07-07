import { environment } from 'src/environments/environment';

export class AppConstant {
    public static readonly DEBUG = !environment.production;

    public static readonly BASE_URL = !environment.production 
        ? "http://localhost:54487/" : "http://localhost:54487/";
    public static readonly BASE_API_URL = `${AppConstant.BASE_URL}api/`;
}