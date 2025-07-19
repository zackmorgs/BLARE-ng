import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ReleaseService, Release } from '../../../../services/release.service';
import { TitleService } from '../../../../services/title.service';

@Component({
  selector: 'app-finish',
  imports: [CommonModule, FormsModule],
  templateUrl: './finish.component.html',
  styleUrl: './finish.component.scss'
})
export class FinishComponent implements OnInit {
  release: Release | null = null;
  isLoading = true;
  isSaving = false;
  errorMessage: string | null = null;
  successMessage: string | null = null;

  // Editable fields
  editableTrackNames: string[] = [];
  isPublic = false;

  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private releaseService = inject(ReleaseService);
  private titleService = inject(TitleService);

  ngOnInit() {
    const releaseId = this.route.snapshot.paramMap.get('releaseId');
    
    if (releaseId) {
      this.loadRelease(releaseId);
    } else {
      this.errorMessage = 'Invalid release ID';
      this.isLoading = false;
    }

    this.titleService.setTitle('Finish Release - BLARE');
  }

  private loadRelease(releaseId: string) {
    this.releaseService.getRelease(releaseId).subscribe({
      next: (release: Release) => {
        this.release = release;
        // Copy track names for editing
        this.editableTrackNames = [...(release.trackNames || [])];
        this.isPublic = release.isPublic || false;
        this.isLoading = false;
        
        // Update title with release info
        this.titleService.setTitle(`Finish Release: ${release.title} - BLARE`);
      },
      error: (error: any) => {
        console.error('Error loading release:', error);
        this.errorMessage = 'Failed to load release. Please try again.';
        this.isLoading = false;
      }
    });
  }

  saveChanges() {
    if (!this.release) return;

    this.isSaving = true;
    this.errorMessage = null;
    this.successMessage = null;

    // Prepare update data - we'll need to add these fields to UpdateReleaseRequest or create a new endpoint
    const updateData = {
      trackNames: this.editableTrackNames,
      isPublic: this.isPublic
    };

    // For now, let's use a direct API call since we need to update fields not in UpdateReleaseRequest
    // You might need to add a new endpoint in your API for this
    this.updateReleaseTrackTitles(this.release.id, updateData);
  }

  private updateReleaseTrackTitles(releaseId: string, updateData: any) {
    // Use the new updateReleaseMetadata method
    this.releaseService.updateReleaseMetadata(releaseId, updateData)
      .subscribe({
        next: (updatedRelease: Release) => {
          this.release = updatedRelease;
          this.successMessage = 'Changes saved successfully!';
          this.isSaving = false;
          
          // Clear success message after 3 seconds
          setTimeout(() => {
            this.successMessage = null;
          }, 3000);
        },
        error: (error: any) => {
          console.error('Error updating release:', error);
          this.errorMessage = error.error?.message || 'Failed to save changes. Please try again.';
          this.isSaving = false;
        }
      });
  }

  addTrack() {
    this.editableTrackNames.push('New Track');
  }

  removeTrack(index: number) {
    if (this.editableTrackNames.length > 1) {
      this.editableTrackNames.splice(index, 1);
    }
  }

  trackByIndex(index: number): number {
    return index;
  }

  goToRelease() {
    if (this.release) {
      this.router.navigate(['/play', this.release.artistSlug, this.release.releaseSlug]);
    }
  }

  goHome() {
    this.router.navigate(['/home']);
  }
}
