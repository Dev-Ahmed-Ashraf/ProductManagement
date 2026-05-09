import { HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { ApiResponse } from '../../Models/GenericResponse.model';
import { getApiStatusMessage } from '../../Models/Enums/api-status-code.enum';

@Injectable({ providedIn: 'root' })
export class ErrorHandlerService {
  private readonly toastr = inject(ToastrService);

  getMessage(error: unknown): string {
    if (error instanceof HttpErrorResponse) {
      return this.getHttpErrorMessage(error);
    }

    if (typeof error === 'object' && error !== null && 'message' in error) {
      return String(error.message);
    }

    if (typeof error === 'string' || typeof error === 'number' || typeof error === 'boolean') {
      return String(error);
    }

    return 'Unexpected error occurred';
  }

  showError(error: unknown, title = 'Error'): void {
    this.toastr.error(this.getMessage(error), title);
  }

  private getHttpErrorMessage(error: HttpErrorResponse): string {
    const apiError = error.error as Partial<ApiResponse<unknown>> & {
      statusCode?: number;
      errors?: string[];
      message?: string;
    };

    const apiStatusCode = apiError?.statusCode ?? null;
    const apiMessage = apiError?.message || null;
    const validationDetails = Array.isArray(apiError?.errors)
      ? apiError.errors.filter(Boolean)
      : [];

    if (error.status === 0) {
      return 'Unable to reach the server. Check your connection and try again.';
    }

    const businessMessage = getApiStatusMessage(apiStatusCode);
    return validationDetails.length > 0
      ? `${businessMessage} ${validationDetails.join(', ')}`
      : apiMessage || businessMessage || `HTTP Error ${error.status}`;
  }
}
