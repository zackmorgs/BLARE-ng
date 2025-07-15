import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { AuthService, User, UpdateProfileRequest } from './../../../services/auth.service';

@Component({
  selector: 'app-account',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './account.component.html',
  styleUrl: './account.component.scss'
})
export class AccountComponent implements OnInit {
  private fb = inject(FormBuilder);
  username: string = '';
  authService = inject(AuthService);
  profileForm: FormGroup;
  userProfile: User | null = null;
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  constructor(private route: ActivatedRoute) {
    this.username = this.route.snapshot.paramMap.get('usernameSlug') || '';
    
    // Initialize the form
    this.profileForm = this.fb.group({
      firstName: ['', [Validators.maxLength(50)]],
      lastName: ['', [Validators.maxLength(50)]],
      bio: ['', [Validators.maxLength(500)]]
    });
  }

  ngOnInit(): void {
    this.loadUserProfile();
  }

  // Check if this is the current user's own account
  get isOwnAccount(): boolean {
    const currentUser = this.authService.getCurrentUser();
    return currentUser?.username === this.username;
  }

  private loadUserProfile(): void {
    if (this.isOwnAccount) {
      // Load current user data
      const currentUser = this.authService.getCurrentUser();
      if (currentUser) {
        this.userProfile = currentUser;
        this.populateForm(currentUser);
      }
    } else {
      // Load public profile data
      this.isLoading = true;
      this.authService.getUserProfile(this.username).subscribe({
        next: (user) => {
          this.userProfile = user;
          this.isLoading = false;
        },
        error: (error) => {
          this.errorMessage = 'Failed to load user profile';
          this.isLoading = false;
        }
      });
    }
  }

  private populateForm(user: User): void {
    this.profileForm.patchValue({
      firstName: user.firstName || '',
      lastName: user.lastName || '',
      bio: user.bio || ''
    });
  }

  updateAccount(): void {
    if (!this.profileForm.valid || !this.isOwnAccount) {
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    const profileData: UpdateProfileRequest = this.profileForm.value;

    this.authService.updateProfile(profileData).subscribe({
      next: (updatedUser) => {
        this.userProfile = updatedUser;
        this.successMessage = 'Profile updated successfully!';
        this.isLoading = false;
        
        // Clear success message after 3 seconds
        setTimeout(() => {
          this.successMessage = '';
        }, 3000);
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Failed to update profile';
        this.isLoading = false;
      }
    });
  }

  // Form getters for validation
  get firstName() { return this.profileForm.get('firstName'); }
  get lastName() { return this.profileForm.get('lastName'); }
  get bio() { return this.profileForm.get('bio'); }
}
