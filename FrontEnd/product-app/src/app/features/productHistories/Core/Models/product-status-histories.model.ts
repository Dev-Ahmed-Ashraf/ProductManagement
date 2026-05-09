import { AuditResponse } from '../../../../core/Models/audit-response.model';

export interface ProductStatusHistory extends AuditResponse {
  productId: number;
  productName: string;
  oldStatus: number;
  newStatus: number;
}
