import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { TitleService } from '../../services/title.service';
import { Artist, Release, ReleaseService } from '../../services/release.service';
import { LoaderComponent } from '../../components/loader/loader.component';
import { PlayerService, Track } from '../../services/player.service';
import { TrackOptionsComponent } from '../../components/track-options/track-options.component';

@Component({
  selector: 'app-play',
  imports: [CommonModule, LoaderComponent, TrackOptionsComponent],
  templateUrl: './play.component.html',
  styleUrl: './play.component.scss'
})
export class PlayComponent implements OnInit {
  releaseIsLoading: boolean = true;
  artistIsLoading: boolean = true;

  isLoading: boolean = true;

  artistSlug: string | null = null;
  releaseSlug: string | null = null;
  $releaseService: ReleaseService;

  releaseToPlay!: Release;
  artistToPlay!: Artist;
  currentTrackIndex: number = 0; // Track index for the currently playing track
  currentTrack: { name: string } | null = null; // Current track object

  showTrackOptions: boolean = false; // Flag to control track options visibility

  constructor(
    private route: ActivatedRoute,
    private titleService: TitleService,
    private playerService: PlayerService,
    releaseService: ReleaseService
  ) {
    this.$releaseService = releaseService;
  }

  ngOnInit(): void {
    // Get the slugs from the route parameters
    this.artistSlug = this.route.snapshot.paramMap.get('artistSlug');
    this.releaseSlug = this.route.snapshot.paramMap.get('releaseSlug');

    this.loadRelease(this.artistSlug, this.releaseSlug);
  }
  loadArtist(artistID: string): void {
    if (artistID) {
      // Logic to load the artist based on the ID
      console.log(`Loading artist: ${artistID}`);
      this.$releaseService.getArtistById(artistID).subscribe({
        next: (artist) => {
          console.log('Artist loaded:', artist);
          // You can handle the loaded artist data here
          this.artistToPlay = artist;
          this.artistIsLoading = false;
        },
        error: (error) => {
          console.error('Error loading artist:', error);
        }
      });
      // // Set the page title
      // this.titleService.setTitle(`Playing Artist - ${artistSlug}`);
      // Additional logic to fetch and display the artist details can be added here
      this.artistIsLoading = true;
      this.isLoading = false;
    } else {
      console.error('Artist slug is missing');
    }
  }

  loadRelease(artistSlug: string | null, releaseSlug: string | null): void {
    if (artistSlug && releaseSlug) {
      // Logic to load the release based on the slugs
      console.log(`Loading release for artist: ${artistSlug}, release: ${releaseSlug}`);

      this.$releaseService.getReleaseBySlugs(artistSlug, releaseSlug).subscribe({
        next: (release) => {
          console.log('Release loaded:', release);
          // You can handle the loaded release data here
          this.releaseToPlay = release;

          this.loadArtist(release.artistId);
          this.releaseIsLoading = false;
        },
        error: (error) => {
          console.error('Error loading release:', error);
        }
      });

      // Set the page title
      this.titleService.setTitle(`Playing Release - ${artistSlug} - ${releaseSlug}`);

      // Additional logic to fetch and display the release details can be added here
    } else {
      console.error('Artist or release slug is missing');
    }
  }

  playTrack(trackName: string, trackIndex: number) {
    const trackUrl = this.releaseToPlay.trackUrls[trackIndex];
    console.log('Playing:', trackName, 'from:', trackUrl);

    // Update current track index and track
    this.currentTrackIndex = trackIndex;
    this.currentTrack = { name: trackName };

    // Create track object for player service
    const track: Track = {
      id: `${this.releaseToPlay.id}_${trackIndex}`, // Create unique ID
      title: trackName,
      artist: this.artistToPlay?.name || 'Unknown Artist',
      fileUrl: `http://localhost:5051${trackUrl}`, // Full URL for audio
      duration: 0, // You can add duration if available
      albumArt: this.releaseToPlay.coverImageUrl ? `http://localhost:5051${this.releaseToPlay.coverImageUrl}` : undefined
    };

    // Create playlist from all tracks in the release
    const playlist: Track[] = this.releaseToPlay.trackNames.map((name, index) => ({
      id: `${this.releaseToPlay.id}_${index}`,
      title: name,
      artist: this.artistToPlay?.name || 'Unknown Artist',
      fileUrl: `http://localhost:5051${this.releaseToPlay.trackUrls[index]}`,
      duration: 0,
      albumArt: this.releaseToPlay.coverImageUrl ? `http://localhost:5051${this.releaseToPlay.coverImageUrl}` : undefined
    }));

    // Use player service to play the track
    this.playerService.playTrackFromPlaylist(playlist, trackIndex);
  }

  handleTrackOptionClick(event: MouseEvent, trackName: string, trackIndex: number): void {
    event.stopPropagation(); // Prevent click from propagating to the track item
    this.currentTrack = { name: trackName };
    this.currentTrackIndex = trackIndex;
    this.showTrackOptions = true;
  }

  handleTrackOption(option: string): void {
    console.log(`Track option selected: ${option} for track: ${this.currentTrack?.name} at index: ${this.currentTrackIndex}`);
    
    switch (option) {
      case 'playlist':
        // Handle add to playlist logic
        console.log('Adding to playlist...');
        break;
      case 'favorites':
        // Handle add to favorites logic
        console.log('Adding to favorites...');
        break;
      case 'share':
        // Handle share logic
        console.log('Sharing track...');
        break;
      case 'download':
        // Handle download logic
        console.log('Downloading track...');
        break;
      case 'info':
        // Handle track info logic
        console.log('Showing track info...');
        break;
      case 'close':
        // Close the options modal
        this.showTrackOptions = false;
        break;
      default:
        console.log('Unknown option:', option);
    }
    
    // Close the options modal for all options except 'close' (already handled above)
    if (option !== 'close') {
      this.showTrackOptions = false;
    }
  }
}
