import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-nav',
  imports: [ RouterLink],
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.scss'
})
export class NavComponent {
  navOpen = false;

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
