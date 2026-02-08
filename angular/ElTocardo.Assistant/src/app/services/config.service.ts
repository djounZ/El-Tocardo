import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';

export interface AppConfig {
  auth: {
    issuer: string;
    scope: string;
    clientId: string;
  };
  api: {
    basePath: string;
  };
}

@Injectable({
  providedIn: 'root'
})
export class ConfigService {
  private config?: AppConfig;

  constructor(private http: HttpClient) {}

  async loadConfig(): Promise<void> {
    try {
      this.config = await firstValueFrom(
        this.http.get<AppConfig>('/config.json')
      );
      console.log('Configuration loaded:', this.config);
    } catch (error) {
      console.error('Failed to load configuration:', error);
      throw error;
    }
  }

  get authIssuer(): string  {
    return this.config?.auth.issuer || '';
  }

  get authScope(): string {
    return this.config?.auth.scope || '';
  }

  get authClientId(): string {
    return this.config?.auth.clientId || '';
  }

  get apiBasePath(): string {
    return this.config?.api.basePath || '';
  }

  getConfig(): AppConfig | undefined {
    return this.config;
  }
}
