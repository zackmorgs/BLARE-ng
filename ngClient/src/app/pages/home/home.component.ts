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
    console.log('ngOnInit called');
    
    // Load recent releases for all users
    if (this.authService.isAuthenticated()) {
      const currentUser = this.authService.getCurrentUser();
      console.log('Current user:', currentUser);
      
      if (currentUser?.role === 'artist') {
        console.log('Loading releases for artist:', currentUser.id);
        this.artistReleases$ = this.releaseService.getReleasesByArtist(currentUser.id);
        
        // Add error handling and debugging
        this.artistReleases$.subscribe({
          next: (releases) => console.log('Artist releases loaded:', releases),
          error: (error) => console.error('Error loading artist releases:', error)
        });
      } else if (currentUser?.role === 'listener') {
        console.log('Loading recent releases for listener');
        this.recentReleases$ = this.releaseService.getRecentReleases();
        
        this.recentReleases$.subscribe({
          next: (releases) => console.log('Recent releases loaded:', releases),
          error: (error) => console.error('Error loading recent releases:', error)
        });
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