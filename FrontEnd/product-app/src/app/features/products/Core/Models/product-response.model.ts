import { AuditResponse } from '../../../../core/Models/audit-response.model';

export interface ProductResponse extends AuditResponse {
  name: string;
  description: string | null;
  price: number;
  quantity: number;
  status: number;
}
