import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiResponse } from '../../../../core/Models/GenericResponse.model';
import { PagedResponse } from '../../../../core/Models/paged-response.model';
import { ProductStatusHistory } from '../Models/product-status-histories.model';
import { StatusHistoriesHttpService } from './status-histories-http.service';
import { ProductStatusHistoriesFilters } from '../Models/ProductStatusHistoriesFilters.model';

@Injectable({
  providedIn: 'root',
})
export class StatusHistoriesService {
  private http = inject(StatusHistoriesHttpService);

  getStatusHistoriesByProductId(
    params: ProductStatusHistoriesFilters,
  ): Observable<ApiResponse<PagedResponse<ProductStatusHistory>>> {
    return this.http.getStatusHistories(params);
  }
}
