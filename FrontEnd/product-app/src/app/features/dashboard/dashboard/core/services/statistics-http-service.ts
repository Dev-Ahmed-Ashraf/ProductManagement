import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../core/Models/environment';
import { ApiResponse } from '../../../../../core/Models/GenericResponse.model';
import { Observable } from 'rxjs';
import { StatisticsResponse } from '../models/StatisticsResponse.model';

@Injectable({
  providedIn: 'root',
})
export class StatisticsHttpService {
  private http = inject(HttpClient);

  private apiUrl = `${environment.apiUrl}/statistics`;

  getStatistics(): Observable<ApiResponse<StatisticsResponse>> {
    return this.http.get<ApiResponse<StatisticsResponse>>(`${this.apiUrl}`);
  }
}
