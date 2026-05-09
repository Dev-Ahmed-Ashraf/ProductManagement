export enum ApiStatusCode {
  Success = 200,
  InvalidCredentials = 401,
  ValidationError = 400,
  TokenExpired = 401,
  RefreshTokenExpired = 401,
  BadRequest = 400,
  Forbidden = 403,
  NotFound = 404,
  RequestTimeout = 408,
  TooManyRequests = 429,
  ServerError = 500,
}

export function getApiStatusMessage(code: number | null | undefined): string | null {
  if (code === null || code === undefined) {
    return null;
  }

  if (
    code === ApiStatusCode.InvalidCredentials ||
    code === ApiStatusCode.TokenExpired ||
    code === ApiStatusCode.RefreshTokenExpired
  ) {
    return 'Your session has expired or credentials are invalid. Please sign in again.';
  }

  if (code === ApiStatusCode.ValidationError || code === ApiStatusCode.BadRequest) {
    return 'The request was invalid. Please check your input and try again.';
  }

  switch (code) {
    case ApiStatusCode.Success:
      return 'Request completed successfully.';
    case ApiStatusCode.Forbidden:
      return 'You do not have permission to perform this action.';
    case ApiStatusCode.NotFound:
      return 'The requested resource was not found.';
    case ApiStatusCode.RequestTimeout:
      return 'The request timed out. Please try again later.';
    case ApiStatusCode.TooManyRequests:
      return 'You have made too many requests. Please wait and try again.';
    case ApiStatusCode.ServerError:
      return 'The server encountered an error. Please try again later.';
    default:
      return null;
  }
}
