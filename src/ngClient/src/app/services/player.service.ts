import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';

export interface Track {
  id: string;
  title: string;
  artist: string;
  fileUrl: string;
  duration: number;
  albumArt?: string;
}

@Injectable({
  providedIn: 'root'
})
export class PlayerService {
  private currentTrackSubject = new BehaviorSubject<Track | null>(null);
  private isPlayingSubject = new BehaviorSubject<boolean>(false);
  private currentPlaylistSubject = new BehaviorSubject<Track[]>([]);
  private currentTrackIndexSubject = new BehaviorSubject<number>(0);

  // Observables for components to subscribe to
  currentTrack$ = this.currentTrackSubject.asObservable();
  isPlaying$ = this.isPlayingSubject.asObservable();
  currentPlaylist$ = this.currentPlaylistSubject.asObservable();
  currentTrackIndex$ = this.currentTrackIndexSubject.asObservable();

  // Play a specific track
  playTrack(track: Track) {
    this.currentTrackSubject.next(track);
    this.isPlayingSubject.next(true);
  }

  // Load a playlist and play a specific track
  playTrackFromPlaylist(tracks: Track[], trackIndex: number) {
    if (trackIndex >= 0 && trackIndex < tracks.length) {
      this.currentPlaylistSubject.next(tracks);
      this.currentTrackIndexSubject.next(trackIndex);
      this.currentTrackSubject.next(tracks[trackIndex]);
      this.isPlayingSubject.next(true);
    }
  }

  // Set current track
  setCurrentTrack(track: Track) {
    this.currentTrackSubject.next(track);
    this.isPlayingSubject.next(true);
  }

  // Play/Pause toggle
  togglePlayPause() {
    this.isPlayingSubject.next(!this.isPlayingSubject.value);
  }

  // Stop playing
  stop() {
    this.isPlayingSubject.next(false);
  }

  // Next track
  nextTrack() {
    const playlist = this.currentPlaylistSubject.value;
    const currentIndex = this.currentTrackIndexSubject.value;
    
    if (playlist.length > 0 && currentIndex < playlist.length - 1) {
      const nextIndex = currentIndex + 1;
      this.currentTrackIndexSubject.next(nextIndex);
      this.currentTrackSubject.next(playlist[nextIndex]);
    }
  }

  // Previous track
  previousTrack() {
    const playlist = this.currentPlaylistSubject.value;
    const currentIndex = this.currentTrackIndexSubject.value;
    
    if (playlist.length > 0 && currentIndex > 0) {
      const prevIndex = currentIndex - 1;
      this.currentTrackIndexSubject.next(prevIndex);
      this.currentTrackSubject.next(playlist[prevIndex]);
    }
  }

  // Get current track info
  getCurrentTrack(): Track | null {
    return this.currentTrackSubject.value;
  }

  // Get playing status
  getIsPlaying(): boolean {
    return this.isPlayingSubject.value;
  }

  // Check if player should be displayed
  hasActiveTrack(): boolean {
    return this.currentTrackSubject.value !== null;
  }

  // Observable for checking if player should be displayed
  get hasActiveTrack$() {
    return this.currentTrack$.pipe(
      map(track => track !== null)
    );
  }
}
