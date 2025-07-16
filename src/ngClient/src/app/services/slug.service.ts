import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class SlugService {
  constructor(private router: Router) {}

  /**
   * Convert a string to a URL-friendly slug
   */
  createSlug(text: string): string {
    return text
      .toLowerCase()
      .trim()
      .replace(/[^a-z0-9\s-]/g, '') // Remove special characters
      .replace(/\s+/g, '-') // Replace spaces with hyphens
      .replace(/-+/g, '-') // Replace multiple hyphens with single hyphen
      .replace(/^-|-$/g, ''); // Remove leading/trailing hyphens
  }

  /**
   * Convert a slug back to a readable title
   */
  slugToTitle(slug: string): string {
    return slug
      .split('-')
      .map(word => word.charAt(0).toUpperCase() + word.slice(1))
      .join(' ');
  }

  /**
   * Navigate to a release play page
   */
  navigateToRelease(artistName: string, releaseTitle: string): void {
    const artistSlug = this.createSlug(artistName);
    const releaseSlug = this.createSlug(releaseTitle);
    this.router.navigate(['/play', artistSlug, releaseSlug]);
  }

  /**
   * Navigate to a playlist play page
   */
  navigateToPlaylist(playlistName: string): void {
    const playlistSlug = this.createSlug(playlistName);
    this.router.navigate(['/play/playlist', playlistSlug]);
  }

  /**
   * Generate a shareable URL for a release
   */
  getReleaseUrl(artistName: string, releaseTitle: string): string {
    const artistSlug = this.createSlug(artistName);
    const releaseSlug = this.createSlug(releaseTitle);
    return `${window.location.origin}/play/${artistSlug}/${releaseSlug}`;
  }

  /**
   * Generate a shareable URL for a playlist
   */
  getPlaylistUrl(playlistName: string): string {
    const playlistSlug = this.createSlug(playlistName);
    return `${window.location.origin}/play/playlist/${playlistSlug}`;
  }

  /**
   * Validate if a string is a valid slug format
   */
  isValidSlug(slug: string): boolean {
    return /^[a-z0-9]+(?:-[a-z0-9]+)*$/.test(slug);
  }

  /**
   * Extract artist and release names from URL params
   */
  extractNamesFromSlugs(artistSlug: string, releaseSlug: string): { artist: string; release: string } {
    return {
      artist: this.slugToTitle(artistSlug),
      release: this.slugToTitle(releaseSlug)
    };
  }
}
