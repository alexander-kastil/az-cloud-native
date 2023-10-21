import { LayoutModule } from '@angular/cdk/layout';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { MsalAuthUtilModule } from '../auth/msal-auth-util.module';


import { NavbarComponent } from './navbar/navbar.component';
import { SidebarComponent } from './sidebar/sidebar.component';

const comps = [NavbarComponent, SidebarComponent];

@NgModule({
    exports: comps,
    imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    LayoutModule,
    MsalAuthUtilModule,
    ...comps,
],
})
export class MenusModule {}
