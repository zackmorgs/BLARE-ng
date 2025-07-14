import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-nav',
  imports: [RouterLink, CommonModule],
  templateUrl: './nav-settings.component.html',
  styleUrl: './nav-settings.component.scss'
})
export class NavSettings {
  navOpen = false;
  authService = inject(AuthService);

  get navIconSrc(): string {
    return this.navOpen ? '/svg/icon-close.svg' : '/svg/icon-settings.svg';
  }

  get navIconAlt(): string {
    return this.navOpen ? 'Close Icon' : 'Menu Icon';
  }

  toggleNav(): void {
    this.navOpen = !this.navOpen;
  }
}
