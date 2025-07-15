import { Component, Input, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface Track {
  id: string;
  title: string;
  artist: string;
  fileUrl: string;
  duration: number;
}

@Component({
  selector: 'app-player',
  imports: [CommonModule],
  templateUrl: './player.component.html',
  styleUrl: './player.component.scss',
  standalone: true
})
export class PlayerComponent implements OnDestroy {
  @Input() track: Track | null = null;
  
  isShowing = false;
  isPlaying = false;
  currentTime = 0;
  duration = 0;
  volume = 0.5;
  
  private audio: HTMLAudioElement | null = null;
  
  togglePlayer() {
    this.isShowing = !this.isShowing;
  }

  togglePlayPause() {
    if (!this.audio || !this.track) return;
    
    if (this.isPlaying) {
      this.pause();
    } else {
      this.play();
    }
  }

  play() {
    if (!this.audio || !this.track) return;
    
    if (!this.audio.src) {
      this.audio.src = this.track.fileUrl;
    }
    
    this.audio.play().then(() => {
      this.isPlaying = true;
    }).catch(error => {
      console.error('Error playing audio:', error);
    });
  }

  pause() {
    if (!this.audio) return;
    
    this.audio.pause();
    this.isPlaying = false;
  }

  stop() {
    if (!this.audio) return;
    
    this.audio.pause();
    this.audio.currentTime = 0;
    this.currentTime = 0;
    this.isPlaying = false;
  }

  setVolume(volume: number) {
    if (!this.audio) return;
    
    this.volume = Math.max(0, Math.min(1, volume));
    this.audio.volume = this.volume;
  }

  seek(time: number) {
    if (!this.audio) return;
    
    this.audio.currentTime = time;
    this.currentTime = time;
  }

  loadTrack(track: Track) {
    this.track = track;
    this.initializeAudio();
  }

  private initializeAudio() {
    if (!this.track) return;

    // Clean up existing audio
    if (this.audio) {
      this.audio.pause();
      this.audio.removeEventListener('timeupdate', this.onTimeUpdate.bind(this));
      this.audio.removeEventListener('loadedmetadata', this.onLoadedMetadata.bind(this));
      this.audio.removeEventListener('ended', this.onEnded.bind(this));
    }

    // Create new audio element
    this.audio = new Audio();
    this.audio.src = this.track.fileUrl;
    this.audio.volume = this.volume;

    // Add event listeners
    this.audio.addEventListener('timeupdate', this.onTimeUpdate.bind(this));
    this.audio.addEventListener('loadedmetadata', this.onLoadedMetadata.bind(this));
    this.audio.addEventListener('ended', this.onEnded.bind(this));
    this.audio.addEventListener('error', this.onError.bind(this));

    // Reset state
    this.currentTime = 0;
    this.isPlaying = false;
  }

  private onTimeUpdate() {
    if (this.audio) {
      this.currentTime = this.audio.currentTime;
    }
  }

  private onLoadedMetadata() {
    if (this.audio) {
      this.duration = this.audio.duration;
    }
  }

  private onEnded() {
    this.isPlaying = false;
    this.currentTime = 0;
  }

  private onError(error: any) {
    console.error('Audio error:', error);
    this.isPlaying = false;
  }

  ngOnDestroy() {
    if (this.audio) {
      this.audio.pause();
      this.audio.removeEventListener('timeupdate', this.onTimeUpdate.bind(this));
      this.audio.removeEventListener('loadedmetadata', this.onLoadedMetadata.bind(this));
      this.audio.removeEventListener('ended', this.onEnded.bind(this));
      this.audio.removeEventListener('error', this.onError.bind(this));
      this.audio = null;
    }
  }
}