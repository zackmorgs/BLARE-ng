import { Component, inject } from '@angular/core';
import { RouterOutlet, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NavSettings } from './components/nav-settings/nav-settings.component';
import { AuthService } from './services/auth.service';
import { PlayerComponent } from './components/player/player.component';
import { TitleService } from './services/title.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NavSettings, CommonModule, PlayerComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'client';
  authService = inject(AuthService);
  titleService = inject(TitleService); // This initializes the title service
}
