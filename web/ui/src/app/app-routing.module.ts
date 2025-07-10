import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CallbackComponent } from './callback/callback.component';
// import { AuthGuard } from './core/guard/auth.guard';
import { LayoutComponent } from './layout/layout.component';
import { LoginDlgComponent } from './layout/login-dlg/login-dlg.component';
import { SilentComponent } from './layout/silent/silent.component';
import { InvoiceUploadComponent } from './ERS-Pages/invoiceUpload/invoiceUpload.component';
import { LoginComponent } from './pages/login/login.component';
import { LoginGuard } from './core/guard/login.guard';

const routes: Routes = [
  // { path: '', pathMatch: 'full', redirectTo: 'ers' },
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: '',
    canActivate: [LoginGuard],
    component: LayoutComponent,
    children: [
      {
        path:'',
        redirectTo:'ers',
        pathMatch:'full'
      },
      {
        path: 'ers',
        loadChildren: () => import('./ERS-Pages/ERSPages.module').then(m => m.ERSPagesModule),
      }]
  },
  {
    path: '**',
    redirectTo: 'ers'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { onSameUrlNavigation: 'reload' })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
