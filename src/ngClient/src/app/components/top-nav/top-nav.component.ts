import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { ConnectionHealthService } from '../../services/connectionhealth.service';

@Component({
  selector: 'top-nav',
  imports: [RouterLink, CommonModule],
  templateUrl: './top-nav.component.html',
  styleUrl: './top-nav.component.scss'
})

export class TopNav {
  navOpen = false;
  authService = inject(AuthService);
  isOnline = true;
  $connectionHealthService = inject(ConnectionHealthService);

  ngOnInit(): void {
    // Initialize any necessary data or state here
    window.addEventListener('online', () => {
      this.isOnline = true;
    });
    window.addEventListener('offline', () => {
      this.isOnline = false;
    });
    this.isOnline = navigator.onLine; // Set initial online status
  }


  get navIconSrc(): string {
    return this.navOpen ? '/svg/icon-close.svg' : '/svg/icon-settings.svg';
  }

  get navIconAlt(): string {
    return this.navOpen ? 'Close Icon' : 'Menu Icon';
  }

  toggleNav(): void {
    this.navOpen = !this.navOpen;
  }

  closeNav(): void {
    this.navOpen = false;
  }

  getUsername(): string {
    const currentUser = this.authService.getCurrentUser();
    return currentUser ? currentUser.username : '';
  }
}
