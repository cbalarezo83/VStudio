import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';


@Component({
    selector: 'nav-menu',
    templateUrl: './navmenu.component.html',
    styleUrls: ['./navmenu.component.css']
})
export class NavMenuComponent {
    constructor(public auth: AuthService,
        private router: Router) { }

    logout(): boolean {

        if (this.auth.logout()) {
            this.router.navigate([""]);
        }
        return false;
    }
}
