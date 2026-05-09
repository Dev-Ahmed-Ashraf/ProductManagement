import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { StatisticsHttpService } from './statistics-http-service';
import { StatisticsResponse } from '../models/StatisticsResponse.model';
import { ApiResponse } from '../../../../../core/Models/GenericResponse.model';

@Injectable({
  providedIn: 'root',
})
export class StatisticsService {
  private http = inject(StatisticsHttpService);

  getStatistics(): Observable<ApiResponse<StatisticsResponse>> {
    return this.http.getStatistics();
  }
}
