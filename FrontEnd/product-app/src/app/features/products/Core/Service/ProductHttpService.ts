import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../core/Models/environment';
import { createProductRequest } from '../Models/create-product-request.model';
import { ApiResponse } from '../../../../core/Models/GenericResponse.model';
import { ProductResponse } from '../Models/product-response.model';
import { ProductListQuery } from '../Models/product-list-query.model';
import { PagedResponse } from '../../../../core/Models/paged-response.model';
import { ChangeProductStatusResponse } from '../Models/ChangeProductStatusResponse.model';
import { ProductWhistoriesResponse } from '../Models/productWhistoriesResponse.model';

@Injectable({
  providedIn: 'root',
})
export class ProductHttpService {
  private http = inject(HttpClient);

  private apiUrl = `${environment.apiUrl}/Product`;

  addProduct(data: createProductRequest) {
    return this.http.post<ApiResponse<ProductResponse>>(`${this.apiUrl}`, data);
  }

  getProducts(query: ProductListQuery) {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber)
      .set('pageSize', query.pageSize);

    if (query.name) {
      params = params.set('name', query.name);
    }

    if (query.description) {
      params = params.set('description', query.description);
    }

    if (query.price !== null && query.price !== undefined) {
      params = params.set('price', query.price);
    }

    if (query.quantity !== null && query.quantity !== undefined) {
      params = params.set('quantity', query.quantity);
    }

    return this.http.get<ApiResponse<PagedResponse<ProductResponse>>>(this.apiUrl, { params });
  }

  getProductById(id: number) {
    return this.http.get<ApiResponse<ProductWhistoriesResponse>>(`${this.apiUrl}/${id}`);
  }

  deleteProduct(id: number) {
    return this.http.delete<ApiResponse<null>>(`${this.apiUrl}/${id}`);
  }

  changeStatus(id: number, newStatus: number) {
    return this.http.patch<ApiResponse<ChangeProductStatusResponse>>(
      `${this.apiUrl}/${id}/status`,
      {
        newStatus: newStatus,
      },
    );
  }
}
