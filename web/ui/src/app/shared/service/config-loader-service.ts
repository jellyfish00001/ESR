import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, Observable, tap, throwError } from 'rxjs';


interface Config {
  api: string;
  mcpApi: string;
  environment: string;
  // add other properties as needed
}
@Injectable({
  providedIn: 'root'
})
export class ConfigLoaderService {
  private config: Config | undefined;
  constructor(private http: HttpClient) { }
  loadConfig(): Observable<Config> {
    return this.http.get<Config>('/assets/config.json').pipe(
      tap(config => {
        // console.log('1-Configuration loaded:', config);
        this.config = config;
        if (config) {
          sessionStorage.setItem('config', JSON.stringify(this.config));
        }
      }),
      catchError((error) => {
        console.error('Failed to load configuration:', error);
        return throwError(error);
      })
    );
  }

  getConfig(): any {
    return this.config;
  }

  setConfig(config: Config): void {
    this.config = config;
  }
  getSettings<T>(key: string): T {
    if (!this.config) {
      throw new Error('Configuration has not been loaded.');
    }
    const keys = key.split('.');
    let result: any = this.config;

    for (const k of keys) {
      result = result[k];
      if (result === undefined) {
        throw new Error(`Configuration key "${key}" not found.`);
      }
    }

    return result as T;
  }

}
