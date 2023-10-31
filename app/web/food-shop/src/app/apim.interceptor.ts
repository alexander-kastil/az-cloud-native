import { HttpHandlerFn, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { environment } from '../environments/environment';

export const apimInterceptor = () => {
    const interceptor: HttpInterceptorFn = (req: HttpRequest<unknown>, next: HttpHandlerFn) => {
        var request = req.clone({
            headers: req.headers.set(
                'Ocp-Apim-Subscription-Key',
                environment.azure.apimSubscriptionKey
            )
        });
        return next(request);
    };
    return interceptor;
};