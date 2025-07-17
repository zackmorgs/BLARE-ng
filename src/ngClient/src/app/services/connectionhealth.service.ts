import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';

export interface ConnectionHealth {
    status: 'healthy' | 'unhealthy';
    timestamp: Date;
}

export interface ConnectionHealthResponse {
    status: any;
    timestamp: Date;
}

@Injectable({
    providedIn: 'root'
})

export class ConnectionHealthService {
    private http = inject(HttpClient);
    private healthApiUrl = 'http://localhost:5051/api/health';

    private healthSubject = new BehaviorSubject<ConnectionHealthResponse | null>(null);
    public health$ = this.healthSubject.asObservable();
    // Get connection health status
    getConnectionHealth(): Observable<ConnectionHealthResponse> {
        return this.http.get<ConnectionHealthResponse>(this.healthApiUrl).pipe(
            tap(response => this.healthSubject.next(response))
        );
    }
}    