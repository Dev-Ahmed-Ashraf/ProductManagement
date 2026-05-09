export interface LoginResponse {
  userId: string;
  fullName: string;
  email: string;
  roles: string[];
  claims: string[];
  accessToken: string;
  accessTokenExpiresAt: Date;
  refreshToken: string;
  refreshTokenExpiresAt: Date;
}
