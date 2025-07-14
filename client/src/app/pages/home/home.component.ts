import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from './../../services/auth.service';
import { ReleaseService, Release } from '../../services/release.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],

})

export class HomeComponent implements OnInit {
  title = 'BLARE';
  authService = inject(AuthService);
  releaseService = inject(ReleaseService);

  artistReleases$: Observable<Release[]> | null = null;
  recentReleases$: Observable<Release[]> | null = null;

  hasReleases$: Observable<boolean> | null = null;

  ngOnInit() {
    // Load recent releases for all users
    if (this.authService.isAuthenticated()) {
      if (this.authService.getCurrentUser()?.role === 'artist') {
        this.artistReleases$ = this.releaseService.getReleasesByArtist(this.authService.getCurrentUser()?.id || '');
      } else if (this.authService.getCurrentUser()?.role === 'listener') {
        this.recentReleases$ = this.releaseService.getRecentReleases();
      }
    }
  }

  getArtistReleases(artistId: string): Observable<Release[]> {
    return this.releaseService.getReleasesByArtist(artistId);
  }

  constructor() {
    // You can inject services or perform any initialization here
    console.log(this.authService.getCurrentUser()?.role);
  }
}