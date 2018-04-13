import { Injectable, Injector } from "@angular/core";
import { Router } from "@angular/router";
import { HttpResponse,HttpHandler, HttpEvent, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { AuthService } from "./auth.service";
import { Observable } from "rxjs";

import { HttpErrorResponse } from "@angular/common/http/src/response";

@Injectable()
export class AuthResponseInterceptor implements HttpInterceptor {
    currentRequest: HttpRequest<any>;
    auth: AuthService;

    constructor(private injector: Injector,private router : Router) { }

    intercept(request: HttpRequest<any>,
              next: HttpHandler): Observable<HttpEvent<any>> {

        this.auth = this.injector.get(AuthService);
        var token = this.auth.isLoggedIn() ? this.auth.getAuth()!.token : null;

        if (token) {
            //save current request

            if (token) {

                //save current request
                this.currentRequest = request;

                return next.handle(request).
                    do((event: HttpEvent<any>) => {
                        if (event instanceof HttpResponse) { }
                    })
                    .catch(error => { return this.handleError(error); });
            }
        } else {
            return next.handle(request);
        }
    }

    handleError(err: any) {
        if (err instanceof HttpErrorResponse) {
            if (err.status === 401) {
                  // JWT token might be expired:
                // try to get a new one using refresh token
                console.log("Token expired. Attempting refresh...");

                this.auth.refreshToken().
                    subscribe(res => {
                        console.log("here");
                    }, error => console.log(error));
            } else {
                return Observable.throw(err);
            }
        }
    }
}
