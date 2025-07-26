import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ReleaseService, CreateReleaseRequest } from './../../../../services/release.service';
import { AuthService } from '../../../../services/auth.service';
import { UploaderComponent } from '../../../../components/uploader/uploader.component';
import { TitleService } from '../../../../services/title.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-new',
  imports: [CommonModule, ReactiveFormsModule, UploaderComponent],
  templateUrl: './new.component.html',
  styleUrl: './new.component.scss'
})

export class NewComponent implements OnInit, OnDestroy {
  releaseForm: FormGroup;
  errorMessage: string | null = null;
  isSubmitting = false;
  selectedCoverImage: File | null = null;
  selectedAudioFiles: File[] = [];

  isUploading: boolean = false;
  uploadProgress: { [fileName: string]: number } = {}; // Track progress for each file
  uploadedFiles: string[] = []; // Track completed uploads

  private progressSubscription?: Subscription;

  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  }

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
    
    // Subscribe to upload progress
    this.progressSubscription = this.releaseService.uploadProgress$.subscribe(
      (progressEvent) => {
        this.updateUploadProgress(progressEvent.fileName, progressEvent.progress);
      }
    );
  }

  ngOnDestroy() {
    if (this.progressSubscription) {
      this.progressSubscription.unsubscribe();
    }
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
      // Initialize progress tracking for each file
      this.uploadProgress = {};
      this.uploadedFiles = [];
      files.forEach(file => {
        this.uploadProgress[file.name] = 0;
      });
      console.log('Audio files selected:', files.map(f => f.name));
    } else {
      this.selectedAudioFiles = [];
      this.uploadProgress = {};
      this.uploadedFiles = [];
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
      this.isUploading = false;
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = null;

    const currentUser = this.authService.getCurrentUser();
    if (!currentUser) {
      this.errorMessage = 'You must be logged in to create a release';
      this.isSubmitting = false;
      this.isUploading = false;
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
      trackIds: [],
      coverImage: this.selectedCoverImage,
      audioFiles: this.selectedAudioFiles
    };

    console.log('Creating release with data:', releaseData);

    // Use the progress-enabled method
    this.releaseService.createReleaseWithProgress(releaseData).subscribe({
      next: (response) => {
        console.log('Release created successfully:', response);
        this.isSubmitting = false;

        if (response.id) {
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

  updateUploadProgress(fileName: string, progress: number) {
    this.uploadProgress[fileName] = progress;
    if (progress === 100 && !this.uploadedFiles.includes(fileName)) {
      this.uploadedFiles.push(fileName);
    }
  }
}
