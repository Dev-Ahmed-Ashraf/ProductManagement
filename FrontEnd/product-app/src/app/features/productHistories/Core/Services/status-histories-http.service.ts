import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../core/Models/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ApiResponse } from '../../../../core/Models/GenericResponse.model';
import { PagedResponse } from '../../../../core/Models/paged-response.model';
import { ProductStatusHistory } from '../Models/product-status-histories.model';
import { ProductStatusHistoriesFilters } from '../Models/ProductStatusHistoriesFilters.model';

@Injectable({
  providedIn: 'root',
})
export class StatusHistoriesHttpService {
  private http = inject(HttpClient);

  private apiUrl = `${environment.apiUrl}/ProductStatusHistories`;

  getStatusHistories(filters: ProductStatusHistoriesFilters) {
    let params = new HttpParams()
      .set('pageNumber', filters.pageNumber)
      .set('pageSize', filters.pageSize);

    if (filters.productId !== undefined && filters.productId !== null) {
      params = params.set('productId', filters.productId);
    }

    if (filters.oldStatus !== undefined && filters.oldStatus !== null) {
      params = params.set('oldStatus', filters.oldStatus);
    }

    if (filters.newStatus !== undefined && filters.newStatus !== null) {
      params = params.set('newStatus', filters.newStatus);
    }

    if (filters.fromDate) {
      params = params.set('fromDate', filters.fromDate.toISOString());
    }

    if (filters.toDate) {
      params = params.set('toDate', filters.toDate.toISOString());
    }

    return this.http.get<ApiResponse<PagedResponse<ProductStatusHistory>>>(
      `${this.apiUrl}?${params}`,
    );
  }
}
