import { environment } from 'src/environments/environment';

export class AppConstant {
    public static readonly DEBUG = !environment.production;

    public static readonly BASE_URL = !environment.production 
        ? "http://localhost:40002/" : "http://localhost:40002/";

    public static readonly BASE_API_URL = `${AppConstant.BASE_URL}api/`;

    public static readonly BASE_REPORTING_URL = `http://localhost:40003/Reports/Html5ReportViewer1.html`;
}