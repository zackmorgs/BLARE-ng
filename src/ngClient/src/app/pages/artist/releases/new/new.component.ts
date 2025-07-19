import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ReleaseService, CreateReleaseRequest } from './../../../../services/release.service';
import { AuthService } from '../../../../services/auth.service';
import { UploaderComponent } from '../../../../components/uploader/uploader.component';
import { TitleService } from '../../../../services/title.service';

@Component({
  selector: 'app-new',
  imports: [CommonModule, ReactiveFormsModule, UploaderComponent],
  templateUrl: './new.component.html',
  styleUrl: './new.component.scss'
})

export class NewComponent implements OnInit {
  releaseForm: FormGroup;
  errorMessage: string | null = null;
  isSubmitting = false;
  selectedCoverImage: File | null = null;
  selectedAudioFiles: File[] = [];
  
  isUploading: boolean = false;

  private fb = inject(FormBuilder);
  private releaseService = inject(ReleaseService);
  private authService = inject(AuthService);
  private router = inject(Router);
  private titleService = inject(TitleService);

  constructor() {
    this.releaseForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(1)]],
      type: ['', [Validators.required]],
      description: [''],
      releaseDate: ['', [Validators.required]],
      musicTags: ['']
    });
  }

  ngOnInit() {
    // Set the page title
    this.titleService.setTitle('Create New Release - BLARE');
  }

  // Form field getters for validation
  get title() { return this.releaseForm.get('title'); }
  get type() { return this.releaseForm.get('type'); }
  get description() { return this.releaseForm.get('description'); }
  get releaseDate() { return this.releaseForm.get('releaseDate'); }
  get musicTags() { return this.releaseForm.get('musicTags'); }

  onCoverFileChange(event: any) {
    const files = event.target.files;
    if (files) {
      this.selectedCoverImage = files[0];
      console.log('Cover image selected:', files[0].name);
    } else {
      this.selectedCoverImage = null;
      console.log('No cover image selected');
    }
  }

  onAudioFileChange(event: any) {
    const files = Array.from(event.target.files) as File[];
    if (files.length > 0) {
      this.selectedAudioFiles = files;
      console.log('Audio files selected:', files.map(f => f.name));
    } else {
      this.selectedAudioFiles = [];
      console.log('No audio files selected');
    }
  }

  cancel() {
    this.router.navigate(['/home']);
  }

  onAlbumCreateSubmit() {
    this.isUploading = true;

    if (this.releaseForm.invalid || !this.selectedCoverImage || this.selectedAudioFiles.length == 0) {
      this.markFormGroupTouched();
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = null;

    const currentUser = this.authService.getCurrentUser();
    if (!currentUser) {
      this.errorMessage = 'You must be logged in to create a release';
      this.isSubmitting = false;
      return;
    }

    // Process music tags (split by comma and trim)
    const musicTags = this.musicTags?.value 
      ? this.musicTags.value.split(',').map((tag: string) => tag.trim()).filter((tag: string) => tag.length > 0)
      : [];

    const releaseData: CreateReleaseRequest = {
      title: this.title?.value,
      type: this.type?.value,
      description: this.description?.value || '',
      releaseDate: new Date(this.releaseDate?.value),
      artistId: currentUser.id,
      musicTags: musicTags,
      trackIds: [], // Will be populated when tracks are uploaded
      coverImage: this.selectedCoverImage,
      audioFiles: this.selectedAudioFiles
    };

    console.log('Creating release with data:', releaseData);

    this.releaseService.createRelease(releaseData).subscribe({
      next: (response) => {
        console.log('Release created successfully:', response);
        console.log('Response ID:', response.id);
        console.log('Response ID type:', typeof response.id);
        
        this.isSubmitting = false;

        if (response.id) {
          console.log('Attempting to navigate to:', '/artist/releases/new/finish/' + response.id);
          this.router.navigate(['/artist/releases/new/finish', response.id]).then(
            (success) => {
              console.log('Navigation success:', success);
              this.isUploading = false;
            },
            (error) => {
              console.error('Navigation error:', error);
              this.isUploading = false;
            }
          );
        } else {
          console.error('No ID in response, cannot navigate');
          this.errorMessage = 'Release created but cannot navigate to finish page. Missing ID in response.';
          this.isUploading = false;
        }
      },
      error: (error) => {
        console.error('Error creating release:', error);
        this.errorMessage = error.error?.message || 'Failed to create release. Please try again.';
        this.isSubmitting = false;
        this.isUploading = false;
      }
    });
  }

  private markFormGroupTouched() {
    Object.keys(this.releaseForm.controls).forEach(key => {
      const control = this.releaseForm.get(key);
      control?.markAsTouched();
    });
  }
}
