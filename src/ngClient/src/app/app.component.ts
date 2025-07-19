import { Component, inject } from '@angular/core';
import { RouterOutlet, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { TopNav } from './components/top-nav/top-nav.component';
import { AuthService } from './services/auth.service';
import { PlayerComponent } from './components/player/player.component';
import { TitleService } from './services/title.service';
import { PlayerService } from './services/player.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, TopNav, CommonModule, PlayerComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'client';
  authService = inject(AuthService);
  titleService = inject(TitleService); // This initializes the title service
  playerService = inject(PlayerService); // This initializes the player service
  playerShowing: boolean = false;

}


 