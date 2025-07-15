import {Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';

export interface Tag {
    id: string;
    name: string;
}

export interface CreateTagRequest {
    name: string;
}

@Injectable({
    providedIn: 'root'
})

export class TagService 
{
    private http = inject(HttpClient);
    private apiUrl = 'http://localhost:5051/api/tag';

    getAllTags(): Observable<Tag[]> {
        return this.http.get<Tag[]>(this.apiUrl + "/all");
    }

    createTag(tagName: string): Observable<Tag> {
        return this.http.post<Tag>(this.apiUrl + "/create", { name: tagName });
    }
    
    searchTags(query: string): Observable<Tag[]> {
        return this.http.get<Tag[]>(`${this.apiUrl}/search?name=${encodeURIComponent(query)}`);
    }
    GetSome(): Observable<Tag[]> {
        return this.http.get<Tag[]>(`${this.apiUrl}/some`);
    }
}
