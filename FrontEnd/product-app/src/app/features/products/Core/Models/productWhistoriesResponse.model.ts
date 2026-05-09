import { ProductResponse } from './product-response.model';
import { ProductStatusHistoriesResponse } from './ProductStatusHistoriesResponse.model';

export interface ProductWhistoriesResponse extends ProductResponse {
  history: ProductStatusHistoriesResponse[];
}
