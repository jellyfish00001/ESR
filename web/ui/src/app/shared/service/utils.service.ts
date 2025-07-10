import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class UtilsService {
  configFile: string = '/assets/config.json?nocache=' + (new Date()).getTime();
  
  constructor(private http: HttpClient) { }
  rawCompare(a, b): boolean {
    return JSON.stringify(a) === JSON.stringify(b);  } 
  getConfig() {
    return this.http.get(this.configFile);
  }
}
