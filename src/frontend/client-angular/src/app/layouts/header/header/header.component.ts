import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent {
  isProfileMenuOpen = false;
  currentUsername?: string;

  constructor(
    public authService: AuthService,
    private router: Router
  ) {
    this.currentUsername = this.authService.getCurrentUsername();
  }

  toggleProfileMenu(): void {
    this.isProfileMenuOpen = !this.isProfileMenuOpen;
  }

  logout(): void {
    if (!this.currentUsername) return;
    
    const logoutData = { userName: this.currentUsername };
    
    this.authService.logout(logoutData).subscribe({
      next: () => {
        this.router.navigate(['/']);
        this.isProfileMenuOpen = false;
      },
      error: (err) => {
        console.error('Logout failed:', err);
        this.authService.clearTokens();
        this.router.navigate(['/']);
      }
    });
  }

  navigateToLogin(): void {
    this.router.navigate(['/login']);
  }
}