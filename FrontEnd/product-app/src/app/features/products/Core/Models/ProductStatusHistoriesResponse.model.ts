import { AuditResponse } from '../../../../core/Models/audit-response.model';

export interface ProductStatusHistoriesResponse extends AuditResponse {
  productId: number;
  oldStatus: number | undefined;
  newStatus: number | undefined;
}
