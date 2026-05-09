import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiResponse } from '../../../../core/Models/GenericResponse.model';
import { createProductRequest } from '../Models/create-product-request.model';
import { PagedResponse } from '../../../../core/Models/paged-response.model';
import { ProductListQuery } from '../Models/product-list-query.model';
import { ProductResponse } from '../Models/product-response.model';
import { ProductHttpService } from './ProductHttpService';
import { ChangeProductStatusResponse } from '../Models/ChangeProductStatusResponse.model';
import { ProductWhistoriesResponse } from '../Models/productWhistoriesResponse.model';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private productHttpService = inject(ProductHttpService);

  createProduct(data: createProductRequest): Observable<ApiResponse<ProductResponse>> {
    return this.productHttpService.addProduct(data);
  }

  getProducts(query: ProductListQuery): Observable<ApiResponse<PagedResponse<ProductResponse>>> {
    return this.productHttpService.getProducts(query);
  }

  getProductById(id: number): Observable<ApiResponse<ProductWhistoriesResponse>> {
    return this.productHttpService.getProductById(id);
  }

  deleteProduct(id: number): Observable<ApiResponse<null>> {
    return this.productHttpService.deleteProduct(id);
  }

  changeStatus(
    id: number,
    newStatus: number,
  ): Observable<ApiResponse<ChangeProductStatusResponse>> {
    return this.productHttpService.changeStatus(id, newStatus);
  }
}
