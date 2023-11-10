import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { SidenavFacade } from '../../state/sidenav/sidenav.facade';
import { NavItem } from './nav-item.model';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss'],
})
export class NavbarComponent {
  http = inject(HttpClient);
  mf = inject(SidenavFacade);
  menuItems = this.http.get<NavItem[]>('/assets/nav-items.json');

  toggleMenu() {
    this.mf.toggleMenuVisibility();
  }
}
