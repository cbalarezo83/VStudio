import { EventEmitter, Inject, Injectable, PLATFORM_ID } from "@angular/core";
import { isPlatformBrowser } from '@angular/common';
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Observable } from "rxjs";
import 'rxjs/Rx';

@Injectable()
export class AuthService {
    authKey: string = "auth";
    clientId: string = "TestMakerFree";


    constructor(private http: HttpClient,
                @Inject(PLATFORM_ID) private platformId: any) {
    }

    // performs the login
    login(username: string, password: string): Observable<boolean> {

        var url = "api/token/auth";

        var data = {
            username: username,
            password: password,
            client_id: this.clientId,
            // required when signing up with username/password
            grant_type: "password",
            // space-separated list of scopes for which the token is issued
            scope: "offline_access profile email"
        };


        //return this.http.post<TokenResponse>(url, data).
        //    map(res => {

        //        let token = res && res.token;

        //        if (token) {
        //            // store username and jwt token
        //            this.setAuth(res);
        //            // successful login
        //            return Observable.of(true);
        //        } else {
        //            // failed login
        //            return Observable.throw('Unauthorized');
        //        }

        //    }).
        //    catch(error => {
        //        return Observable.throw(error);
        //    });

        return this.getAuthFromServer(url, data);

    }

    refreshToken(): Observable<boolean> {
        var url = "/api/token/auth";

        var data = {
            client_id: this.clientId,
            // required when signing up with username/password
            grant_type: "refresh_token",
            refresh_token: this.getAuth()!.refresh_token,
            // space-separated list of scopes for which the token is issued
            scope: "offline_access profile email"
        };

        return this.getAuthFromServer(url, data);

    }

    getAuthFromServer(url: string, data: any): Observable<boolean> {

        return this.http.post<TokenResponse>(url, data).
            map(res => {

                let token = res && res.token;

                if (token) {
                     // store username and jwt token
                    this.setAuth(res);
                    return true;
                }
                else {
                    // failed login
                    return false;
                }
            }).
            catch(error => {
                return new Observable<any>(error);
            });
                
    }

    logout(): boolean {
        this.setAuth(null);
        return true;
    }

    setAuth(auth: TokenResponse | null): boolean {

        if (isPlatformBrowser(this.platformId)) {
            if (auth) {
                localStorage.setItem(
                    this.authKey,
                    JSON.stringify(auth));
            }
            else {
                localStorage.removeItem(this.authKey);
            }
        }

        return true;
    }

    getAuth(): TokenResponse | null {
        if (isPlatformBrowser(this.platformId)) {
            var i = localStorage.getItem(this.authKey);

            if (i) {
                return JSON.parse(i);
            }
        }

        return null;
    }

    // Returns TRUE if the user is logged in, FALSE otherwise.
    isLoggedIn(): boolean {
        if (isPlatformBrowser(this.platformId)) {
            return localStorage.getItem(this.authKey) != null;
        }
        return false;
    }

}