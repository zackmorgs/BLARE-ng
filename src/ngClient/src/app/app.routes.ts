import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  {
    path: 'home',
    loadComponent: () => import('./pages/home/home.component').then(m => m.HomeComponent),
    canActivate: [authGuard]
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
    loadComponent: () => import('./pages/logout/logout.component').then(m => m.LogoutComponent),
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
    path: 'user/:usernameSlug/account',
    loadComponent: () => import('./pages/user/account/account.component').then(m => m.AccountComponent),
    canActivate: [authGuard]
  },
  {
    path: 'user/:usernameSlug',
    loadComponent: () => import('./pages/user/user.component').then(m => m.UserComponent),
    canActivate: [authGuard]
  },
  {
    path: 'user/:usernameSlug/playlist/:playlistSlug',
    loadComponent: () => import('./pages/user/playlist/playlist.component').then(m => m.PlaylistComponent),
    canActivate: [authGuard]
  },
  {
    path: 'artist/releases/new',
    loadComponent: () => import('./pages/artist/releases/new/new.component').then(m => m.NewComponent),
    canActivate: [authGuard]
  },
  {
    path: 'play/artist/:artistSlug/album/:releaseSlug',
    loadComponent: () => import('./pages/play/play.component').then(m => m.PlayComponent),
    canActivate: [authGuard]
  },
  // {
  //   path: 'play/playlist/:slug',
  //   loadComponent: () => import('./pages/play/playlist/play-playlist.component').then(m => m.PlayPlaylistComponent),
  //   canActivate: [authGuard]
  // },
  { path: '**', redirectTo: '/' }
];