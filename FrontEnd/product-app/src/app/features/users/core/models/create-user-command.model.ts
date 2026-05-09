export interface CreateUserCommand {
  fullName: string;
  email: string;
  password: string;
  confirmPassword: string;
  role: string;
}
