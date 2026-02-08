import { Component, signal, OnInit, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from './services/auth.service';
import { AiProviderService } from 'eltocardo-api-sdk';
import { AiProviderDto } from 'eltocardo-api-sdk';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit {
  protected readonly title = signal('ElTocardo.Assistant');
  protected readonly authService = inject(AuthService);
  protected readonly aiProviderService = inject(AiProviderService);
  
  protected aiProviders = signal<AiProviderDto[]>([]);
  protected loading = signal(false);
  protected error = signal<string | null>(null);

  ngOnInit(): void {
    // Wait a bit for auth to initialize
    setTimeout(() => {
      if (this.authService.hasValidAccessToken()) {
        this.loadAiProviders();
      }
    }, 1000);
  }

  protected login(): void {
    this.authService.login();
  }

  protected logout(): void {
    this.authService.logout();
  }

  protected loadAiProviders(): void {
    this.loading.set(true);
    this.error.set(null);
    
    this.aiProviderService.getAllAiProvider().subscribe({
      next: (providers) => {
        this.aiProviders.set(providers);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading AI providers:', err);
        this.error.set('Failed to load AI providers. Please try again.');
        this.loading.set(false);
      }
    });
  }

  protected get isAuthenticated(): boolean {
    return this.authService.hasValidAccessToken();
  }
}
