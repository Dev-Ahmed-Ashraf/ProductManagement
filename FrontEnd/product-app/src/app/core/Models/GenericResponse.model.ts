export interface ApiResponse<T = any> {
  success: boolean;
  message?: string;
  data?: T;
  errors?: string[];
  statusCode?: number;
  HttpStatusCode?: number;
}
