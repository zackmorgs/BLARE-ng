import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from './../../services/auth.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  
})

export class HomeComponent {
  title = 'BLARE';
  authService = inject(AuthService);
  constructor() {
  
    // You can inject services or perform any initialization here
    console.log(this.authService.getCurrentUser()?.role);
  }
}