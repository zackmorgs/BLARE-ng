import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';

export interface Artist {
    id: string;
    name: string;
    slug: string;
}

export interface Release {
    id: string;
    type: string; // e.g., 'album', 'single', 'ep'
    title: string;
    coverImageUrl: string;
    artistId: string;
    description: string;
    releaseDate: Date;
    musicTags: string[];
    trackIds: string[];
    releaseSlug: string;
    artistSlug: string;
    trackUrls: string[];
    trackNames: string[];
    isPublic: boolean;
}

export interface CreateReleaseRequest {
    type: string;
    title: string;
    coverImage?: File;
    artistId: string;
    description?: string;
    releaseDate: Date;
    musicTags?: string[];
    trackIds?: string[];
    audioFiles?: File[];
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
    private releaseApiUrl = 'http://localhost:5051/api/release';
    private releaseUploadUrl = 'http://localhost:5051/api/release/upload';

    // Cache for releases
    private releasesSubject = new BehaviorSubject<Release[]>([]);
    public releases$ = this.releasesSubject.asObservable();

    // Get all releases
    getAllReleases(): Observable<Release[]> {
        return this.http.get<Release[]>(this.releaseApiUrl).pipe(
            tap(releases => this.releasesSubject.next(releases))
        );
    }

    // Get release by ID
    getReleaseById(id: string): Observable<Release> {
        return this.http.get<Release>(`${this.releaseApiUrl}/artist/${id}`);
    }

    // Get release by ID
    getRelease(id: string): Observable<Release> {
        return this.http.get<Release>(`${this.releaseApiUrl}/${id}`);
    }

    getReleasesByArtist(artistId: string): Observable<Release[]> {
        return this.http.get<Release[]>(`${this.releaseApiUrl}/artist/${artistId}/all`);
    }

    // Get releases by artist
    // getReleasesByArtist(artistName: string): Observable<Release[]> {
    //     return this.http.get<Release[]>(`${this.apiUrl}/artist/${artistName}`);
    // }

    // Get releases by type (album, single, ep)
    getReleasesByType(type: string): Observable<Release[]> {
        return this.http.get<Release[]>(`${this.releaseApiUrl}/type/${type}`);
    }

    // Search releases
    searchReleases(query: string): Observable<Release[]> {
        return this.http.get<Release[]>(`${this.releaseApiUrl}/search?q=${encodeURIComponent(query)}`);
    }

    // Create new release
    createRelease(release: CreateReleaseRequest): Observable<Release> {
        const formData = new FormData();
        
        // Basic release metadata
        formData.append('title', release.title);
        formData.append('type', release.type);
        formData.append('artistId', release.artistId);
        formData.append('description', release.description || '');
        formData.append('releaseDate', release.releaseDate.toISOString());
        
        // Music tags array
        release.musicTags?.forEach(tag => formData.append('musicTags[]', tag));

        // Track IDs array
        // release.trackIds?.forEach(id => formData.append('trackIds[]', id));

        // Cover image file
        if (release.coverImage) {
            formData.append('coverImage', release.coverImage);
        }
        
        // Audio files array
        release.audioFiles?.forEach(file => {
            formData.append('releaseFiles', file);
        });

        return this.http.put<Release>(this.releaseUploadUrl, formData);
    }

    // Update release
    updateRelease(id: string, updates: UpdateReleaseRequest): Observable<Release> {
        return this.http.put<Release>(`${this.releaseApiUrl}/${id}`, updates).pipe(
            tap(updatedRelease => {
                const currentReleases = this.releasesSubject.value;
                const index = currentReleases.findIndex(r => r.id === id);``
                if (index !== -1) {
                    currentReleases[index] = updatedRelease;
                    this.releasesSubject.next([...currentReleases]);
                }
            })
        );
    }

    // Update track names and public status
    updateReleaseMetadata(releaseId: string, metadata: { trackNames?: string[], isPublic?: boolean }): Observable<Release> {
        return this.http.put<Release>(`${this.releaseApiUrl}/${releaseId}/metadata`, metadata);
    }

    // Delete release
    deleteRelease(id: string): Observable<void> {
        return this.http.delete<void>(`${this.releaseApiUrl}/${id}`).pipe(
            tap(() => {
                const currentReleases = this.releasesSubject.value;
                const filteredReleases = currentReleases.filter(r => r.id !== id);
                this.releasesSubject.next(filteredReleases);
            })
        );
    }

    // Add track to release
    addTrackToRelease(releaseId: string, trackId: string): Observable<Release> {
        return this.http.post<Release>(`${this.releaseApiUrl}/${releaseId}/tracks`, { trackId }).pipe(
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
        return this.http.delete<Release>(`${this.releaseApiUrl}/${releaseId}/track/${trackId}`).pipe(
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
        return this.http.get<Release[]>(`${this.releaseApiUrl}/`);
    }

    // Get featured releases
    getFeaturedReleases(): Observable<Release[]> {
        return this.http.get<Release[]>(`${this.releaseApiUrl}/featured`);
    }

    // Upload cover image (if you have file upload)
    uploadCoverImage(releaseId: string, file: File): Observable<{coverImageUrl: string}> {
        const formData = new FormData();
        formData.append('coverImage', file);
        return this.http.post<{coverImageUrl: string}>(`${this.releaseApiUrl}/${releaseId}/cover`, formData);
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
        return this.http.get<{count: number}>(`${this.releaseApiUrl}/artist/${artistId}/count`);
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

    getReleaseBySlugs(artistSlug: string, releaseSlug: string): Observable<Release> {
        return this.http.get<Release>(`${this.releaseApiUrl}/artist/${artistSlug}/title/${releaseSlug}`);
    }

    getArtistById(artistId: string): Observable<Artist> {
        return this.http.get<Artist>(`${this.releaseApiUrl}/artist/${artistId}`);
    }

    getArtistNameByAlbumId(artistId: string): Observable<string> {
        return this.http.get<string>(`${this.releaseApiUrl}/artist-name/album/${artistId}`);
    }

}