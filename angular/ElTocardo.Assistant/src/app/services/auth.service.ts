import { Injectable } from '@angular/core';
import { OAuthService, AuthConfig } from 'angular-oauth2-oidc';
import { filter } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private authConfig: AuthConfig = {
    // Your OIDC provider URL
    issuer: 'http://localhost:5095/',
    
    // The SPA's id. Register SPA with this id at the auth-server
    clientId: 'ElTocardoAssistant',
    
    // URL of the SPA to redirect the user to after login
    redirectUri: window.location.origin,
    
    // The SPA's redirectUri where the token response is sent
    responseType: 'code',
    
    // Set the scope for the permissions the client should request
    scope: 'openid profile email eltocardoapiuser',
    
    // Use PKCE
    useSilentRefresh: true,
    showDebugInformation: true,
    
    // Disable at_hash check as we are using Code Flow with PKCE
    disableAtHashCheck: true,
    
    // Require HTTPS for issuer
    requireHttps: false,
  };

  constructor(private oauthService: OAuthService) {
    this.configure();
  }

  private configure(): void {
    this.oauthService.configure(this.authConfig);
    this.oauthService.setupAutomaticSilentRefresh();
    
    // Load Discovery Document and try to login
    this.oauthService.loadDiscoveryDocumentAndTryLogin().then(() => {
      if (this.oauthService.hasValidAccessToken()) {
        console.log('User is authenticated');
      } else {
        console.log('User is not authenticated');
      }
    });

    // Optional: Listen to token events
    this.oauthService.events
      .pipe(filter(e => e.type === 'token_received'))
      .subscribe(() => {
        console.log('Access token received:', this.oauthService.getAccessToken());
      });
  }

  public login(): void {
    this.oauthService.initCodeFlow();
  }

  public logout(): void {
    this.oauthService.logOut();
  }

  public getAccessToken(): string | null {
    return this.oauthService.getAccessToken();
  }

  public hasValidAccessToken(): boolean {
    return this.oauthService.hasValidAccessToken();
  }

  public get identityClaims(): any {
    return this.oauthService.getIdentityClaims();
  }
}
