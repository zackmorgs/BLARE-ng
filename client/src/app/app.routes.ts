import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  {
    path: 'home',
    loadComponent: () => import('./pages/home/home.component').then(m => m.HomeComponent)
  },
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./pages/register/register.component').then(m => m.RegisterComponent)
  },
  {
    path: 'logout',
    loadComponent: () => import('./pages/logout/logout.component').then(m => m.LogoutComponent)
  },
  {
    path: 'account',
    loadComponent: () => import('./pages/user/account/account.component').then(m => m.AccountComponent),
    canActivate: [authGuard]
  },
  {
    path: 'support',
    loadComponent: () => import('./pages/support/support.component').then(m => m.SupportComponent)
  },
  {
    path: 'download',
    loadComponent: () => import('./pages/download/download.component').then(m => m.DownloadComponent)
  },
  {
    path: 'privacy',
    loadComponent: () => import('./pages/privacy/privacy.component').then(m => m.PrivacyComponent)
  },
  {
    path: 'terms',
    loadComponent: () => import('./pages/terms/terms.component').then(m => m.TermsComponent)
  },
  {
    path: 'user/:usernameSlug',
    loadComponent: () => import('./pages/user/user.component').then(m => m.UserComponent)
  },
  {
    path: 'user/:usernameSlug/playlist/:playlistSlug',
    loadComponent: () => import('./pages/user/playlist/playlist.component').then(m => m.PlaylistComponent)
  },
  { path: '**', redirectTo: '/' }
];