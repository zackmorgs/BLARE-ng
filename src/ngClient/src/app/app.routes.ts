import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  {
    path: 'home',
    loadComponent: () => import('./pages/home/home.component').then(m => m.HomeComponent),
    canActivate: [authGuard],
    data: { title: 'Home - BLARE' }
  },
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login.component').then(m => m.LoginComponent),
    data: { title: 'Login - BLARE' }
  },
  {
    path: 'register',
    loadComponent: () => import('./pages/register/register.component').then(m => m.RegisterComponent),
    data: { title: 'Register - BLARE' }
  },
  {
    path: 'logout',
    loadComponent: () => import('./pages/logout/logout.component').then(m => m.LogoutComponent),
    canActivate: [authGuard],
    data: { title: 'Logout - BLARE' }
  },
  {
    path: 'support',
    loadComponent: () => import('./pages/support/support.component').then(m => m.SupportComponent),
    data: { title: 'Support - BLARE' }
  },
  {
    path: 'download',
    loadComponent: () => import('./pages/download/download.component').then(m => m.DownloadComponent),
    data: { title: 'Download - BLARE' }
  },
  {
    path: 'privacy',
    loadComponent: () => import('./pages/privacy/privacy.component').then(m => m.PrivacyComponent),
    data: { title: 'Privacy Policy - BLARE' }
  },
  {
    path: 'terms',
    loadComponent: () => import('./pages/terms/terms.component').then(m => m.TermsComponent),
    data: { title: 'Terms of Service - BLARE' }
  },
  {
    path: 'user/:usernameSlug/account',
    loadComponent: () => import('./pages/user/account/account.component').then(m => m.AccountComponent),
    canActivate: [authGuard],
    data: { title: 'Account Settings - BLARE' }
  },
  {
    path: 'user/:usernameSlug',
    loadComponent: () => import('./pages/user/user.component').then(m => m.UserComponent),
    canActivate: [authGuard],
    data: { title: 'User Profile - BLARE' }
  },
  {
    path: 'user/:usernameSlug/playlist/:playlistSlug',
    loadComponent: () => import('./pages/user/playlist/playlist.component').then(m => m.PlaylistComponent),
    canActivate: [authGuard],
    data: { title: 'Playlist - BLARE' }
  },
  {
    path: 'artist/releases/new',
    loadComponent: () => import('./pages/artist/releases/new/new.component').then(m => m.NewComponent),
    canActivate: [authGuard],
    data: { title: 'New Release - BLARE' }
  },
  {
    path: 'play/artist/:artistSlug/release/:releaseSlug',
    loadComponent: () => import('./pages/play/play.component').then(m => m.PlayComponent),
    canActivate: [authGuard],
    data: { title: 'Now Playing - BLARE' }
  },
  {
    path: "artist/releases/new/finish/:releaseId",
    loadComponent: () => import('./pages/artist/releases/finish/finish.component').then(m => m.FinishComponent),
    canActivate: [authGuard],
    data: { title: 'Finish Release - BLARE' }
  },

  { path: '**', redirectTo: '/' }
];