import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'courier', pathMatch: 'full' },
  { path: 'courier', loadComponent: () => import('./features/courier/courier.component').then(m => m.CourierComponent) }
];
