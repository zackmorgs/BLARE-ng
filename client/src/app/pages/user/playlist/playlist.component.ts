import { Component } from '@angular/core';

import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-playlist',
  imports: [],
  templateUrl: './playlist.component.html',
  styleUrl: './playlist.component.scss'
})
export class PlaylistComponent {
  usernameSlug : string;
  playlistSlug: string;

  constructor(private route: ActivatedRoute) {
    this.usernameSlug = this.route.snapshot.paramMap.get('usernameSlug')!;
    this.playlistSlug = this.route.snapshot.paramMap.get('playlistSlug')!;
  }
}
