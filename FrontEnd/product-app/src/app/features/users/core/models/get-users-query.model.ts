export interface GetUsersQuery {
  pageNumber?: number;
  pageSize?: number;
  name?: string | null;
  userName?: string | null;
  email?: string | null;
  role?: string | null;
  isActive?: boolean | null;
}
