import { BaseResponse } from './base-response.model';

export interface AuditResponse extends BaseResponse {
  createdAt: Date;
  createdBy: string;
}
