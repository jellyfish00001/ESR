import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, switchMap } from 'rxjs/operators';
import { BehaviorSubject, EMPTY, Observable } from 'rxjs';
import _ from 'lodash';
import { Router } from '@angular/router';
import { URLConst } from "../const/url.const";
import { WebApiService } from './webapi.service';
import { EnvironmentconfigService } from '../../shared/service/environmentconfig.service';

@Injectable({
  providedIn: 'root'
})
export class PermissionService {

  constructor(
    private httpclient: HttpClient,
    private router: Router,
        private Service: WebApiService,
        private EnvironmentconfigService: EnvironmentconfigService,
    // private toDoListService: ToDoListService,
  ) {}
  getPermission(roleKeys): Observable<any> {
     return this.Service.doPost(
          this.EnvironmentconfigService.authConfig.ersUrl + URLConst.GetAuth,
          roleKeys
        ).pipe(
          map((res) => res || false),
        )
  }

  checkPermission(roleKeys) {
    return this.getPermission(roleKeys)
     
  }
}
