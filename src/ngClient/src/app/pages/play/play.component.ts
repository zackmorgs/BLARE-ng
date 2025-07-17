import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-play',
  imports: [CommonModule],
  templateUrl: './play.component.html',
  styleUrl: './play.component.scss'
})
export class PlayComponent implements OnInit {
  artistSlug: string | null = null;
  releaseSlug: string | null = null;

  constructor(private route: ActivatedRoute) {
    // Initialization logic can go here
  }

  ngOnInit(): void {
    // Get the slugs from the route parameters
    this.artistSlug = this.route.snapshot.paramMap.get('artistSlug');
    this.releaseSlug = this.route.snapshot.paramMap.get('releaseSlug');
    
    console.log('Artist Slug:', this.artistSlug);
    console.log('Release Slug:', this.releaseSlug);
  }
}
