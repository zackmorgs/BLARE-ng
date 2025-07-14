import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';

export interface Release {
    id: string;
    type: string; // e.g., 'album', 'single', 'ep'
    title: string;
    coverImageUrl: string;
    artistID: string;
    description: string;
    version: string;
    releaseDate: Date;
    musicTags: string[];
    trackIds: string[];
}

export interface CreateReleaseRequest {
    type: string;
    title: string;
    coverImageUrl?: string;
    artistID: string;
    description?: string;
    version: string;
    releaseDate: Date;
    musicTags?: string[];
    trackIds?: string[];
}

export interface UpdateReleaseRequest {
    type?: string;
    title?: string;
    coverImageUrl?: string;
    description?: string;
    version?: string;
    releaseDate?: Date;
    musicTags?: string[];
    trackIds?: string[];
}

@Injectable({
    providedIn: 'root'
})
export class ReleaseService {
    private http = inject(HttpClient);
    private apiUrl = 'http://localhost:5051/api/release';
    
    // Cache for releases
    private releasesSubject = new BehaviorSubject<Release[]>([]);
    public releases$ = this.releasesSubject.asObservable();

    // Get all releases
    getAllReleases(): Observable<Release[]> {
        return this.http.get<Release[]>(this.apiUrl).pipe(
            tap(releases => this.releasesSubject.next(releases))
        );
    }

    // Get release by ID
    getReleaseById(id: string): Observable<Release> {
        return this.http.get<Release>(`${this.apiUrl}/artist/${id}`);
    }

    getReleasesByArtist(artistId: string): Observable<Release[]> {
        return this.http.get<Release[]>(`${this.apiUrl}/artist/${artistId}`);
    }

    // Get releases by artist
    // getReleasesByArtist(artistName: string): Observable<Release[]> {
    //     return this.http.get<Release[]>(`${this.apiUrl}/artist/${artistName}`);
    // }

    // Get releases by type (album, single, ep)
    getReleasesByType(type: string): Observable<Release[]> {
        return this.http.get<Release[]>(`${this.apiUrl}/type/${type}`);
    }

    // Search releases
    searchReleases(query: string): Observable<Release[]> {
        return this.http.get<Release[]>(`${this.apiUrl}/search?q=${encodeURIComponent(query)}`);
    }

    // Create new release
    createRelease(release: CreateReleaseRequest): Observable<Release> {
        return this.http.post<Release>(this.apiUrl, release).pipe(
            tap(newRelease => {
                const currentReleases = this.releasesSubject.value;
                this.releasesSubject.next([...currentReleases, newRelease]);
            })
        );
    }

    // Update release
    updateRelease(id: string, updates: UpdateReleaseRequest): Observable<Release> {
        return this.http.put<Release>(`${this.apiUrl}/${id}`, updates).pipe(
            tap(updatedRelease => {
                const currentReleases = this.releasesSubject.value;
                const index = currentReleases.findIndex(r => r.id === id);
                if (index !== -1) {
                    currentReleases[index] = updatedRelease;
                    this.releasesSubject.next([...currentReleases]);
                }
            })
        );
    }

    // Delete release
    deleteRelease(id: string): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
            tap(() => {
                const currentReleases = this.releasesSubject.value;
                const filteredReleases = currentReleases.filter(r => r.id !== id);
                this.releasesSubject.next(filteredReleases);
            })
        );
    }

    // Add track to release
    addTrackToRelease(releaseId: string, trackId: string): Observable<Release> {
        return this.http.post<Release>(`${this.apiUrl}/${releaseId}/tracks`, { trackId }).pipe(
            tap(updatedRelease => {
                const currentReleases = this.releasesSubject.value;
                const index = currentReleases.findIndex(r => r.id === releaseId);
                if (index !== -1) {
                    currentReleases[index] = updatedRelease;
                    this.releasesSubject.next([...currentReleases]);
                }
            })
        );
    }

    // Remove track from release
    removeTrackFromRelease(releaseId: string, trackId: string): Observable<Release> {
        return this.http.delete<Release>(`${this.apiUrl}/${releaseId}/tracks/${trackId}`).pipe(
            tap(updatedRelease => {
                const currentReleases = this.releasesSubject.value;
                const index = currentReleases.findIndex(r => r.id === releaseId);
                if (index !== -1) {
                    currentReleases[index] = updatedRelease;
                    this.releasesSubject.next([...currentReleases]);
                }
            })
        );
    }

    // Get recent releases
    getRecentReleases(): Observable<Release[]> {
        return this.http.get<Release[]>(`${this.apiUrl}/`);
    }

    // Get featured releases
    getFeaturedReleases(): Observable<Release[]> {
        return this.http.get<Release[]>(`${this.apiUrl}/featured`);
    }

    // Upload cover image (if you have file upload)
    uploadCoverImage(releaseId: string, file: File): Observable<{coverImageUrl: string}> {
        const formData = new FormData();
        formData.append('coverImage', file);
        return this.http.post<{coverImageUrl: string}>(`${this.apiUrl}/${releaseId}/cover`, formData);
    }

    // Helper method to refresh releases cache
    refreshReleases(): void {
        this.getAllReleases().subscribe();
    }

    // Helper method to clear cache
    clearCache(): void {
        this.releasesSubject.next([]);
    }

    // Get releases count by artist
    getReleasesCountByArtist(artistId: string): Observable<{count: number}> {
        return this.http.get<{count: number}>(`${this.apiUrl}/artist/${artistId}/count`);
    }

    // Validate release data
    validateRelease(release: CreateReleaseRequest | UpdateReleaseRequest): string[] {
        const errors: string[] = [];
        
        if ('title' in release && (!release.title || release.title.trim().length === 0)) {
            errors.push('Title is required');
        }
        
        if ('type' in release && release.type && !['album', 'single', 'ep'].includes(release.type)) {
            errors.push('Invalid release type. Must be album, single, or ep');
        }
        
        if ('releaseDate' in release && release.releaseDate && new Date(release.releaseDate) > new Date()) {
            errors.push('Release date cannot be in the future');
        }
        
        return errors;
    }
}

